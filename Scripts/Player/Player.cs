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

        [SerializeField] [Tooltip("걷는 속도")] protected float walkSpeed = 5.0f;
        [SerializeField] [Tooltip("달리는 속도")] public float runSpeed = 10.0f;
        [SerializeField] [Tooltip("웅크리는 속도")] protected float crouchSpeed;
        [SerializeField] [Tooltip("점프")] protected float jumpForce = 5f;
        [SerializeField] [Tooltip("카메라 거리")] [Range(1f, 5f)] protected float cameraDistance = 3f;

        protected Vector2 axisMovement;
        protected Vector2 axisLook;
        //protected AudioSource audioSource;
        private readonly RaycastHit[] groundHits = new RaycastHit[8];
        //private CapsuleCollider capsule;
        protected bool holdingButtonRun;
        protected bool jumpRequest = false; // 점프 요청을 위한 변수

        protected Transform modelTransform;
        protected Transform cameraPivotTransform;
        [SerializeField]
        protected Transform cameraTransform;
        protected CharacterController characterController;
        protected Animator animator;
        [SerializeField]
        protected Animator animatorFirst;

        protected Vector3 mouseMove;      //카메라 회전값
        protected Vector3 moveVelocity;       //이동 속도
        public PlayerState playerState = PlayerState.IDLE;
        public bool isRun,isCrouch;             //달리기 상태
        protected bool IsRun { get { return isRun; } set { isRun = value; animator.SetBool("isRun", value); } }  //값을 변경하면 애니메이터 값도 자동으로 변경되도록
        protected bool IsCrouch { get { return isCrouch; } set { isCrouch = value; animator.SetBool("isCrouch", value); } }  //값을 변경하면 애니메이터 값도 자동으로 변경되도록
        protected bool isGroundedCheck;   //timeScale이 0이 되거나 어떤 이유로 한 프레임만 isGrounded가 false가 되는 경우 모션이 튀는 증상을 막기위한 용도 
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
        private bool isDead; //여러번 뒤지는거 방지용

        private bool isDangerDead; //만약 자서 쓰러지는건지 체크
        Camera cameraDep;


        Yarn.Unity.DialogueRunner dialogueRun;

        public Transform eye; //눈 위치 트랜스폼

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

        private Coroutine lookAtCoroutine; // 현재 실행 중인 코루틴을 저장
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

        private void Update()   //캐릭터 조정 및 컨트롤 반영은 여기서 진행
        {
            IsRun = holdingButtonRun && CanRun();

            SwitchView();
            if (Time.timeScale < 0.001f)  //일시정지 등 시간을 멈춘 상태에선 입력 방지
            {
                isIdle = true;
                return; 
            }         
            if (dialogueRun.IsDialogueRunning == true)  //대화문 나올때 이동 금지
            {
                isIdle = true;
                
                return;
            }
            isIdle = false;
            FreezeRotationXZ();     //CharacterController 캡슐이 어떤 이유로든 기울어지지 않도록 방지


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

            CameraDistanceCtrl();   //카메라 거리 조작
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

        public void LookNpc(Transform npcTrans) //말건 npc한테 카메라 회전
        {
            lookAtCoroutine = StartCoroutine(LookAtTargetCoroutine(npcTrans));
        }

        private IEnumerator LookAtTargetCoroutine(Transform transTarget)
        {
            if (transTarget == null) yield break; // 타겟이 없으면 바로 종료

            Vector3 targetPosition = transTarget.position + new Vector3(0, 1.25f, 0); // y 값만 1 증가
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - cameraPivotTransform.position);

            float elapsedTime = 0f; // 경과 시간

            while (Quaternion.Angle(cameraPivotTransform.rotation, targetRotation) > 0.1f)
            {
                if (elapsedTime > 1f) yield break; // 1초 이상 지나면 종료

                //Debug.Log("lookokkokooookookokkk");
                cameraPivotTransform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, Time.deltaTime * cameraRotationSpeed);

                elapsedTime += Time.deltaTime; // 경과 시간 업데이트
                yield return null;
            }

            // 최종적으로 정확한 목표 회전 적용 (부드러운 종료)
            cameraPivotTransform.rotation = targetRotation;
        }
        public void StopLooking() // 코루틴 강제 종료
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
                sDown = false; // 한번 전환 후 sDown 초기화

                modelTransform.rotation = Quaternion.Euler(0f, cameraPivotTransform.eulerAngles.y, 0f);
            }
            else if (sDown && threePview&&threePviewAgree)
            {
                threePview = false;
                cameraDep.cullingMask = cameraDep.cullingMask & ~(1 << LayerMask.NameToLayer("Player3rd"));
                cameraDep.cullingMask |= 1 << LayerMask.NameToLayer("FP_");
                sDown = false; // 한번 전환 후 sDown 초기화
            }
        }
        public void ResetRotation(Transform rotation) //씬 전환시 시점 리셋
        {
            Quaternion rotations = rotation.rotation;
            mouseMove = rotations.eulerAngles;
        }

        public void ResetCamrea() //컷씬 끝날 시 시점 리셋
        {
            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localRotation = Quaternion.identity;
        }

        private void LateUpdate()       //최종 카메라 보정은 여기서 진행
        {
            if (Time.timeScale < 0.001f) return;         //일시정지 등 시간을 멈춘 상태에선 입력 방지

            float cameraHeight = 1.3f;
            mouseMove += new Vector3(-axisLook.y * sensitivity, axisLook.x * sensitivity, 0);

            //카메라 수직 움직임 제한
            mouseMove.x = Mathf.Clamp(mouseMove.x, -60f, 60f);

            if (playerState == PlayerState.IDLE && !isIdle) //정상 상태일때만 마우스 조작 가능
            {
                //마우스 방향으로 화면움직임
                cameraPivotTransform.localEulerAngles = mouseMove;
            }

            if (threePview)
            {
                cameraPivotTransform.position = transform.position + Vector3.up * cameraHeight;  //캐릭터의 가슴 높이쯤 

                RaycastHit cameraWallHit;   //카메라가 벽 뒤로 가서 화면이 가려지는 것을 방지
                if (Physics.Raycast(cameraPivotTransform.position, cameraTransform.position 
                    - cameraPivotTransform.position, out cameraWallHit, cameraDistance, ~(1 << LayerMask.NameToLayer("Player3rd"))))   
                    //플레이어의 콜라이더에 막히지 않도록
                    cameraTransform.localPosition = Vector3.back * (cameraWallHit.distance * 0.9f);
                else
                    cameraTransform.localPosition = Vector3.back * cameraDistance + Vector3.right * 0.5f; // + 오른쪽 1f만큼

            }
            else
            {
                cameraPivotTransform.position = transform.position + ((transform.up * characterController.height / 2) + offset);
                cameraTransform.localPosition = Vector3.zero;

                if (playerState == PlayerState.DEAD) //만약 죽었다면
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

            screenFade.StartFadeOut(AfterDeathAction); //페이드아웃 실행 후 콜백 받음    
        }
        private void AfterDeathAction()
        {
            

            if(TutorialManager.instance != null) //튜토리얼중이라면
            {
                gameObject.SetActive(false); // 비활성화
                gameObject.transform.localPosition = Vector3.zero; // 위치 설정
                gameObject.SetActive(true); // 다시 활성화

                screenFade.OffFade();
                return;
            }

            if (!isDangerDead) //만약 그냥 졸려서 쓰러진경우
            {
                if (SceneTransitionManager.Instance.currentLocation != SceneTransitionManager.Location.HomeGround) //만약 집에서 쓰러진 경우 아니면
                {
                    SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.HospitalCut);
                    
                    screenFade.OffFade();
                }
                else
                {
                    UIManager.instance.StartScreenFade(true);
                }
                GameTimeStateManager.instance.OnIsSleep(true);
                //TimeManager.instance.SkipTimeToSix(); //6시까지 시간 보내기
            }
            else
            {
                if (StatusManager.instance.GetMoodle()[9].isActive) //만약 만신창이 상태라면
                {
                    //뒤지는 화면 열기
                    UIManager.instance.GameOver(true);
                }
                else
                {
                    SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.HospitalCut);
                    screenFade.OffFade();

                   // StatusManager.instance.MoodleChange(9, true, 48 * 60); //48시간 만신창이
                }
                
            }
            /*
            // 페이드 아웃이 끝난 후 실행될 코드
            SetRagdollState(false); //다시 래그돌 끄기
            isDead = false;
            playerState = PlayerState.IDLE;

            isDangerDead = false;  */  //씬 넘어가기 직전에 레그돌 풀리면서 한번더 부딪혀서 일단 코루틴으로 지연실행으로 바꿈

            StartCoroutine(DelayedRagdoll(2f));
        }
        private IEnumerator DelayedRagdoll(float delay)
        {
            yield return new WaitForSeconds(delay);
            // 페이드 아웃이 끝난 후 실행될 코드
            SetRagdollState(false); //다시 래그돌 끄기
            isDead = false;
            playerState = PlayerState.IDLE;

            isDangerDead = false;
        }

        protected void FreezeRotationXZ()
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);   //기울어짐 방지
        }

        private void CameraDistanceCtrl()
        {
            if (threePview == true)
            {
                //cameraDistance -= Input.GetAxisRaw("Mouse ScrollWheel");
            }    //휠로 카메라의 거리를 조절
        }

        private bool CanRun()
        {
            //만약 근육통 무들 있다면 블록
            if (StatusManager.instance.GetMoodle()[5].isActive)
            {
                return false;
            }

            //Block.
            if (StatusManager.instance.GetCurrentSP() <= 0)
                return false;

            if (IsCrouch) //웅크려있을때 못달리게
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
            //CharacterController의 isGrounded는 이전 프레임 때 Move 등의 함수로 땅 쪽으로 향하였을때 접지가 되어야만 true를 리턴한다.
            //이는 경사로를 내려가거나 울퉁불퉁한 지면을 다닐때 수시로 false 값을 리턴하므로 Raycast로 땅을 한 번 더 체크하여 안정성을 강화한다..
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
                    jumpRequest = true; // 점프 요청 설정        
                }
            }
        }


        public void SetupRagdoll()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            playerCapsule = GetComponent<CapsuleCollider>();

            // 자식 오브젝트들 중 Rigidbody와 Collider를 가진 파트를 가져옴
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();

            // 레그돌 초기 설정: 모든 파트 비활성화
            SetRagdollState(false);
        }

        // 레그돌 활성화/비활성화 설정
        private void SetRagdollState(bool isActive)
        {
            // 레그돌이 활성화되면 애니메이터 비활성화
            animator.enabled = !isActive;

            if (characterController != null)
            {
                characterController.enabled = !isActive; // 레그돌 활성화 시 Character Controller 비활성화
                playerCapsule.enabled = !isActive; // 레그돌 활성화 시 비활성화

            }

            foreach (var col in ragdollColliders)
            {
                if (col != playerCapsule && col != characterController) // 메인 Collider는 제외
                    col.enabled = isActive;
            }
            foreach (var rb in ragdollRigidbodies)
            {
                if (rb != playerRigidbody) // 메인 Rigidbody는 제외
                    rb.isKinematic = !isActive;
            }        
        }

        // 충돌 감지
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Car")) // 차와 충돌 시
            {
                if (!isDead)
                {
                    SoundManager.instance.PlaySound2D("car-crash");

                    Debug.Log("Carrashhhhhhh");
                    ActivateRagdoll(collision);

                    PlayerDead(true); //뒤짐

                    playerState = PlayerState.DEAD;
                }
            }
        }

        // 레그돌 활성화 및 날아가는 힘 적용
        private void ActivateRagdoll(Collision collision)
        {
            SetRagdollState(true); // 레그돌 활성화

            // 충돌 지점에서 힘을 가해 플레이어를 날려 보냄
            Vector3 impactDirection = collision.contacts[0].point - transform.position;
            impactDirection = impactDirection.normalized;

            // Y축으로 추가적인 힘을 가하기 위해 방향 수정
            impactDirection.y += upwardForce; // upwardForce: 위로 날리는 정도 (예: 1.0f)

            foreach (var rb in ragdollRigidbodies)
            {
                rb.AddForce(-impactDirection * impactForce, ForceMode.Impulse);
            }


        }

        public void SetSenstive(float senstive)
        {
            sensitivity = senstive / 20 + 0.1f;
        }

        public void OnFaint() //2시에 기절 시 호출
        {
            SetRagdollState(true); // 레그돌 활성화

            PlayerDead(false); //뒤짐

            playerState = PlayerState.DEAD;
        }

        public void OnDead() //HP없어 뒤질때 호출
        {
            SetRagdollState(true); // 레그돌 활성화

            PlayerDead(true); //뒤짐

            playerState = PlayerState.DEAD;
        }
        

        protected virtual void NewMovement()
        {
            animator.applyRootMotion = threePview; //3인칭일때만 루트모션 활성화

            // 경사면 미끄러짐 처리
            if (slideDownSlopes && OnMaxedAngleSlope())
                slideVelocity += new Vector3(slopeDirection.x, -slopeDirection.y, slopeDirection.z) * slopeSlideSpeed * Time.deltaTime;
            else
                slideVelocity = Vector3.zero;

            // 목표 속도 계산 (Slope 방향 + 카메라 방향 기반)
            desiredVelocity = slideVelocity + Vector3.SmoothDamp(desiredVelocity,
                (SlopeDirection() * axisMovement.y + cameraPivotTransform.right * axisMovement.x).normalized * speed,
                ref desiredVelocityRef, 0.1f);

            // 웅크리기시 캡슐 크기 조정
            characterController.height = IsCrouch ?
                Mathf.Lerp(characterController.height, crouchHeight, Time.deltaTime * 15) :
                Mathf.Lerp(characterController.height, defaultHeight, Time.deltaTime * 15);
            characterController.center = IsCrouch ? crouchController : defalutController;

            // 중력 및 점프
            if (characterController.isGrounded)
            {
                velocity.y = Physics.gravity.y * 0.5f;
            }
            else if (velocity.magnitude * 3.5f < 350)
            {
                velocity += Physics.gravity * Time.deltaTime;
            }

            // 달리기, 걷기, 웅크리기에 따른 속도 설정
            if (IsRun)
                speed = runSpeed;
            else if (IsCrouch)
                speed = crouchSpeed;
            else
                speed = walkSpeed;

            // 이동 적용
            velocity.x = desiredVelocity.x;
            velocity.z = desiredVelocity.z;

            // 애니메이션은 항상 입력값 반영 (1인칭이어도)
            animator.SetFloat("speedX", axisMovement.x);
            animator.SetFloat("speedY", axisMovement.y);

            if (!threePview)
            {
                characterController.Move(velocity * Time.deltaTime);
            }
            

            // 3인칭이든 1인칭이든 이동 입력이 있을 때만 몸 회전 처리
            Vector2 frameInput = axisMovement.normalized;
            moveVelocity = new Vector3(frameInput.x, 0, frameInput.y).normalized;
            moveVelocity = transform.TransformDirection(moveVelocity);

            if (moveVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion cameraRotation = cameraPivotTransform.rotation;
                cameraRotation.x = cameraRotation.z = 0;

                if (threePview)
                {
                    //  3인칭일 때만 transform.rotation을 카메라 방향으로 강제 회전
                    transform.rotation = cameraRotation;
                }

                if (IsRun)
                {
                    animatorFirst.SetBool("isRun", true);

                    // 몸통(modelTransform)은 이동 방향 따라 부드럽게 회전 (1인칭/3인칭 공통)
                    Quaternion characterRotation = Quaternion.LookRotation(moveVelocity);
                    characterRotation.x = characterRotation.z = 0;
                    modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, characterRotation, 10.0f * Time.deltaTime);

                    if (StatusManager.instance != null) StatusManager.instance.isRun = true;
                }
                else
                {
                    animatorFirst.SetBool("isRun", false);

                    // 걷기 상태일 때 몸통은 카메라 방향 부드럽게 따라감
                    modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, cameraRotation, 10.0f * Time.deltaTime);

                    if (StatusManager.instance != null) StatusManager.instance.isRun = false;
                }
            }

            // 추가: 웅크리기 애니메이션 처리
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

        //발소리 부분
        protected void ProgressStepCycle()
        {
            // 정지 중이거나 공중에 있거나 타임스케일이 0이면 패스
            if (!characterController.isGrounded || Time.timeScale == 0f || axisMovement == Vector2.zero || IsCrouch) return;

            // 이동 중이면 사이클 진행
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