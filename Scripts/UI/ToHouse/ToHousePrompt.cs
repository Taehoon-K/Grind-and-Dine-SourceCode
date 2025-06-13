using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class ToHousePrompt : UiPrompt
{
    [SerializeField]
    private GameObject emptyWarning; //갈 수 있는 곳 없다는 경고문
    [SerializeField]
    private GameObject whichFloor; //몇층 갈건지 text
    [SerializeField]
    private Transform contentGrid; //층 선택 버튼들

    public GameObject prefabListing;
    string RELATIONSHIP_PREFIX = "NPCRealtionship_";
    SceneTransitionManager.Location location;

    public void RenderShop(List<string> npcLists)
    {
        List<string> listNpc = null;
        //원래 있던거 리셋
        if (contentGrid.childCount > 0)
        {
            foreach (Transform child in contentGrid)
            {
                Destroy(child.gameObject);
            }
        }


        listNpc = CheckEssential(npcLists); //조건 충족하는 리스트만 담기    
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

    private List<string> CheckEssential(List<string> allNpcs) //권한 체크하는 함수
    {
        List<string> newNpcs = new List<string>(); //호감도랑 집에 위치해서 통과한 npc들 담을 리스트
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();


        foreach (string npc in allNpcs)
        {
            if(GetRelationshipAndLocation(npc)) //만약 조건 충족할시
            {
                newNpcs.Add(npc);
            }
        }
        return newNpcs;
    }

    private bool GetRelationshipAndLocation(string npcName) //호감도 리턴하는 함수
    {
        int relationPoint;
        string currentLocaiton;
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
        if (!blackboard.ContainsKey(RELATIONSHIP_PREFIX + npcName)) //만약 키가 없다면 리턴
        {
            return false; 
        }
        blackboard.TryGetValue(RELATIONSHIP_PREFIX + npcName, out NPCRelationshipState relationship);
        relationPoint = relationship.friendshipPoints; //호감도 저장

        currentLocaiton = NpcManager.Instance.GetNPCLocation(npcName).location.ToString(); //npc 위치 불러오기
        location = NpcManager.Instance.GetNPCLocation(npcName).location;
        if (relationPoint >= 1000) //호감도 4칸 이상이고 npc 현재 위치가 자기 집일경우
        {
            if (NpcManager.Instance.HasParent(npcName)) //만약 부모가 있다면
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

    public void ExitPrompt() //X버튼 클릭시 나가는용도
    {
        //UIManager.instance.CurrentUIState = UIState.None;
    }

}
