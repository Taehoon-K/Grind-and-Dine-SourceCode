using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorInternalButton : MonoBehaviour
{
    [SerializeField]
    private Elevator elevator;
    public void PickUp()
    {
        
        UIManager.instance.TriggerElevatorPrompt(elevator.currentFloor);

    }
}
