using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAction : CutsceneAction
{
    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public float duration;

    public override void Execute()
    {
        CutsceneCamManager.Instance.MoveCamera(targetPosition, targetRotation, duration, onExecutionComplete);
    }
}
