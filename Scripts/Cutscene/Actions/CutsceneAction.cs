using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CutsceneAction:ScriptableObject
{
    protected Action onExecutionComplete;

    public void Init(Action onExecutionComplete)
    {
        this.onExecutionComplete = onExecutionComplete;
        Execute();
    }
    public abstract void Execute();

    /*public void Complete()
    {
        onExecutionComplete?.Invoke();
    }*/

}
