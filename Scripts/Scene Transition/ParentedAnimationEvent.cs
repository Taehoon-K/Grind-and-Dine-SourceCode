using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentedAnimationEvent : MonoBehaviour
{
    public void NotifyAncestors(string message)
    {
        SendMessageUpwards(message);
    }
}