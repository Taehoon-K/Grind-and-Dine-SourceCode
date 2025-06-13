using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechAction : CutsceneAction
{
    public string yarnString;
    public override void Execute()
    {
        FindObjectOfType<PlayerYarn>().StartCutsceneSpeech(yarnString, onExecutionComplete); //다이얼로그 시작
        //Complete();
    }
}
