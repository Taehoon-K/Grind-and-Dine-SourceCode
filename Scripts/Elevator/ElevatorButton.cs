using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    [SerializeField]
    private Elevator elevator;
    [SerializeField]
    private int floor; // �� ��ġ
    public void PickUp()
    {
        elevator.CallElevator(floor);

    }
}
