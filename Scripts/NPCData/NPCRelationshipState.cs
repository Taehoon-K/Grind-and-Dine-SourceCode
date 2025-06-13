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

    public bool loveWith; //사귀는지 여부, 처음엔 폴스

    public NPCRelationshipState()
    {
    }
    public NPCRelationshipState(int code, int friendshipPoints)
    {
        this.npcCode = code;
        this.friendshipPoints = friendshipPoints;
    }
    public NPCRelationshipState(int code, bool villain = false) //n[c 처음 만날때
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
