using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCRelationshipState
{
    public int npcCode;
    public int friendshipPoints;
    public bool villain;

    public bool hasTalkedToday;
    public bool giftGivenToday =false;
    public int giftGivenTwiceWeek;

    public bool loveWith; //��ʹ��� ����, ó���� ����

    public NPCRelationshipState()
    {
    }
    public NPCRelationshipState(int code, int friendshipPoints)
    {
        this.npcCode = code;
        this.friendshipPoints = friendshipPoints;
    }
    public NPCRelationshipState(int code, bool villain = false) //n[c ó�� ������
    {
        this.npcCode = code;
        friendshipPoints = 0;
        this.villain = villain;
    }

    public float Hearts
    {
       get { return (float)friendshipPoints / 250; }
    }
}
