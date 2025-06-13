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
    protected int NpcCode; //���� npc �ڱ� �ڵ�
    public bool mustNotV;
    public bool canLove; //���� �������� ����

    public bool isWoman; //�������� �������� ����

    public bool hasParent; //�θ� ���� ����� ����
    public string parentName;

    public GameTimestamp birthday;
    public List<ItemData> bestGift;
    public List<ItemData> goodGift;
    public List<ItemData> badGift;
    public List<ItemData> worstGift;
    public GameObject prefab;

    [SerializeField]
    protected int[] BestfriendNpc; //ģ�� ģ���� npc�� �ڵ� ���, �ڽ� ȣ���� ���� �� ���� ���� ����
    [SerializeField]
    protected int[] FriendNpc; //ģ���� npc�� �ڵ� ���, �ڽ� ȣ���� ���� �� ���� ���� ����

    public DialogueCondition[] dialogues;

    //public abstract string CharacterName(); //ĳ���� �̸�
    //public abstract string TalkToNode(); //��� ���ɶ� ���
    public string CharacterName() //ĳ���� �̸�
    {
        return _chracterName;
    }
    public string TalkToNode()//��� ���ɶ� ���
    {
        return _talkToNode;
    }
    public string TalkToNodeFirstOfDay()//�Ϸ��� ó�� ���ɶ� ���
    {
        return _talkToNode+"Day";
    }
    public string TalkToNodeFirst()//�� ó�� ���ɶ� ���
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
    //�̺�Ʈ ��ȭ��
    /*public string TalkEvent(string eventNode)
    {
        return _talkToNode + eventNode;
    }*/

    //���ϼ��� �� ���̾�α׵�
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

    //���� �� ���̾�α׵�
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

    public string TalkToNodeReject() //������ �� ��� ����
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