using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private GameObject inside;
    [SerializeField]
    private Transform firstFloor; // 1�� ��ġ
    [SerializeField]
    private Transform secondFloor; // 2�� ��ġ
    public float speed = 2.0f; // �̵� �ӵ�
    private Animator doorAnimator; // �� �ִϸ�����
    public float doorOpenTime = 5f; // ���� ���� �ִ� �ð�

    private bool isMoving = false; // ���������Ͱ� �����̰� �ִ��� ����
    private bool doorOpen = false; // ���� ���� �ִ��� ����
    private Transform targetFloor; // ��ǥ ��
    [HideInInspector]
    public int currentFloor; //���� ����, 1������ ����

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

    public void CallElevator(int a) //���������� �θ���
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
        //���� �ȿ� ��� ������ �ڵ� ����

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
