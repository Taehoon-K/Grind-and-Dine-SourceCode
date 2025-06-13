using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public abstract class LikeNpc : NPC
{
    [SerializeField]
    protected int[] BestGift;
    [SerializeField]
    protected int[] GoodGift;
    [SerializeField]
    protected int[] SosoGift;
    [SerializeField]
    protected int[] BadGift;
    [SerializeField]
    protected int[] WorstGift;

    [SerializeField]
    protected int[] BestfriendNpc; //친한 친구인 npc들 코드 목록, 자신 호감도 증감 시 같이 절반 증감
    [SerializeField]
    protected int[] FriendNpc; //친구인 npc들 코드 목록, 자신 호감도 증감 시 같이 쿼터 증감

    public bool giftSucceed { get; private set; } //선물 받았을 시

    protected int Likeability
    {
        get { return NpcManager.instance.likeAbility[NpcCode]; } //자기 npc코드의 호감도 값 매니저에서 가져오기
        set
        {
            int oldValue = Likeability; //예전 호감도 저장

            if (value > oldValue && !NpcManager.instance.billain[NpcCode]) //빌런 아니고 호감도 증가했을 경우
            {
                int increaseAmount = (value - oldValue) / 2; // 증가한 값의 절반 계산
                for (int i = 0; i < BestfriendNpc.Length; i++)  //친한 친구일시
                {
                    NpcManager.instance.likeAbility[BestfriendNpc[i]] += increaseAmount; // 각 배열 요소의 값을 증가
                    if (NpcManager.instance.likeAbility[BestfriendNpc[i]] > 1000)
                    {
                        NpcManager.instance.likeAbility[BestfriendNpc[i]] = 1000; //호감도 최대 100 조정
                    }
                }
                for (int i = 0; i <FriendNpc.Length; i++) //그냥 친구
                {
                    NpcManager.instance.likeAbility[FriendNpc[i]] += increaseAmount/2;
                    if (NpcManager.instance.likeAbility[FriendNpc[i]] > 1000)
                    {
                        NpcManager.instance.likeAbility[FriendNpc[i]] = 1000; //호감도 최대 100 조정
                    }
                }

            }
            if (value < oldValue) //호감도 감소했을시
            {
                int decreaseAmount = (value - oldValue) / 2; // 증가한 값의 절반 계산
                for (int i = 0; i < BestfriendNpc.Length; i++)
                {
                    NpcManager.instance.likeAbility[BestfriendNpc[i]] += decreaseAmount; // 각 배열 요소의 값을 증가
                    if (NpcManager.instance.likeAbility[BestfriendNpc[i]] < 0)
                    {
                        NpcManager.instance.likeAbility[BestfriendNpc[i]] = 0; //호감도 최소 0 조정
                    }
                }
                for (int i = 0; i < FriendNpc.Length; i++)
                {
                    NpcManager.instance.likeAbility[FriendNpc[i]] += decreaseAmount / 2;
                    if (NpcManager.instance.likeAbility[FriendNpc[i]] < 0)
                    {
                        NpcManager.instance.likeAbility[FriendNpc[i]] = 0; //호감도 최소 0 조정
                    }
                }
            } 


            if (value < 0)
            {
                NpcManager.instance.likeAbility[NpcCode] = 0;
            }
            else if (value > 1000)
            {
                NpcManager.instance.likeAbility[NpcCode] = 1000;
            }
            else
            {
                NpcManager.instance.likeAbility[NpcCode] = value;
            }
  
        }
    }

    public void LikeChange(int ability) //ability만틈 호감도 증감, 나중에 대화문 등 호감도 증감 시 사용
    {
        Likeability += ability;
    }

    public string TalkToNodeGift(int target) //들고있는 선물이 선물 목록 배열에 있나 살펴봄
    {
        if (NpcManager.instance.likeAbility[NpcCode] < 10) //호감도가 10 이하면(증오 상태면)
        {
            giftSucceed = true;
            return TalkToNode() + "AngryGift"; //증오 상태라 선물 거부
        }
        else //증오 상태가 아니라면
        {
            if (NpcManager.instance.giftTime[NpcCode] < 2) //선물 횟수 아직 안채워졌다면
            {
                if (Array.IndexOf(BestGift, target) != -1)
                {
                    Likeability += 5;
                    giftSucceed = true;
                    NpcManager.instance.giftTime[NpcCode]++;
                    return TalkToNode() + "Event"; //나중에 수정할것!
                }
                else if (Array.IndexOf(GoodGift, target) != -1)
                {
                    Likeability += 3;
                    giftSucceed = true;
                    NpcManager.instance.giftTime[NpcCode]++;
                    return TalkToNode() + "Good";
                }
                else if (Array.IndexOf(SosoGift, target) != -1)
                {
                    Likeability += 1;
                    giftSucceed = true;
                    NpcManager.instance.giftTime[NpcCode]++;
                    return TalkToNode() + "Soso";
                }
                else if (Array.IndexOf(BadGift, target) != -1)
                {
                    Likeability -= 3;
                    giftSucceed = true;
                    NpcManager.instance.giftTime[NpcCode]++;
                    return TalkToNode() + "Bad";
                }
                else if (Array.IndexOf(WorstGift, target) != -1)
                {
                    Likeability -= 5;
                    giftSucceed = true;
                    NpcManager.instance.giftTime[NpcCode]++;
                    return TalkToNode() + "Worst";
                }
                else
                {
                    giftSucceed = false;
                    return TalkToNode() + "Nothing"; // 해당 숫자가 어느 배열에도 없는 경우
                }
            }
            else //선물 횟수 채웠다면
            {
                giftSucceed = false;
                return TalkToNode() + "Enough"; //선물 많이 받아서 거절
            }
        }
    }
}
*/