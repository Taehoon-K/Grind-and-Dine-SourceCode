using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToHouseButton : MonoBehaviour
{
    [SerializeField]
    private List<string> npcName;
    public void SwitchScene() //��ȭ���̳� Ŭ�� ������ ���� �� ���
    {
        UIManager.instance.TriggerHousePrompt(npcName);
    }

    
}
