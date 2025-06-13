using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private GameObject inside;
    [SerializeField]
    private Transform firstFloor; // 1층 위치
    [SerializeField]
    private Transform secondFloor; // 2층 위치
    public float speed = 2.0f; // 이동 속도
    private Animator doorAnimator; // 문 애니메이터
    public float doorOpenTime = 5f; // 문이 열려 있는 시간

    private bool isMoving = false; // 엘리베이터가 움직이고 있는지 여부
    private bool doorOpen = false; // 문이 열려 있는지 여부
    private Transform targetFloor; // 목표 층
    [HideInInspector]
    public int currentFloor; //현재 층수, 1층에서 시작

    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        currentFloor = 1;
    }
    void Update()
    {
        if (isMoving)
        {
            MoveElevator();
        }
    }

    public void CallElevator(int a) //엘레베이터 부르기
    {
        currentFloor = a;
        Transform aa = intToFloor(a);
        if (!isMoving && !doorOpen)
        {
            targetFloor = aa;
            isMoving = true;
        }
    }

    private void MoveElevator()
    {
        inside.transform.position = Vector3.MoveTowards(inside.transform.position, targetFloor.position, speed * Time.deltaTime);

        if (inside.transform.position == targetFloor.position)
        {
            isMoving = false;
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        doorAnimator.SetBool("isOpen", true);
        doorOpen = true;
        Invoke("CloseDoor", doorOpenTime);
    }

    public void CloseDoor()
    {
        doorAnimator.SetBool("isOpen", false);
        doorOpen = false;
    }

    private void CloseDoorAuto()
    {
        //만약 안에 사람 없으면 자동 닫힘

        CloseDoor();
    }

    public void GoToFloor(int floor)
    {
        currentFloor = floor;
        Transform aa = intToFloor(floor);
        if (!isMoving)
        {
            targetFloor = aa;
            isMoving = true;
            CloseDoor();
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private Transform intToFloor(int floor)
    {
        switch (floor)
        {
            case 1:
                return firstFloor;
            case 2:
                return secondFloor;
            default:
                return firstFloor;
        }
    }
}
