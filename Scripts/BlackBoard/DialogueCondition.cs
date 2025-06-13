using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogueCondition : IConditional
{
    public string id;
    public BlackboardCondition[] conditions;
    public string yarnString;

    public bool CheckConditions(out int conditionsMet)
    {
        /*conditionsMet = 0; //조건 많은 것부터 우선순위\
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();

        foreach(BlackboardCondition condition in conditions)
        {
            if (!blackboard.CompareValue(condition))
            {
                return false;
            }
            conditionsMet++;
        }
        return true;*/
        IConditional conditionChecker = this;
        return conditionChecker.CheckConditions(conditions, out conditionsMet);
    }
}
