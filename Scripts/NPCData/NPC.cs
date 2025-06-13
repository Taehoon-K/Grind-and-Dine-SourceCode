using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP.InventorySystem;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "NPC")]
public class NPC : ScriptableObject
{
    public Sprite portraitImage;
    public LocalizedString name_key;
    [SerializeField]
    protected string _chracterName;
    [SerializeField]
    protected string _talkToNode;
    [SerializeField]
    protected int NpcCode; //각자 npc 자기 코드
    public bool mustNotV;
    public bool canLove; //연애 가능한지 여부

    public bool isWoman; //남자인지 여자인지 여부

    public bool hasParent; //부모 집에 사는지 여부
    public string parentName;

    public GameTimestamp birthday;
    public List<ItemData> bestGift;
    public List<ItemData> goodGift;
    public List<ItemData> badGift;
    public List<ItemData> worstGift;
    public GameObject prefab;

    [SerializeField]
    protected int[] BestfriendNpc; //친한 친구인 npc들 코드 목록, 자신 호감도 증감 시 같이 절반 증감
    [SerializeField]
    protected int[] FriendNpc; //친구인 npc들 코드 목록, 자신 호감도 증감 시 같이 쿼터 증감

    public DialogueCondition[] dialogues;

    //public abstract string CharacterName(); //캐릭터 이름
    //public abstract string TalkToNode(); //평소 말걸때 노드
    public string CharacterName() //캐릭터 이름
    {
        return _chracterName;
    }
    public string TalkToNode()//평소 말걸때 노드
    {
        return _talkToNode;
    }
    public string TalkToNodeFirstOfDay()//하루의 처음 말걸때 노드
    {
        return _talkToNode+"Day";
    }
    public string TalkToNodeFirst()//맨 처음 말걸때 노드
    {
        return _talkToNode+"First";
    }
    public int GetNpcCode()
    {
        return NpcCode;
    }
    public string TalkToNodeNoGift()
    {
        return _talkToNode + "NoGift";
    }
    //이벤트 대화문
    /*public string TalkEvent(string eventNode)
    {
        return _talkToNode + eventNode;
    }*/

    //생일선물 시 다이얼로그들
    public string TalkToNodeBirthdayGood()
    {
        return _talkToNode + "BirthGood";
    }
    public string TalkToNodeBirthdaySoso()
    {
        return _talkToNode + "BirthSoso";
    }
    public string TalkToNodeBirthdayBad()
    {
        return _talkToNode + "BirthBad";
    }

    //선물 시 다이얼로그들
    public string TalkToNodeBest()
    {
        return _talkToNode + "Best";
    }
    public string TalkToNodeGood()
    {
        return _talkToNode + "Good";
    }
    public string TalkToNodeSoso()
    {
        return _talkToNode + "Soso";
    }
    public string TalkToNodeBad()
    {
        return _talkToNode + "Bad";
    }
    public string TalkToNodeWorst()
    {
        return _talkToNode + "Worst";
    }

    public string TalkToNodeReject() //빌런일 시 고백 거절
    {
        return _talkToNode + "Reject";
    }
    public string TalkToNodeAccept()
    {
        return _talkToNode + "Accept";
    }
    public string TalkToNodeImposs()
    {
        return _talkToNode + "Imposs";
    }
}