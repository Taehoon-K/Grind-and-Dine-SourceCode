using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepBed : MonoBehaviour
{
    [SerializeField] private bool isJail;
    public void PickUp()
    {
        UIManager.instance.TriggerSleepPrompt(isJail);
    }
}
