using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechAction : CutsceneAction
{
    public string yarnString;
    public override void Execute()
    {
        FindObjectOfType<PlayerYarn>().StartCutsceneSpeech(yarnString, onExecutionComplete); //���̾�α� ����
        //Complete();
    }
}
