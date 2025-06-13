using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using FIMSpace.FLook;
using UnityEngine;
using UnityEngine.Localization;

public class InteractableExtra : MonoBehaviour
{
    public LocalizedString nameKey;
    public string simpleTalk;
    public string simpleGift;

    FLookAnimator lookAni;
    void Start()
    {
        lookAni = GetComponent<FLookAnimator>();

        GameObject eyeObject = GameObject.FindGameObjectWithTag("Eye");
        if (eyeObject != null&& lookAni != null)
        {
            lookAni.ObjectToFollow = eyeObject.transform;
        }
    }
}
