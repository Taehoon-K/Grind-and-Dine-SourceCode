using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMinigame : Kupa.Player
{


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        modelTransform = transform.GetChild(0);
        animator = GetComponent<Animator>();
        cameraPivotTransform = cameraTransform.parent;

    }

    void Start()
    {
        //Audio Source Setup.
        footstepAudioSource = GetComponent<AudioSource>();

        sensitivity = PlayerPrefs.GetFloat("MouseSensSlider", 50f) / 20 + 0.1f;

        mouseMove = NormalizeAngles(cameraPivotTransform.localEulerAngles); //마우스 시점 미리
                                                                            //카메라 위치로 고정

        //get defaults
        defaultHeight = characterController.height;

        defalutController = characterController.center;
    }
    private void Update()   //캐릭터 조정 및 컨트롤 반영은 여기서 진행
    {
        if (Time.timeScale < 0.001f) return;         //일시정지 등 시간을 멈춘 상태에선 입력 방지


        IsRun = holdingButtonRun && CanRun();

        FreezeRotationXZ();     //CharacterController 캡슐이 어떤 이유로든 기울어지지 않도록 방지

        NewMovement();
       // characterController.Move(moveVelocity * Time.deltaTime);//최종적으로 CharacterController Move 호출
    }
    private void FixedUpdate()
    {
        //PlayerIdle();
        ProgressStepCycle();
    }
    private void LateUpdate()       //최종 카메라 보정은 여기서 진행
    {
        if (Time.timeScale < 0.001f) return;         //일시정지 등 시간을 멈춘 상태에선 입력 방지

        mouseMove += new Vector3(-axisLook.y * sensitivity, axisLook.x * sensitivity, 0); //마우스의 움직임을 가감
                                                                                          // Clamp the vertical camera movement
        mouseMove.x = Mathf.Clamp(mouseMove.x, -60f, 60f);

        cameraPivotTransform.localEulerAngles = mouseMove;


        
        cameraPivotTransform.position = GameObject.FindWithTag("Eye").transform.position;
        cameraTransform.localPosition = Vector3.zero;
    }

    private bool CanRun()
    {
        //This blocks running backwards, or while fully moving sideways.
        if (axisMovement.y <= 0 || Math.Abs(Mathf.Abs(axisMovement.x) - 1) < 0.01f)
            return false;

        //Return.
        return true;
    }
    /*
    protected override void CalcInputMove()
    {
        float speed;
        if (IsRun)
        {
            speed = runSpeed;
        }
        else if (IsCrouch)
        {
            speed = crouchSpeed;
        }
        else
            speed = walkSpeed;
        Vector2 frameInput = axisMovement;
        //가속 과정이 조작감을 떨어뜨린다고 생각하여 GetAxisRaw를 사용하여 가속 과정 생략. normalized를 사용하여 대각선 이동 시 벡터의 길이가 약 1.41배 되는 부분 보정
        moveVelocity = new Vector3(frameInput.x, 0, frameInput.y).normalized * (speed);
        animator.SetFloat("speedX", frameInput.x);   //모션은 GetAxis을 써야 자연스러우므로 GetAxis 값 사용
        animator.SetFloat("speedY", frameInput.y);

        moveVelocity = transform.TransformDirection(moveVelocity);    //입력 키를 카메라가 보고 있는 방향으로 조정

        //조작 중에만 카메라의 방향에 상대적으로 캐릭터가 움직이도록 한다.
        if (0.01f < moveVelocity.sqrMagnitude)
        {
            Quaternion cameraRotation = cameraPivotTransform.rotation;
            cameraRotation.x = cameraRotation.z = 0;    //y축만 필요하므로 나머지 값은 0으로 바꾼다.
            transform.rotation = cameraRotation;
            if (IsRun)
            {
                animatorFirst.SetBool("isRun", true);
                //달리기 상태에선 이동 방향으로 몸을 돌린다.
                Quaternion characterRotation = Quaternion.LookRotation(moveVelocity);
                characterRotation.x = characterRotation.z = 0;
                //모델 회전은 자연스러움을 위해 Slerp를 사용
                modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, characterRotation, 10.0f * Time.deltaTime);
            }
            else
            {
                animatorFirst.SetBool("isRun", false);
                //통상 상태에선 정면을 유지한채 움직인다.
                modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, cameraRotation, 10.0f * Time.deltaTime);
            }

            //Select the correct audio clip to play.
            audioSource.clip = IsRun ? audioClipRunning : audioClipWalking;
            //Play it!
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }*/

    public override void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!IsCrouch)
            {
                jumpRequest = true; // 점프 요청 설정        
            }
        }
    }
    private Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = (angles.x > 180) ? angles.x - 360 : angles.x;
        angles.y = (angles.y > 180) ? angles.y - 360 : angles.y;
        angles.z = (angles.z > 180) ? angles.z - 360 : angles.z;
        return angles;
    }

}
