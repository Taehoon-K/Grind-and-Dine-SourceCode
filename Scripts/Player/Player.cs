using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using MK.Toon;

namespace Kupa
{
    public class Player : MonoBehaviour
    {
        [Header("Audio Clips")]

        [Tooltip("Step Audio")]
        /* [SerializeField]
         protected AudioClip audioClipWalking;
         [SerializeField]
         protected AudioClip audioClipRunning;*/
        [SerializeField] protected AudioClip[] footstepClips;
        [SerializeField] protected float stepInterval = 5f;

        protected float stepCycle;
        protected float nextStep;
        protected AudioSource footstepAudioSource;

        [Header("Mouse Settings")]

        [Tooltip("Sensitivity when looking around.")]
        [SerializeField]
        protected float sensitivity = 1f;

        private bool isIdle = false;
        public enum PlayerState { IDLE, FISH, UNDER_ATTACK, DEAD }

        [SerializeField] [Tooltip("�ȴ� �ӵ�")] protected float walkSpeed = 5.0f;
        [SerializeField] [Tooltip("�޸��� �ӵ�")] public float runSpeed = 10.0f;
        [SerializeField] [Tooltip("��ũ���� �ӵ�")] protected float crouchSpeed;
        [SerializeField] [Tooltip("����")] protected float jumpForce = 5f;
        [SerializeField] [Tooltip("ī�޶� �Ÿ�")] [Range(1f, 5f)] protected float cameraDistance = 3f;

        protected Vector2 axisMovement;
        protected Vector2 axisLook;
        //protected AudioSource audioSource;
        private readonly RaycastHit[] groundHits = new RaycastHit[8];
        //private CapsuleCollider capsule;
        protected bool holdingButtonRun;
        protected bool jumpRequest = false; // ���� ��û�� ���� ����

        protected Transform modelTransform;
        protected Transform cameraPivotTransform;
        [SerializeField]
        protected Transform cameraTransform;
        protected CharacterController characterController;
        protected Animator animator;
        [SerializeField]
        protected Animator animatorFirst;

        protected Vector3 mouseMove;      //ī�޶� ȸ����
        protected Vector3 moveVelocity;       //�̵� �ӵ�
        public PlayerState playerState = PlayerState.IDLE;
        public bool isRun,isCrouch;             //�޸��� ����
        protected bool IsRun { get { return isRun; } set { isRun = value; animator.SetBool("isRun", value); } }  //���� �����ϸ� �ִϸ����� ���� �ڵ����� ����ǵ���
        protected bool IsCrouch { get { return isCrouch; } set { isCrouch = value; animator.SetBool("isCrouch", value); } }  //���� �����ϸ� �ִϸ����� ���� �ڵ����� ����ǵ���
        protected bool isGroundedCheck;   //timeScale�� 0�� �ǰų� � ������ �� �����Ӹ� isGrounded�� false�� �Ǵ� ��� ����� Ƣ�� ������ �������� �뵵 
        bool sDown;
        public bool threePview = true;
        public bool isConver = false;
        private bool threePviewAgree = true;
        private bool threePviewed = false;

        private Rigidbody playerRigidbody;
        private CapsuleCollider playerCapsule;
        private Rigidbody[] ragdollRigidbodies;
        private Collider[] ragdollColliders;

        [Header("Slopes")]
        public bool slideDownSlopes = true;
        public float slopeSlideSpeed = 1;
        private Vector3 slopeDirection;
        [Tooltip("Camera offset from the player.")]
        public Vector3 offset = new Vector3(0, 0.7f, 0);
        [Tooltip("Player height while crouching.")]
        public float crouchHeight = 1.2f;
        public Vector3 crouchController = new Vector3(0, 0.8f, 0);

        [Header("Ragdoll")]
        [SerializeField]
        private float upwardForce;
        [SerializeField]
        private float impactForce = 1f;
        [SerializeField]
        private ScreenFadeOff screenFade;
        private bool isDead; //������ �����°� ������

        private bool isDangerDead; //���� �ڼ� �������°��� üũ
        Camera cameraDep;


        Yarn.Unity.DialogueRunner dialogueRun;

        public Transform eye; //�� ��ġ Ʈ������

        //input velocity
        private Vector3 desiredVelocityRef;
        private Vector3 desiredVelocity;
        private Vector3 slideVelocity;

        //out put velocity
        private Vector3 velocity;
        private float speed;
        protected float defaultHeight;
        protected Vector3 defalutController;

        private float cameraRotationSpeed = 2.0f;

        private Coroutine lookAtCoroutine; // ���� ���� ���� �ڷ�ƾ�� ����
        public CollisionFlags CollisionFlags { get; set; }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            cameraTransform = Camera.main.transform;
            cameraPivotTransform = cameraTransform.parent;
            animatorFirst = GameObject.Find("FirstPArms").GetComponent<Animator>();

            cameraDep = GameObject.Find("Main Camera").GetComponent<Camera>();
        }
        public void GetTransform()
        {
            modelTransform = transform.GetChild(3);
        }
        void Start()
        {
            dialogueRun = FindObjectOfType<Yarn.Unity.DialogueRunner>();

            //Audio Source Setup.
            footstepAudioSource = GetComponent<AudioSource>();

            sensitivity = PlayerPrefs.GetFloat("MouseSensSlider", 50f) / 20 + 0.1f;

            //get defaults
            defaultHeight = characterController.height;
            defalutController = characterController.center;

        }

        private void Update()   //ĳ���� ���� �� ��Ʈ�� �ݿ��� ���⼭ ����
        {
            IsRun = holdingButtonRun && CanRun();

            SwitchView();
            if (Time.timeScale < 0.001f)  //�Ͻ����� �� �ð��� ���� ���¿��� �Է� ����
            {
                isIdle = true;
                return; 
            }         
            if (dialogueRun.IsDialogueRunning == true)  //��ȭ�� ���ö� �̵� ����
            {
                isIdle = true;
                
                return;
            }
            isIdle = false;
            FreezeRotationXZ();     //CharacterController ĸ���� � �����ε� �������� �ʵ��� ����


            switch (playerState)
            {
                case PlayerState.IDLE:
                    //PlayerIdle();
                    break;
                
                case PlayerState.FISH:
                    animator.SetFloat("speedX", 0);
                    animator.SetFloat("speedY", 0);
                    PlayerFishing();
                    break;
                case PlayerState.DEAD:
                    animator.SetFloat("speedX", 0);
                    animator.SetFloat("speedY", 0);
                    break;
                default:
                    break;
            }

            CameraDistanceCtrl();   //ī�޶� �Ÿ� ����
        }
        private void FixedUpdate()
        {
            ProgressStepCycle();
            if (isIdle)
            {
                characterController.Move(Vector3.zero * Time.deltaTime);
                animator.SetFloat("speedX", 0);
                animator.SetFloat("speedY", 0);
            }
            else
            {
                NewMovement();
            }
        }

        public void LookNpc(Transform npcTrans) //���� npc���� ī�޶� ȸ��
        {
            lookAtCoroutine = StartCoroutine(LookAtTargetCoroutine(npcTrans));
        }

        private IEnumerator LookAtTargetCoroutine(Transform transTarget)
        {
            if (transTarget == null) yield break; // Ÿ���� ������ �ٷ� ����

            Vector3 targetPosition = transTarget.position + new Vector3(0, 1.25f, 0); // y ���� 1 ����
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - cameraPivotTransform.position);

            float elapsedTime = 0f; // ��� �ð�

            while (Quaternion.Angle(cameraPivotTransform.rotation, targetRotation) > 0.1f)
            {
                if (elapsedTime > 1f) yield break; // 1�� �̻� ������ ����

                //Debug.Log("lookokkokooookookokkk");
                cameraPivotTransform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, Time.deltaTime * cameraRotationSpeed);

                elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ
                yield return null;
            }

            // ���������� ��Ȯ�� ��ǥ ȸ�� ���� (�ε巯�� ����)
            cameraPivotTransform.rotation = targetRotation;
        }
        public void StopLooking() // �ڷ�ƾ ���� ����
        {
            if (lookAtCoroutine != null)
            {
                StopCoroutine(lookAtCoroutine);
                lookAtCoroutine = null;
            }
        }


        public void OnInventory()
        {
            if (threePview)
            {
                threePview = false;
                threePviewed = true;
                cameraDep.cullingMask = cameraDep.cullingMask & ~(1 << LayerMask.NameToLayer("Player3rd"));
                cameraDep.cullingMask |= 1 << LayerMask.NameToLayer("FP_");
            }
            threePviewAgree = false;
        }
        public void OffInventory()
        {
            if (threePviewed)
            {
                threePview = true;
                threePviewed = false;
                cameraDep.cullingMask = cameraDep.cullingMask & ~(1 << LayerMask.NameToLayer("FP_"));
                cameraDep.cullingMask |= 1 << LayerMask.NameToLayer("Player3rd");
            }
            threePviewAgree = true;
        }

        void SwitchView()
        {
            if (sDown&&!threePview&&threePviewAgree)
            {
                threePview = true;
                cameraDep.cullingMask = cameraDep.cullingMask & ~(1 << LayerMask.NameToLayer("FP_"));
                cameraDep.cullingMask |= 1 << LayerMask.NameToLayer("Player3rd");
                sDown = false; // �ѹ� ��ȯ �� sDown �ʱ�ȭ

                modelTransform.rotation = Quaternion.Euler(0f, cameraPivotTransform.eulerAngles.y, 0f);
            }
            else if (sDown && threePview&&threePviewAgree)
            {
                threePview = false;
                cameraDep.cullingMask = cameraDep.cullingMask & ~(1 << LayerMask.NameToLayer("Player3rd"));
                cameraDep.cullingMask |= 1 << LayerMask.NameToLayer("FP_");
                sDown = false; // �ѹ� ��ȯ �� sDown �ʱ�ȭ
            }
        }
        public void ResetRotation(Transform rotation) //�� ��ȯ�� ���� ����
        {
            Quaternion rotations = rotation.rotation;
            mouseMove = rotations.eulerAngles;
        }

        public void ResetCamrea() //�ƾ� ���� �� ���� ����
        {
            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localRotation = Quaternion.identity;
        }

        private void LateUpdate()       //���� ī�޶� ������ ���⼭ ����
        {
            if (Time.timeScale < 0.001f) return;         //�Ͻ����� �� �ð��� ���� ���¿��� �Է� ����

            float cameraHeight = 1.3f;
            mouseMove += new Vector3(-axisLook.y * sensitivity, axisLook.x * sensitivity, 0);

            //ī�޶� ���� ������ ����
            mouseMove.x = Mathf.Clamp(mouseMove.x, -60f, 60f);

            if (playerState == PlayerState.IDLE && !isIdle) //���� �����϶��� ���콺 ���� ����
            {
                //���콺 �������� ȭ�������
                cameraPivotTransform.localEulerAngles = mouseMove;
            }

            if (threePview)
            {
                cameraPivotTransform.position = transform.position + Vector3.up * cameraHeight;  //ĳ������ ���� ������ 

                RaycastHit cameraWallHit;   //ī�޶� �� �ڷ� ���� ȭ���� �������� ���� ����
                if (Physics.Raycast(cameraPivotTransform.position, cameraTransform.position 
                    - cameraPivotTransform.position, out cameraWallHit, cameraDistance, ~(1 << LayerMask.NameToLayer("Player3rd"))))   
                    //�÷��̾��� �ݶ��̴��� ������ �ʵ���
                    cameraTransform.localPosition = Vector3.back * (cameraWallHit.distance * 0.9f);
                else
                    cameraTransform.localPosition = Vector3.back * cameraDistance + Vector3.right * 0.5f; // + ������ 1f��ŭ

            }
            else
            {
                cameraPivotTransform.position = transform.position + ((transform.up * characterController.height / 2) + offset);
                cameraTransform.localPosition = Vector3.zero;

                if (playerState == PlayerState.DEAD) //���� �׾��ٸ�
                {
                    cameraPivotTransform.rotation = eye.rotation;
                }      
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "EnemyAttack")
            {
                //PlayerUnderAttackEnter(other.GetComponent<PlayerHitObject>());
            }
        }

        

        private void PlayerFishing()
        {

        }
        /*private void PlayerUnderAttackEnter(PlayerHitObject playerHit)
        {
            playerState = PlayerState.UNDER_ATTACK;
            if (isSuperArmor == false && playerState != PlayerState.DEAD)
                animator.SetTrigger("UnderAttack");
            playerHealthPoint -= playerHit.damage;
            if (playerHealthPoint <= 0f)
            {
                playerHealthPoint = 0f;
                playerState = PlayerState.DEAD;
                animator.SetBool("isDead", true);
            }
        }*/
        private void PlayerDead(bool isDanger)
        {
            isDangerDead = isDanger;
            isDead = true;

            screenFade.StartFadeOut(AfterDeathAction); //���̵�ƿ� ���� �� �ݹ� ����    
        }
        private void AfterDeathAction()
        {
            

            if(TutorialManager.instance != null) //Ʃ�丮�����̶��
            {
                gameObject.SetActive(false); // ��Ȱ��ȭ
                gameObject.transform.localPosition = Vector3.zero; // ��ġ ����
                gameObject.SetActive(true); // �ٽ� Ȱ��ȭ

                screenFade.OffFade();
                return;
            }

            if (!isDangerDead) //���� �׳� ������ ���������
            {
                if (SceneTransitionManager.Instance.currentLocation != SceneTransitionManager.Location.HomeGround) //���� ������ ������ ��� �ƴϸ�
                {
                    SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.HospitalCut);
                    
                    screenFade.OffFade();
                }
                else
                {
                    UIManager.instance.StartScreenFade(true);
                }
                GameTimeStateManager.instance.OnIsSleep(true);
                //TimeManager.instance.SkipTimeToSix(); //6�ñ��� �ð� ������
            }
            else
            {
                if (StatusManager.instance.GetMoodle()[9].isActive) //���� ����â�� ���¶��
                {
                    //������ ȭ�� ����
                    UIManager.instance.GameOver(true);
                }
                else
                {
                    SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.HospitalCut);
                    screenFade.OffFade();

                   // StatusManager.instance.MoodleChange(9, true, 48 * 60); //48�ð� ����â��
                }
                
            }
            /*
            // ���̵� �ƿ��� ���� �� ����� �ڵ�
            SetRagdollState(false); //�ٽ� ���׵� ����
            isDead = false;
            playerState = PlayerState.IDLE;

            isDangerDead = false;  */  //�� �Ѿ�� ������ ���׵� Ǯ���鼭 �ѹ��� �ε����� �ϴ� �ڷ�ƾ���� ������������ �ٲ�

            StartCoroutine(DelayedRagdoll(2f));
        }
        private IEnumerator DelayedRagdoll(float delay)
        {
            yield return new WaitForSeconds(delay);
            // ���̵� �ƿ��� ���� �� ����� �ڵ�
            SetRagdollState(false); //�ٽ� ���׵� ����
            isDead = false;
            playerState = PlayerState.IDLE;

            isDangerDead = false;
        }

        protected void FreezeRotationXZ()
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);   //������ ����
        }

        private void CameraDistanceCtrl()
        {
            if (threePview == true)
            {
                //cameraDistance -= Input.GetAxisRaw("Mouse ScrollWheel");
            }    //�ٷ� ī�޶��� �Ÿ��� ����
        }

        private bool CanRun()
        {
            //���� ������ ���� �ִٸ� ���
            if (StatusManager.instance.GetMoodle()[5].isActive)
            {
                return false;
            }

            //Block.
            if (StatusManager.instance.GetCurrentSP() <= 0)
                return false;

            if (IsCrouch) //��ũ�������� ���޸���
            {
                return false;
            }

            //This blocks running backwards, or while fully moving sideways.
            if (axisMovement.y <= 0 || Math.Abs(Mathf.Abs(axisMovement.x) - 1) < 0.01f)
                return false;

            //Return.
            return true;
        }

        
        protected bool GroundCheck(out RaycastHit hit)
        {
            //CharacterController�� isGrounded�� ���� ������ �� Move ���� �Լ��� �� ������ ���Ͽ����� ������ �Ǿ�߸� true�� �����Ѵ�.
            //�̴� ���θ� �������ų� ���������� ������ �ٴҶ� ���÷� false ���� �����ϹǷ� Raycast�� ���� �� �� �� üũ�Ͽ� �������� ��ȭ�Ѵ�..
            Debug.DrawRay(transform.position + Vector3.up * 0.07f, Vector3.down * 0.06f);
            Debug.DrawRay(transform.position + Vector3.up * 0.07f + Vector3.right*0.3f, Vector3.down * 0.06f+ Vector3.right * 0.03f);
            Debug.DrawRay(transform.position + Vector3.up * 0.07f + Vector3.left * 0.3f, Vector3.down * 0.06f + Vector3.left * 0.03f);
            return Physics.Raycast(transform.position + Vector3.up * 0.07f, Vector3.down, out hit, 0.06f)
                ||Physics.Raycast(transform.position+Vector3.up*0.07f + Vector3.right * 0.3f, Vector3.down, out hit, 0.06f)
                || Physics.Raycast(transform.position + Vector3.up * 0.07f + Vector3.left * 0.3f, Vector3.down, out hit, 0.06f);
            
        }

       

        public void OnLook(InputAction.CallbackContext context)
        {
            //Read.
            axisLook = context.ReadValue<Vector2>();
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            //Read.
            axisMovement = context.ReadValue<Vector2>();
        }
        public void OnTryRun(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                //Started.
                case InputActionPhase.Started:
                    //Start.
                    holdingButtonRun = true;
                    break;
                //Canceled.
                case InputActionPhase.Canceled:
                    //Stop.
                    holdingButtonRun = false;
                    break;
            }
        }
        public void OnTryCrouch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                // Toggle IsCrouch value on key press
                IsCrouch = !IsCrouch;
            }
        }
        public void OnSwitch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                sDown = !sDown;
            }
        }
        public virtual void OnJump(InputAction.CallbackContext context)
        {     
            if (context.phase == InputActionPhase.Started)
            {
                if (StatusManager.instance.GetCurrentSP() > 100 && !IsCrouch)
                { 
                    jumpRequest = true; // ���� ��û ����        
                }
            }
        }


        public void SetupRagdoll()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            playerCapsule = GetComponent<CapsuleCollider>();

            // �ڽ� ������Ʈ�� �� Rigidbody�� Collider�� ���� ��Ʈ�� ������
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();

            // ���׵� �ʱ� ����: ��� ��Ʈ ��Ȱ��ȭ
            SetRagdollState(false);
        }

        // ���׵� Ȱ��ȭ/��Ȱ��ȭ ����
        private void SetRagdollState(bool isActive)
        {
            // ���׵��� Ȱ��ȭ�Ǹ� �ִϸ����� ��Ȱ��ȭ
            animator.enabled = !isActive;

            if (characterController != null)
            {
                characterController.enabled = !isActive; // ���׵� Ȱ��ȭ �� Character Controller ��Ȱ��ȭ
                playerCapsule.enabled = !isActive; // ���׵� Ȱ��ȭ �� ��Ȱ��ȭ

            }

            foreach (var col in ragdollColliders)
            {
                if (col != playerCapsule && col != characterController) // ���� Collider�� ����
                    col.enabled = isActive;
            }
            foreach (var rb in ragdollRigidbodies)
            {
                if (rb != playerRigidbody) // ���� Rigidbody�� ����
                    rb.isKinematic = !isActive;
            }        
        }

        // �浹 ����
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Car")) // ���� �浹 ��
            {
                if (!isDead)
                {
                    SoundManager.instance.PlaySound2D("car-crash");

                    Debug.Log("Carrashhhhhhh");
                    ActivateRagdoll(collision);

                    PlayerDead(true); //����

                    playerState = PlayerState.DEAD;
                }
            }
        }

        // ���׵� Ȱ��ȭ �� ���ư��� �� ����
        private void ActivateRagdoll(Collision collision)
        {
            SetRagdollState(true); // ���׵� Ȱ��ȭ

            // �浹 �������� ���� ���� �÷��̾ ���� ����
            Vector3 impactDirection = collision.contacts[0].point - transform.position;
            impactDirection = impactDirection.normalized;

            // Y������ �߰����� ���� ���ϱ� ���� ���� ����
            impactDirection.y += upwardForce; // upwardForce: ���� ������ ���� (��: 1.0f)

            foreach (var rb in ragdollRigidbodies)
            {
                rb.AddForce(-impactDirection * impactForce, ForceMode.Impulse);
            }


        }

        public void SetSenstive(float senstive)
        {
            sensitivity = senstive / 20 + 0.1f;
        }

        public void OnFaint() //2�ÿ� ���� �� ȣ��
        {
            SetRagdollState(true); // ���׵� Ȱ��ȭ

            PlayerDead(false); //����

            playerState = PlayerState.DEAD;
        }

        public void OnDead() //HP���� ������ ȣ��
        {
            SetRagdollState(true); // ���׵� Ȱ��ȭ

            PlayerDead(true); //����

            playerState = PlayerState.DEAD;
        }
        

        protected virtual void NewMovement()
        {
            animator.applyRootMotion = threePview; //3��Ī�϶��� ��Ʈ��� Ȱ��ȭ

            // ���� �̲����� ó��
            if (slideDownSlopes && OnMaxedAngleSlope())
                slideVelocity += new Vector3(slopeDirection.x, -slopeDirection.y, slopeDirection.z) * slopeSlideSpeed * Time.deltaTime;
            else
                slideVelocity = Vector3.zero;

            // ��ǥ �ӵ� ��� (Slope ���� + ī�޶� ���� ���)
            desiredVelocity = slideVelocity + Vector3.SmoothDamp(desiredVelocity,
                (SlopeDirection() * axisMovement.y + cameraPivotTransform.right * axisMovement.x).normalized * speed,
                ref desiredVelocityRef, 0.1f);

            // ��ũ����� ĸ�� ũ�� ����
            characterController.height = IsCrouch ?
                Mathf.Lerp(characterController.height, crouchHeight, Time.deltaTime * 15) :
                Mathf.Lerp(characterController.height, defaultHeight, Time.deltaTime * 15);
            characterController.center = IsCrouch ? crouchController : defalutController;

            // �߷� �� ����
            if (characterController.isGrounded)
            {
                velocity.y = Physics.gravity.y * 0.5f;
            }
            else if (velocity.magnitude * 3.5f < 350)
            {
                velocity += Physics.gravity * Time.deltaTime;
            }

            // �޸���, �ȱ�, ��ũ���⿡ ���� �ӵ� ����
            if (IsRun)
                speed = runSpeed;
            else if (IsCrouch)
                speed = crouchSpeed;
            else
                speed = walkSpeed;

            // �̵� ����
            velocity.x = desiredVelocity.x;
            velocity.z = desiredVelocity.z;

            // �ִϸ��̼��� �׻� �Է°� �ݿ� (1��Ī�̾)
            animator.SetFloat("speedX", axisMovement.x);
            animator.SetFloat("speedY", axisMovement.y);

            if (!threePview)
            {
                characterController.Move(velocity * Time.deltaTime);
            }
            

            // 3��Ī�̵� 1��Ī�̵� �̵� �Է��� ���� ���� �� ȸ�� ó��
            Vector2 frameInput = axisMovement.normalized;
            moveVelocity = new Vector3(frameInput.x, 0, frameInput.y).normalized;
            moveVelocity = transform.TransformDirection(moveVelocity);

            if (moveVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion cameraRotation = cameraPivotTransform.rotation;
                cameraRotation.x = cameraRotation.z = 0;

                if (threePview)
                {
                    //  3��Ī�� ���� transform.rotation�� ī�޶� �������� ���� ȸ��
                    transform.rotation = cameraRotation;
                }

                if (IsRun)
                {
                    animatorFirst.SetBool("isRun", true);

                    // ����(modelTransform)�� �̵� ���� ���� �ε巴�� ȸ�� (1��Ī/3��Ī ����)
                    Quaternion characterRotation = Quaternion.LookRotation(moveVelocity);
                    characterRotation.x = characterRotation.z = 0;
                    modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, characterRotation, 10.0f * Time.deltaTime);

                    if (StatusManager.instance != null) StatusManager.instance.isRun = true;
                }
                else
                {
                    animatorFirst.SetBool("isRun", false);

                    // �ȱ� ������ �� ������ ī�޶� ���� �ε巴�� ����
                    modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, cameraRotation, 10.0f * Time.deltaTime);

                    if (StatusManager.instance != null) StatusManager.instance.isRun = false;
                }
            }

            // �߰�: ��ũ���� �ִϸ��̼� ó��
            animator.SetBool("isCrouch", IsCrouch);
        }
        public virtual bool OnMaxedAngleSlope()
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, characterController.height))
            {
                slopeDirection = hit.normal;
                return Vector3.Angle(slopeDirection, Vector3.up) > characterController.slopeLimit;
            }

            return false;
        }
        public virtual Vector3 SlopeDirection()
        {
            //setup a raycast from position to down at the bottom of the collider
            RaycastHit slopeHit;
            if (Physics.Raycast(cameraPivotTransform.position, Vector3.down, out slopeHit, (characterController.height / 2) + 0.1f))
            {
                //get the direction result according to slope normal
                return Vector3.ProjectOnPlane(cameraPivotTransform.forward, slopeHit.normal);
            }

            //if not on slope then slope is forward ;)
            return cameraPivotTransform.forward;
        }

        public virtual float SlopeAngle()
        {
            //setup a raycast from position to down at the bottom of the collider
            RaycastHit slopeHit;
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit))
            {
                //get the direction result according to slope normal
                return (Vector3.Angle(Vector3.down, slopeHit.normal) - 180) * -1;
            }

            //if not on slope then slope is forward ;)
            return 0;
        }

        protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //if hit something while jumping from the above then go down again
            if (CollisionFlags == CollisionFlags.Above)
            {
                velocity.y = 0;
            }
        }

        //�߼Ҹ� �κ�
        protected void ProgressStepCycle()
        {
            // ���� ���̰ų� ���߿� �ְų� Ÿ�ӽ������� 0�̸� �н�
            if (!characterController.isGrounded || Time.timeScale == 0f || axisMovement == Vector2.zero || IsCrouch) return;

            // �̵� ���̸� ����Ŭ ����
            float currentSpeed = isRun ? runSpeed : walkSpeed;
            stepCycle += (characterController.velocity.magnitude + currentSpeed) * Time.fixedDeltaTime;

            if (stepCycle > nextStep)
            {
                nextStep = stepCycle + stepInterval;
                PlayFootstepSound();
            }
        }

        protected void PlayFootstepSound()
        {
            if (footstepClips == null || footstepClips.Length == 0) return;

            AudioClip clip = footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)];
            footstepAudioSource.clip = clip;
            footstepAudioSource.Play();
        }

    }
}