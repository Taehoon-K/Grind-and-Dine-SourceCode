using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RelaionshipListingManager : MonoBehaviour
{
    //�ν��Ͻ��� �� ������
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

        //���� �ִ��� ����
        if (listingGrid.childCount > 0)
        {
            foreach (Transform child in listingGrid)
            {
                Destroy(child.gameObject);
            }
        }

        // 1. ���� ������ �ִ� NPC ���� ó��
        HashSet<int> usedNpcCodes = new HashSet<int>();

        foreach (NPCRelationshipState shopItem in relationship)
        {
            NPC npcData = GetNpcDataFromCode(shopItem.npcCode);
            if (npcData == null) continue;

            GameObject listingGameObject = Instantiate(npcListing, listingGrid);
            listingGameObject.GetComponent<RelationshipListing>().Display(npcData, shopItem);

            usedNpcCodes.Add(shopItem.npcCode);
        }

        // 2. ���� ������ ���� NPC�� ó��
        foreach (NPC npc in npcs)
        {
            if (usedNpcCodes.Contains(npc.GetNpcCode()))
                continue; // �̹� ó���� NPC�� ��ŵ

            GameObject listingGameObject = Instantiate(npcListing, listingGrid);
            listingGameObject.GetComponent<RelationshipListing>().Display(npc, null); // ���� ����
        }
    }

    public NPC GetNpcDataFromCode(int code)
    {
        return npcs.Find(i => i.GetNpcCode() == code); //�ڵ�� ���ҽ����� npc������ ã��
    }
    private void LoadAllCharacter()
    {
        npcs = NpcManager.Instance.Characters();
    }
}
