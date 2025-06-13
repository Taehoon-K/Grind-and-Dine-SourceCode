using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLotationAction : CutsceneAction
{
    public Quaternion targetRotation;
    public float duration;

    public override void Execute()
    {
        CutsceneCamManager.Instance.LotationCamera(targetRotation, duration, onExecutionComplete);
    }
}
