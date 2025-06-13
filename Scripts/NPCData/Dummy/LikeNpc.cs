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
    protected int[] BestfriendNpc; //ģ�� ģ���� npc�� �ڵ� ���, �ڽ� ȣ���� ���� �� ���� ���� ����
    [SerializeField]
    protected int[] FriendNpc; //ģ���� npc�� �ڵ� ���, �ڽ� ȣ���� ���� �� ���� ���� ����

    public bool giftSucceed { get; private set; } //���� �޾��� ��

    protected int Likeability
    {
        get { return NpcManager.instance.likeAbility[NpcCode]; } //�ڱ� npc�ڵ��� ȣ���� �� �Ŵ������� ��������
        set
        {
            int oldValue = Likeability; //���� ȣ���� ����

            if (value > oldValue && !NpcManager.instance.billain[NpcCode]) //���� �ƴϰ� ȣ���� �������� ���
            {
                int increaseAmount = (value - oldValue) / 2; // ������ ���� ���� ���
                for (int i = 0; i < BestfriendNpc.Length; i++)  //ģ�� ģ���Ͻ�
                {
                    NpcManager.instance.likeAbility[BestfriendNpc[i]] += increaseAmount; // �� �迭 ����� ���� ����
                    if (NpcManager.instance.likeAbility[BestfriendNpc[i]] > 1000)
                    {
                        NpcManager.instance.likeAbility[BestfriendNpc[i]] = 1000; //ȣ���� �ִ� 100 ����
                    }
                }
                for (int i = 0; i <FriendNpc.Length; i++) //�׳� ģ��
                {
                    NpcManager.instance.likeAbility[FriendNpc[i]] += increaseAmount/2;
                    if (NpcManager.instance.likeAbility[FriendNpc[i]] > 1000)
                    {
                        NpcManager.instance.likeAbility[FriendNpc[i]] = 1000; //ȣ���� �ִ� 100 ����
                    }
                }

            }
            if (value < oldValue) //ȣ���� ����������
            {
                int decreaseAmount = (value - oldValue) / 2; // ������ ���� ���� ���
                for (int i = 0; i < BestfriendNpc.Length; i++)
                {
                    NpcManager.instance.likeAbility[BestfriendNpc[i]] += decreaseAmount; // �� �迭 ����� ���� ����
                    if (NpcManager.instance.likeAbility[BestfriendNpc[i]] < 0)
                    {
                        NpcManager.instance.likeAbility[BestfriendNpc[i]] = 0; //ȣ���� �ּ� 0 ����
                    }
                }
                for (int i = 0; i < FriendNpc.Length; i++)
                {
                    NpcManager.instance.likeAbility[FriendNpc[i]] += decreaseAmount / 2;
                    if (NpcManager.instance.likeAbility[FriendNpc[i]] < 0)
                    {
                        NpcManager.instance.likeAbility[FriendNpc[i]] = 0; //ȣ���� �ּ� 0 ����
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

    public void LikeChange(int ability) //ability��ƴ ȣ���� ����, ���߿� ��ȭ�� �� ȣ���� ���� �� ���
    {
        Likeability += ability;
    }

    public string TalkToNodeGift(int target) //����ִ� ������ ���� ��� �迭�� �ֳ� ���캽
    {
        if (NpcManager.instance.likeAbility[NpcCode] < 10) //ȣ������ 10 ���ϸ�(���� ���¸�)
        {
            giftSucceed = true;
            return TalkToNode() + "AngryGift"; //���� ���¶� ���� �ź�
        }
        else //���� ���°� �ƴ϶��
        {
            if (NpcManager.instance.giftTime[NpcCode] < 2) //���� Ƚ�� ���� ��ä�����ٸ�
            {
                if (Array.IndexOf(BestGift, target) != -1)
                {
                    Likeability += 5;
                    giftSucceed = true;
                    NpcManager.instance.giftTime[NpcCode]++;
                    return TalkToNode() + "Event"; //���߿� �����Ұ�!
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
                    return TalkToNode() + "Nothing"; // �ش� ���ڰ� ��� �迭���� ���� ���
                }
            }
            else //���� Ƚ�� ä���ٸ�
            {
                giftSucceed = false;
                return TalkToNode() + "Enough"; //���� ���� �޾Ƽ� ����
            }
        }
    }
}
*/