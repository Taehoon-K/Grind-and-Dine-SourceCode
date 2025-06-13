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

        mouseMove = NormalizeAngles(cameraPivotTransform.localEulerAngles); //���콺 ���� �̸�
                                                                            //ī�޶� ��ġ�� ����

        //get defaults
        defaultHeight = characterController.height;

        defalutController = characterController.center;
    }
    private void Update()   //ĳ���� ���� �� ��Ʈ�� �ݿ��� ���⼭ ����
    {
        if (Time.timeScale < 0.001f) return;         //�Ͻ����� �� �ð��� ���� ���¿��� �Է� ����


        IsRun = holdingButtonRun && CanRun();

        FreezeRotationXZ();     //CharacterController ĸ���� � �����ε� �������� �ʵ��� ����

        NewMovement();
       // characterController.Move(moveVelocity * Time.deltaTime);//���������� CharacterController Move ȣ��
    }
    private void FixedUpdate()
    {
        //PlayerIdle();
        ProgressStepCycle();
    }
    private void LateUpdate()       //���� ī�޶� ������ ���⼭ ����
    {
        if (Time.timeScale < 0.001f) return;         //�Ͻ����� �� �ð��� ���� ���¿��� �Է� ����

        mouseMove += new Vector3(-axisLook.y * sensitivity, axisLook.x * sensitivity, 0); //���콺�� �������� ����
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
        //���� ������ ���۰��� ����߸��ٰ� �����Ͽ� GetAxisRaw�� ����Ͽ� ���� ���� ����. normalized�� ����Ͽ� �밢�� �̵� �� ������ ���̰� �� 1.41�� �Ǵ� �κ� ����
        moveVelocity = new Vector3(frameInput.x, 0, frameInput.y).normalized * (speed);
        animator.SetFloat("speedX", frameInput.x);   //����� GetAxis�� ��� �ڿ�������Ƿ� GetAxis �� ���
        animator.SetFloat("speedY", frameInput.y);

        moveVelocity = transform.TransformDirection(moveVelocity);    //�Է� Ű�� ī�޶� ���� �ִ� �������� ����

        //���� �߿��� ī�޶��� ���⿡ ��������� ĳ���Ͱ� �����̵��� �Ѵ�.
        if (0.01f < moveVelocity.sqrMagnitude)
        {
            Quaternion cameraRotation = cameraPivotTransform.rotation;
            cameraRotation.x = cameraRotation.z = 0;    //y�ุ �ʿ��ϹǷ� ������ ���� 0���� �ٲ۴�.
            transform.rotation = cameraRotation;
            if (IsRun)
            {
                animatorFirst.SetBool("isRun", true);
                //�޸��� ���¿��� �̵� �������� ���� ������.
                Quaternion characterRotation = Quaternion.LookRotation(moveVelocity);
                characterRotation.x = characterRotation.z = 0;
                //�� ȸ���� �ڿ��������� ���� Slerp�� ���
                modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, characterRotation, 10.0f * Time.deltaTime);
            }
            else
            {
                animatorFirst.SetBool("isRun", false);
                //��� ���¿��� ������ ������ä �����δ�.
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
                jumpRequest = true; // ���� ��û ����        
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
