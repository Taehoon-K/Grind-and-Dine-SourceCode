using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kupa
{
    public class PlayerMini : MonoBehaviour
    {
        

        public enum PlayerState { IDLE, ATTACK, UNDER_ATTACK, DEAD }

        [SerializeField] [Tooltip("�ȴ� �ӵ�")] private float walkSpeed = 3.0f;
        [SerializeField] [Tooltip("�޸��� �ӵ�")] private float runSpeed = 6.0f;
        [SerializeField] [Tooltip("����")] float jumpForce = 5f;
        [SerializeField] [Tooltip("ī�޶� �Ÿ�")] [Range(1f, 5f)] private float cameraDistance = 3f;


        
        private Transform modelTransform;
        private Transform cameraPivotTransform;
        [SerializeField]
        private Transform cameraTransform;
        private CharacterController characterController;
        private Animator animator;
        [SerializeField]
        private Animator animatorFirst;

        private Vector3 mouseMove;      //ī�޶� ȸ����
        private Vector3 moveVelocity;       //�̵� �ӵ�
        private PlayerState playerState = PlayerState.IDLE;
        public bool isRun,isCrouch;             //�޸��� ����
        bool IsRun { get { return isRun; } set { isRun = value; animator.SetBool("isRun", value); } }  //���� �����ϸ� �ִϸ����� ���� �ڵ����� ����ǵ���
        bool IsCrouch { get { return isCrouch; } set { isCrouch = value; animator.SetBool("isCrouch", value); } }  //���� �����ϸ� �ִϸ����� ���� �ڵ����� ����ǵ���
        private bool isGroundedCheck;   //timeScale�� 0�� �ǰų� � ������ �� �����Ӹ� isGrounded�� false�� �Ǵ� ��� ����� Ƣ�� ������ �������� �뵵 
        bool jDown;
        public bool threePview = false;
        public bool isConver = false;



        float tt = 0;

        //Yarn.Unity.DialogueRunner dialogueRun;
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            modelTransform = transform.GetChild(0);
            animator = GetComponent<Animator>();
            //cameraTransform = Camera.main.transform;
            cameraPivotTransform = cameraTransform.parent;
            //animatorFirst = GameObject.Find("FirstPArms").GetComponent<Animator>();

        }

        void Start()
        {
            //dialogueRun = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        }

        private void Update()   //ĳ���� ���� �� ��Ʈ�� �ݿ��� ���⼭ ����
        {
            GetInput();
            if (Time.timeScale < 0.001f) return;         //�Ͻ����� �� �ð��� ���� ���¿��� �Է� ����


            FreezeRotationXZ();     //CharacterController ĸ���� � �����ε� �������� �ʵ��� ����


            switch (playerState)
            {
                case PlayerState.IDLE:
                    PlayerIdle();
                    break;
                
                case PlayerState.UNDER_ATTACK:
                    PlayerUnderAttack();
                    break;
                case PlayerState.DEAD:
                    PlayerDead();
                    break;
                default:
                    break;
            }
        }
        void GetInput()
        {            
            jDown = Input.GetButtonDown("Jump");
        }



        private void LateUpdate()       //���� ī�޶� ������ ���⼭ ����
        {
            if (Time.timeScale < 0.001f) return;         //�Ͻ����� �� �ð��� ���� ���¿��� �Է� ����

            float cameraHeight = 1.3f;
       

            mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y")  /*PreferenceData.MouseSensitivity*/ * 0.5f, Input.GetAxisRaw("Mouse X") */* PreferenceData.MouseSensitivity * */0.5f, 0);   //���콺�� �������� ����
            if (mouseMove.x < -60)  //���� ������ ������ �д�.
                mouseMove.x = -60;
            else if (60 < mouseMove.x)
                mouseMove.x = 60;

            cameraPivotTransform.localEulerAngles = mouseMove;


            if (threePview == true)
            {
                cameraPivotTransform.position = transform.position + Vector3.up * cameraHeight;  //ĳ������ ���� ������ 




                RaycastHit cameraWallHit;   //ī�޶� �� �ڷ� ���� ȭ���� �������� ���� ����
                if (Physics.Raycast(cameraPivotTransform.position, cameraTransform.position - cameraPivotTransform.position, out cameraWallHit, cameraDistance, ~(1 << LayerMask.NameToLayer("Player"))))       //�÷��̾��� �ݶ��̴��� ������ �ʵ���
                    cameraTransform.localPosition = Vector3.back * cameraWallHit.distance;
                else
                    cameraTransform.localPosition = Vector3.back * cameraDistance + Vector3.right * 0.5f; // + ������ 1f��ŭ
            }
            else
            {
                cameraPivotTransform.position = GameObject.FindWithTag("Eye").transform.position;
                cameraTransform.localPosition = Vector3.zero;
            }
        }


        private void PlayerIdle()
        {
            RunCheck();             //�޸��� ���� üũ
            CrouchCheck();
            RaycastHit groundHit;
            if (GroundCheck(out groundHit))    //���鿡 ���� ����ִ� ���
            {
                
                if (isGroundedCheck == false)
                    isGroundedCheck = true;
                animator.SetBool("isGrounded", true);
                CalcInputMove();        //�̵� �Է� ���. �������� ��Ʈ�� ����
                //RaycastHit groundHit;
                //if (GroundCheck(out groundHit))  //������ Raycast�� ��� ���� �� �� �� Ȯ��. 
                    //moveVelocity.y = IsRun ? -runSpeed : -walkSpeed;    //isGounded�� ���� �����Ӷ� velocity.y ��ŭ �������� �ٴڿ� ���� ������ false�� �����Ѵ�. �������� �ӵ��� ����ؼ� y ���� �־�� ���ο����� isGrounded ���� true�� �ȴ�.
                //else
                    //moveVelocity.y = -1;    //Raycast�� ĸ���� �߾ӿ��� ��⿡ �𼭸��� ��ġ�� Raycast�� false�̳� isGrounded�� true�� ��찡 �߻��Ѵ�. ���� ���� ������ �������� �߻��ϹǷ� y���� �ּ�ȭ �Ͽ� �ڿ������� ���������� �Ѵ�.

                if (jDown)
                {
                    moveVelocity.y = 5f;
                    isGroundedCheck = false;
                    animator.SetTrigger("doJump");

                }
                tt = 0;
            }
            else
            {
                tt += Time.deltaTime;
                if (tt >= 1f)
                {
                    CalcInputMove();
                    tt = 0;
                }
                if (isGroundedCheck)
                    isGroundedCheck = false;
                else
                    animator.SetBool("isGrounded", false);
                moveVelocity += Physics.gravity * Time.deltaTime;   //�߷� ����
            }
            

            
            characterController.Move(moveVelocity * Time.deltaTime);//���������� CharacterController Move ȣ��
        }


        private void PlayerUnderAttack()
        {

        }

        private void PlayerDead()
        {
        }

        private void FreezeRotationXZ()
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);   //������ ����
        }



        private void RunCheck()
        {
            if (IsRun == false && Input.GetKeyDown(KeyCode.LeftShift))  //���� ����Ʈ�� ������ �޸��� ����
                IsRun = true;
            //if (IsRun && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)  //�̵� �Է��� ������ �޸��� ���
                //IsRun = false;
            if (IsRun && Input.GetKeyUp(KeyCode.LeftShift))  //�̵� �Է��� ������ �޸��� ���
                IsRun = false;
        }


        private void CrouchCheck()
        {
            if (IsCrouch == false && Input.GetButtonDown("Crouch")) //c�� ������ �޸��� ����
            {
                IsCrouch = true;
            }
            else if (IsCrouch && Input.GetButtonDown("Crouch")) //�̵� �Է��� ������ ��ũ���� ���
            {
                IsCrouch = false;
            }
        }

        private bool GroundCheck(out RaycastHit hit)
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

        private void CalcInputMove()
        {
            //���� ������ ���۰��� ����߸��ٰ� �����Ͽ� GetAxisRaw�� ����Ͽ� ���� ���� ����. normalized�� ����Ͽ� �밢�� �̵� �� ������ ���̰� �� 1.41�� �Ǵ� �κ� ����
            moveVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * (IsRun ? runSpeed : walkSpeed);
            animator.SetFloat("speedX", Input.GetAxis("Horizontal"));   //����� GetAxis�� ��� �ڿ�������Ƿ� GetAxis �� ���
            animator.SetFloat("speedY", Input.GetAxis("Vertical"));
            //animatorFirst.SetFloat("walk", Mathf.Max(Mathf.Abs(Input.GetAxis("Vertical")), Mathf.Abs(Input.GetAxis("Horizontal"))));
            //animatorFirst.SetFloat("speedX",Input.GetAxis("Horizontal"));
            moveVelocity = transform.TransformDirection(moveVelocity);    //�Է� Ű�� ī�޶� ���� �ִ� �������� ����

            //���� �߿��� ī�޶��� ���⿡ ��������� ĳ���Ͱ� �����̵��� �Ѵ�.
            if (0.01f < moveVelocity.sqrMagnitude)
            {
                Quaternion cameraRotation = cameraPivotTransform.rotation;
                cameraRotation.x = cameraRotation.z = 0;    //y�ุ �ʿ��ϹǷ� ������ ���� 0���� �ٲ۴�.
                transform.rotation = cameraRotation;
                if (IsRun)
                {
                    animatorFirst.SetBool("isRun",true);
                    //�޸��� ���¿��� �̵� �������� ���� ������.
                    Quaternion characterRotation = Quaternion.LookRotation(moveVelocity);
                    characterRotation.x = characterRotation.z = 0;
                    //�� ȸ���� �ڿ��������� ���� Slerp�� ���
                    modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, characterRotation, 10.0f * Time.deltaTime);
                    //StatusManager.instance.stamina_state = true;
                }
                else
                {
                    animatorFirst.SetBool("isRun", false);
                    //��� ���¿��� ������ ������ä �����δ�.
                    modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, cameraRotation, 10.0f * Time.deltaTime);
                    //StatusManager.instance.stamina_state = false;
                }
            }
        }


    }
}