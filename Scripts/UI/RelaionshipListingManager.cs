using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RelaionshipListingManager : MonoBehaviour
{
    //인스턴스할 샵 리스팅
    public GameObject npcListing;

    public Transform listingGrid;

    [SerializeField]
    private List<NPC> npcs;


    public void RenderRelation()
    {
        List<NPCRelationshipState> relationship = RelationshipStats.relationships;
        if (npcs.Count == 0)
        {
            LoadAllCharacter();
        }


        
      //  npcListing.GetComponent<RelationshipListing>().Display(npcData, relatoinship);

        //원래 있던거 리셋
        if (listingGrid.childCount > 0)
        {
            foreach (Transform child in listingGrid)
            {
                Destroy(child.gameObject);
            }
        }

        // 1. 관계 정보가 있는 NPC 먼저 처리
        HashSet<int> usedNpcCodes = new HashSet<int>();

        foreach (NPCRelationshipState shopItem in relationship)
        {
            NPC npcData = GetNpcDataFromCode(shopItem.npcCode);
            if (npcData == null) continue;

            GameObject listingGameObject = Instantiate(npcListing, listingGrid);
            listingGameObject.GetComponent<RelationshipListing>().Display(npcData, shopItem);

            usedNpcCodes.Add(shopItem.npcCode);
        }

        // 2. 관계 정보가 없는 NPC도 처리
        foreach (NPC npc in npcs)
        {
            if (usedNpcCodes.Contains(npc.GetNpcCode()))
                continue; // 이미 처리한 NPC는 스킵

            GameObject listingGameObject = Instantiate(npcListing, listingGrid);
            listingGameObject.GetComponent<RelationshipListing>().Display(npc, null); // 관계 없음
        }
    }

    public NPC GetNpcDataFromCode(int code)
    {
        return npcs.Find(i => i.GetNpcCode() == code); //코드로 리소스에서 npc데이터 찾기
    }
    private void LoadAllCharacter()
    {
        npcs = NpcManager.Instance.Characters();
    }
}
