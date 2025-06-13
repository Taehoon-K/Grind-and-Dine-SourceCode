using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class ToHousePrompt : UiPrompt
{
    [SerializeField]
    private GameObject emptyWarning; //�� �� �ִ� �� ���ٴ� ���
    [SerializeField]
    private GameObject whichFloor; //���� ������ text
    [SerializeField]
    private Transform contentGrid; //�� ���� ��ư��

    public GameObject prefabListing;
    string RELATIONSHIP_PREFIX = "NPCRealtionship_";
    SceneTransitionManager.Location location;

    public void RenderShop(List<string> npcLists)
    {
        List<string> listNpc = null;
        //���� �ִ��� ����
        if (contentGrid.childCount > 0)
        {
            foreach (Transform child in contentGrid)
            {
                Destroy(child.gameObject);
            }
        }


        listNpc = CheckEssential(npcLists); //���� �����ϴ� ����Ʈ�� ���    
        if(listNpc.Count == 0)
        {
            emptyWarning.SetActive(true);
            whichFloor.SetActive(false);
            contentGrid.gameObject.SetActive(false);
        }
        else
        {
            emptyWarning.SetActive(false);
            whichFloor.SetActive(true);
            contentGrid.gameObject.SetActive(true);
            foreach (string item in listNpc)
            {
                GameObject listingGameObject = Instantiate(prefabListing, contentGrid);

                listingGameObject.GetComponent<ToHouseListing>().Display(item, location);
            }
        }

    }

    private List<string> CheckEssential(List<string> allNpcs) //���� üũ�ϴ� �Լ�
    {
        List<string> newNpcs = new List<string>(); //ȣ������ ���� ��ġ�ؼ� ����� npc�� ���� ����Ʈ
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();


        foreach (string npc in allNpcs)
        {
            if(GetRelationshipAndLocation(npc)) //���� ���� �����ҽ�
            {
                newNpcs.Add(npc);
            }
        }
        return newNpcs;
    }

    private bool GetRelationshipAndLocation(string npcName) //ȣ���� �����ϴ� �Լ�
    {
        int relationPoint;
        string currentLocaiton;
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
        if (!blackboard.ContainsKey(RELATIONSHIP_PREFIX + npcName)) //���� Ű�� ���ٸ� ����
        {
            return false; 
        }
        blackboard.TryGetValue(RELATIONSHIP_PREFIX + npcName, out NPCRelationshipState relationship);
        relationPoint = relationship.friendshipPoints; //ȣ���� ����

        currentLocaiton = NpcManager.Instance.GetNPCLocation(npcName).location.ToString(); //npc ��ġ �ҷ�����
        location = NpcManager.Instance.GetNPCLocation(npcName).location;
        if (relationPoint >= 1000) //ȣ���� 4ĭ �̻��̰� npc ���� ��ġ�� �ڱ� ���ϰ��
        {
            if (NpcManager.Instance.HasParent(npcName)) //���� �θ� �ִٸ�
            {
                if (NpcManager.Instance.GetParent(npcName) + "H" == currentLocaiton)
                {
                    return true;
                }
                else
                {
                    Debug.Log(NpcManager.Instance.GetParent(npcName) + "H");
                    return false;
                }
            }
            else
            {
                if(npcName + "H" == currentLocaiton)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } 
        }
        else
        {
            return false;
        }

    }

    public void ExitPrompt() //X��ư Ŭ���� �����¿뵵
    {
        //UIManager.instance.CurrentUIState = UIState.None;
    }

}
