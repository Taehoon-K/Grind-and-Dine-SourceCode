using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoCollection;

[CreateAssetMenu(fileName = "Cutscene", menuName ="Cutscene/Cutscene")]
public class Cutscene : ScriptableObject,IConditional
{
    public BlackboardCondition[] conditions;
    //Whether this event cutscene can play again once it has occurred
    public bool recurring;

    public SoCollection<CutsceneAction> action;

    //check if the condition is met
    public bool CheckConditions(out int score)
    {
        IConditional conditional = this;
        return conditional.CheckConditions(conditions, out score);
    }
}
