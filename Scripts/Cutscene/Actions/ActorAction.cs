using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAction : CutsceneAction
{
    public enum ActionType { Move, Remove }
    public ActionType actionType;

    public string actorName;//npc�̸�
    public Vector3 position;
    public Quaternion rotation;

    public override void Execute()
    {
        Debug.Log("executing actor action");
        //���� �׼� Ÿ���� remove���
        if(actionType == ActionType.Remove)
        {
            Remove();
            onExecutionComplete?.Invoke();
            return;
        }

        CreateOrMove();
    }

    private void CreateOrMove()
    {
        CutsceneManager.Instance.AddOrMoveActor(actorName, position,rotation, onExecutionComplete);
    }
    private void Remove()
    {
        CutsceneManager.Instance.KillActor(actorName);
        onExecutionComplete?.Invoke();
    }
}
