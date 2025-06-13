using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP.InventorySystem;
using System.Linq;

public class RelationshipStats : MonoBehaviour
{
    const string RELATIONSHIP_PREFIX = "NPCRealtionship_";
    public static List<NPCRelationshipState> relationships = new List<NPCRelationshipState>();

    public enum GiftReaction
    {
        Best,Good,Soso,Bad,Worst,Impossible,Special
    }
    public static bool FirstMeeting(NPC npc) //ù�����̶�� true
    {
        //return !relationships.Exists(i => i.npcCode == npc.GetNpcCode());
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
        return !blackboard.ContainsKey(RELATIONSHIP_PREFIX + npc.CharacterName());
    }

    public static NPCRelationshipState GetRelationship(NPC npc)
    {
        if (FirstMeeting(npc)) return null;
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
        blackboard.TryGetValue(RELATIONSHIP_PREFIX + npc.CharacterName(), out NPCRelationshipState relationship);
        return relationship;



        //return relationships.Find(i => i.npcCode == npc.GetNpcCode());
    }
    public static int GetRelationshipPoint(NPC npc)
    {
        if (FirstMeeting(npc)) return 0;
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
        blackboard.TryGetValue(RELATIONSHIP_PREFIX + npc.CharacterName(), out NPCRelationshipState relationship);
        return relationship.friendshipPoints;



        //return relationships.Find(i => i.npcCode == npc.GetNpcCode());
    }

    public static List<NPCRelationshipState> GetRelationShip() //�����Ҷ� ���� �ҷ��ÿ뵵, ������ ������ ���� �Ⱦ�
    {
        return relationships;
    }

    //�ε��Ҷ� ���� �ε�
    public static void LoadStats()
    {
        relationships = new List<NPCRelationshipState>();
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
        foreach(NPC n in NpcManager.Instance.Characters())
        {
            Debug.Log(RELATIONSHIP_PREFIX + n.CharacterName());
            if(blackboard.TryGetValue(RELATIONSHIP_PREFIX+ n.CharacterName(), out NPCRelationshipState rs))
            {
                relationships.Add(rs);
            }
        }

        /*if(relationshipsToLoad == null)
        {
            relationships = new List<NPCRelationshipState>();
            return;
        }
        relationships = relationshipsToLoad;*/
    }

    //npc ����Ʈ�� �߰�,���� �߰��� �� ���� ���� ����. ���̵� ���� Ȯ�� ���߰� �ִ� ���� �� ���� �ڵ� ������
    public static void UnlockCharacter(NPC npc)
    {
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();

        // �̹� ����� ĳ���Ͷ�� �ߺ� ����
        if (!FirstMeeting(npc))
        {
            Debug.Log($"{npc.CharacterName()}��(��) �̹� ����� NPC�Դϴ�.");
            return;
        }

        NPCRelationshipState relationship;
        if (npc.mustNotV)
        {
            relationship = new NPCRelationshipState(npc.GetNpcCode());
            //relationships.Add(new NPCRelationshipState(npc.GetNpcCode()));
        }
        else
        {
            bool vill = Villain();
            relationship = new NPCRelationshipState(npc.GetNpcCode(),vill);

        }
        blackboard.SetValue(RELATIONSHIP_PREFIX + npc.CharacterName(), relationship);

        // �ߺ� ���� �� ����Ʈ�� �߰�
        if (!relationships.Any(r => r.npcCode == npc.GetNpcCode()))
        {
            relationships.Add(relationship);
        }
        else
        {
            Debug.Log($"{npc.CharacterName()}��(��) relationships�� �̹� �����մϴ�.");
        }

    }

    public static void UpdateLoveStatus(NPC npc, bool loved) //npc�� ��ʹ����� ���� �ٲٴ� �Լ�
    {
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();

        // NPC�� ������ relationships���� ã���ϴ�.
        NPCRelationshipState relationship = relationships.FirstOrDefault(r => r.npcCode == npc.GetNpcCode());

        // ���� ���谡 �����Ѵٸ�
        if (relationship != null)
        {
            // hasTalkedToday ���� ������Ʈ�մϴ�.
            relationship.loveWith = loved;

            // �����忡 ������Ʈ�� ���� ���¸� �ٽ� �����մϴ�.
            blackboard.SetValue(RELATIONSHIP_PREFIX + npc.CharacterName(), relationship);
        }
        else
        {
            Debug.LogWarning("NPC ���� ���°� �����ϴ�: " + npc.CharacterName());
        }
    }

    public static void AddFriendPoints(NPC npc, int points)
    {
        if (FirstMeeting(npc))
        {
            Debug.LogError("���� �ȸ�����");
            return;
        }

        var relationship = GetRelationship(npc);

        int point;
        point = points;
        if (npc.isWoman && StatusManager.instance.GetSelectedPerk(4, 0) == 1 && points > 0) //���ڰ� �� ����ٸ�
        {
            point =  Mathf.RoundToInt(point * 1.2f);
        }
        else if (!npc.isWoman && StatusManager.instance.GetSelectedPerk(4, 0) == 0 && points > 0) //�����̰� �� ����ٸ�
        {
            point = Mathf.RoundToInt(point * 1.2f);
        }
        GetRelationship(npc).friendshipPoints += point;

        if (!relationship.loveWith && relationship.friendshipPoints > 2000)   //����� ������ 2000 ���Ϸ�
        {
            relationship.friendshipPoints = 2000;
        }

        // friendshipPoints�� -2000�� 3000 ���̷� ����
        relationship.friendshipPoints = Mathf.Clamp(relationship.friendshipPoints, -2000, 3000);
    }

    //���� ó�� ��ȭ���� üũ�ϴ� �Լ�
    public static bool IsFirstConversationOfTheDay(NPC character)
    {
        //���� �ƿ� ó���̶��
        if (FirstMeeting(character)) return true;

        NPCRelationshipState npc = GetRelationship(character);
        return !npc.hasTalkedToday;
    }

    //�÷��̾� ���� Ƚ�� ���� ���Ҵ��� �˻�
    public static bool GiftGivenPossible(NPC npc)
    {
        NPCRelationshipState npcR = GetRelationship(npc);
        if (npcR.giftGivenTwiceWeek <= 2 && !npcR.giftGivenToday)
        {
            return true;
        }
        else
            return false;
    }

    public static GiftReaction GetReactionToGift(NPC character,ItemData item)
    {
        //���� ���� ���޴� �������̶��
        /*if(item is SpecialItemData) //����� �������̶�� �ڵ忡 ���� ��ȭ�� ���
        {
            return GiftReaction.Special;
        }*/
        //���� ���������ϴ� �����̶��
        if (character.bestGift.Contains(item))
            return GiftReaction.Best;
        //���� �����ϴ� �����̶��
        if (character.goodGift.Contains(item))
            return GiftReaction.Good;
        //���� �Ⱦ��ϴ� �����̶��
        if (character.badGift.Contains(item))
            return GiftReaction.Bad;
        //���� ���� �Ⱦ��ϴ� �����̶��
        if (character.worstGift.Contains(item))
            return GiftReaction.Worst;

        //�ش� �ȵǸ�
        return GiftReaction.Soso;
    }

    //���� üũ
    public static bool IsBirthday(NPC npc)
    {
        GameTimestamp birthday = npc.birthday;
        GameTimestamp today = TimeManager.instance.GetGameTimestamp();

        return (today.day == birthday.day) && (today.season == birthday.season);
    }

    //���� �� �� Ƚ�� �ö󰡰�
    public static void GiftGivenSucess(NPC npc)
    {
        NPCRelationshipState npcR = GetRelationship(npc);
        npcR.giftGivenTwiceWeek++;
        npcR.giftGivenToday = true;
        
    }

    private static bool Villain() //�������� Ȯ���� ����
    {
        System.Random rand = new System.Random();
        int totalPeople = (NpcManager.Instance.Characters().Count)-2; //�� npc �� - �ӽ�Ʈ������ ��
        int villain = DiffyToVillian(PlayerStats.Difficulty);  //�����̾�� �� ��
        int villainCount = relationships.Count(i => i.villain == true); //�̹� ������ ��� ��
        int currentPeople = relationships.Count; //��ü ���� ��� ��, ���⵵ �ӽ�Ʈ������ ���ԵǾ�����

        currentPeople++;

        // ������� ���� ���� �̾ƾ� �ϴ� ��� ���� ���� ��� ������ ������ ������ ���� ��
        if (villain - villainCount >= totalPeople - currentPeople + 1)
        {
            //currentRedBalls++;
            return true;
        }

        // �������� ���� ���� ������ ����
        if (rand.Next(totalPeople - currentPeople + 1) < villain - currentPeople)
        {
            //currentRedBalls++;
            return true;
        }

        return false;
    }
    private static int DiffyToVillian(int diffy) //���̵� ���� ������ ���� ��
    {
        switch (diffy)
        {
            case 0:
                return 1;
            case 1:
                return 2;
            case 2:
                return 4;
            case 3:
                return 8;
            default:
                return 0;
        }
    }

    public static void AddAllPoint(int point)
    {
        foreach(NPC npc in NpcManager.Instance.Characters())
        {
            AddFriendPoints(npc, point);
        }
    }

    /*
    public static bool CheckSpecialItem(NPC npc, int itemId)
    {

    }*/
}
