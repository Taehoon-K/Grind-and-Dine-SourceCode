using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToHouseButton : MonoBehaviour
{
    [SerializeField]
    private List<string> npcName;
    public void SwitchScene() //대화문이나 클릭 등으로 진입 시 사용
    {
        UIManager.instance.TriggerHousePrompt(npcName);
    }

    
}
