using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;



public class CrafterController : MonoBehaviour
{
    [SerializeField] private bool isSwitchZ;
    private Animator animator;
    
    float rotationSpeed = 5;
    Vector3 inputVec;
    public bool isMoving;
    public bool isPaused;
    Vector2 axisMovement;
    //public CharacterState charState;
    bool runButton;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (isPaused) //퍼즈 시 이동 아예 방지
        {
            animator.SetBool("Moving", false);
            animator.SetBool("Running", false);
            isMoving = false;
            animator.SetFloat("Velocity", 0f);
            return;
        }

        float x = axisMovement.x;
        float z = axisMovement.y;
        if (!isSwitchZ)
        {
            inputVec = new Vector3(x, 0, z);
            animator.SetFloat("VelocityX", -x);
            animator.SetFloat("VelocityY", z);
        }
        else
        {
            inputVec = new Vector3(-z, 0, x);
            animator.SetFloat("VelocityX", z);
            animator.SetFloat("VelocityY", x);
        }
        

        //if there is some input
        if (x != 0 || z != 0)
        {
            //set that character is moving
            animator.SetBool("Moving", true);
            isMoving = true;

            //if we are running, set the animator
            if (runButton) //손에 벽돌없을때만
            {
                if(BrickMoveManager.instance != null ) //만약 벽돌옮기기 미니게임이라면
                {
                    if(BrickMoveManager.instance.BricksCount == 0)
                    {
                        animator.SetBool("Running", true);
                    }
                }
                else
                {
                    animator.SetBool("Running", true);
                }
                
            }
            else
            {
                animator.SetBool("Running", false);
            }
        }
        else
        {
            //character is not moving
            animator.SetBool("Moving", false);
            isMoving = false;
        }

        //update character position and facing
        UpdateMovement();


        //sent velocity to animator
        animator.SetFloat("Velocity", UpdateMovement());
    }

    //face character along input direction
    void RotateTowardsMovementDir()
    {
        if (!isPaused)
        {
            if (inputVec != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed);
            }
        }
    }

    //movement of character
    float UpdateMovement()
    {
        //get movement input from controls
        Vector3 motion = inputVec;

        //reduce input for diagonal movement
        motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1) ? 0.7f : 1;

        if (!isPaused)
        {
            //if not paused, face character along input direction
            RotateTowardsMovementDir();
        }

        return inputVec.magnitude;
    }
    /*
    void OnGUI()
    {
        if (!isMoving)
        {
            isPaused = false;
            if (GUI.Button(new Rect(25, 25, 150, 30), "Pickup Box"))
            {
                animator.SetTrigger("CarryPickupTrigger");
                StartCoroutine(COMovePause(1.2f));
                StartCoroutine(COShowItem("box", .5f));

            }
            if (GUI.Button(new Rect(25, 65, 150, 30), "Recieve Box"))
            {
                animator.SetTrigger("CarryRecieveTrigger");
                StartCoroutine(COMovePause(1.2f));
                StartCoroutine(COShowItem("box", .5f));

            }
        }
        if (!isMoving)
        {
            if (GUI.Button(new Rect(25, 25, 150, 30), "Put Down Box"))
            {
                animator.SetTrigger("CarryPutdownTrigger");
                StartCoroutine(COMovePause(1.2f));
                StartCoroutine(COShowItem("none", .7f));

            }
            if (GUI.Button(new Rect(25, 65, 150, 30), "Give Box"))
            {
                animator.SetTrigger("CarryHandoffTrigger");
                StartCoroutine(COMovePause(1.2f));
                StartCoroutine(COShowItem("none", .6f));

            }
        }
    }*/

    public IEnumerator COMovePause(float pauseTime)
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseTime);
        isPaused = false;
    }
    /*
    public IEnumerator COShowItem(string item, float waittime)
    {
        yield return new WaitForSeconds(waittime);

        if (item == "none")
        {
            box.SetActive(false);
        }
        else if (item == "box")
        {
            box.SetActive(true);
        }

        yield return null;
    }*/


    public void OnMove(InputAction.CallbackContext context)
    {
        //Read.
        axisMovement = context.ReadValue<Vector2>();
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Start.
                runButton = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                runButton = false;
                break;
        }
    }
}