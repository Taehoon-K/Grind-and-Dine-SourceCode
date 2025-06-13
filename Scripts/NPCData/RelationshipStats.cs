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
    public static bool FirstMeeting(NPC npc) //첫만남이라면 true
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

    public static List<NPCRelationshipState> GetRelationShip() //저장할때 관계 불러올용도, 블랙보드 때문에 이제 안씀
    {
        return relationships;
    }

    //로드할때 관계 로드
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

    //npc 리스트에 추가,여기 추가할 때 빌런 랜덤 결정. 난이도 따라 확률 낮추고 최대 빌런 수 제한 코드 넣을것
    public static void UnlockCharacter(NPC npc)
    {
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();

        // 이미 언락된 캐릭터라면 중복 방지
        if (!FirstMeeting(npc))
        {
            Debug.Log($"{npc.CharacterName()}은(는) 이미 언락된 NPC입니다.");
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

        // 중복 방지 후 리스트에 추가
        if (!relationships.Any(r => r.npcCode == npc.GetNpcCode()))
        {
            relationships.Add(relationship);
        }
        else
        {
            Debug.Log($"{npc.CharacterName()}은(는) relationships에 이미 존재합니다.");
        }

    }

    public static void UpdateLoveStatus(NPC npc, bool loved) //npc와 사귀는지의 여부 바꾸는 함수
    {
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();

        // NPC의 정보를 relationships에서 찾습니다.
        NPCRelationshipState relationship = relationships.FirstOrDefault(r => r.npcCode == npc.GetNpcCode());

        // 만약 관계가 존재한다면
        if (relationship != null)
        {
            // hasTalkedToday 값을 업데이트합니다.
            relationship.loveWith = loved;

            // 블랙보드에 업데이트된 관계 상태를 다시 설정합니다.
            blackboard.SetValue(RELATIONSHIP_PREFIX + npc.CharacterName(), relationship);
        }
        else
        {
            Debug.LogWarning("NPC 관계 상태가 없습니다: " + npc.CharacterName());
        }
    }

    public static void AddFriendPoints(NPC npc, int points)
    {
        if (FirstMeeting(npc))
        {
            Debug.LogError("아직 안만났음");
            return;
        }

        var relationship = GetRelationship(npc);

        int point;
        point = points;
        if (npc.isWoman && StatusManager.instance.GetSelectedPerk(4, 0) == 1 && points > 0) //여자고 퍽 찍었다면
        {
            point =  Mathf.RoundToInt(point * 1.2f);
        }
        else if (!npc.isWoman && StatusManager.instance.GetSelectedPerk(4, 0) == 0 && points > 0) //남자이고 퍽 찍었다면
        {
            point = Mathf.RoundToInt(point * 1.2f);
        }
        GetRelationship(npc).friendshipPoints += point;

        if (!relationship.loveWith && relationship.friendshipPoints > 2000)   //사귀지 않으면 2000 이하로
        {
            relationship.friendshipPoints = 2000;
        }

        // friendshipPoints를 -2000과 3000 사이로 조정
        relationship.friendshipPoints = Mathf.Clamp(relationship.friendshipPoints, -2000, 3000);
    }

    //오늘 처음 대화인지 체크하는 함수
    public static bool IsFirstConversationOfTheDay(NPC character)
    {
        //만약 아예 처음이라면
        if (FirstMeeting(character)) return true;

        NPCRelationshipState npc = GetRelationship(character);
        return !npc.hasTalkedToday;
    }

    //플레이어 선물 횟수 아직 남았는지 검사
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
        //만약 선물 못받는 아이템이라면
        /*if(item is SpecialItemData) //스페셜 아이템이라면 코드에 따라 대화문 출력
        {
            return GiftReaction.Special;
        }*/
        //만약 제일좋아하는 선물이라면
        if (character.bestGift.Contains(item))
            return GiftReaction.Best;
        //만약 좋아하는 선물이라면
        if (character.goodGift.Contains(item))
            return GiftReaction.Good;
        //만약 싫어하는 선물이라면
        if (character.badGift.Contains(item))
            return GiftReaction.Bad;
        //만약 제일 싫어하는 선물이라면
        if (character.worstGift.Contains(item))
            return GiftReaction.Worst;

        //해당 안되면
        return GiftReaction.Soso;
    }

    //생일 체크
    public static bool IsBirthday(NPC npc)
    {
        GameTimestamp birthday = npc.birthday;
        GameTimestamp today = TimeManager.instance.GetGameTimestamp();

        return (today.day == birthday.day) && (today.season == birthday.season);
    }

    //선물 줄 시 횟수 올라가게
    public static void GiftGivenSucess(NPC npc)
    {
        NPCRelationshipState npcR = GetRelationship(npc);
        npcR.giftGivenTwiceWeek++;
        npcR.giftGivenToday = true;
        
    }

    private static bool Villain() //빌런인지 확률로 정함
    {
        System.Random rand = new System.Random();
        int totalPeople = (NpcManager.Instance.Characters().Count)-2; //총 npc 수 - 머스트낫빌런 수
        int villain = DiffyToVillian(PlayerStats.Difficulty);  //빌런이어야 될 수
        int villainCount = relationships.Count(i => i.villain == true); //이미 빌런인 사람 수
        int currentPeople = relationships.Count; //전체 만난 사람 수, 여기도 머스트낫빌런 포함되어있음

        currentPeople++;

        // 현재까지 빨간 공을 뽑아야 하는 사람 수가 남은 사람 수보다 많으면 무조건 빨간 공
        if (villain - villainCount >= totalPeople - currentPeople + 1)
        {
            //currentRedBalls++;
            return true;
        }

        // 무작위로 빨간 공을 뽑을지 결정
        if (rand.Next(totalPeople - currentPeople + 1) < villain - currentPeople)
        {
            //currentRedBalls++;
            return true;
        }

        return false;
    }
    private static int DiffyToVillian(int diffy) //난이도 따라 결정될 빌런 수
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
