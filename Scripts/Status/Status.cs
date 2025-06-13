using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Status
{
    public int currentHp;
    public float currentFatigue;
    public int currentSp;
    public float currentHungry;
    public int currentAngry;
    public int currentSadness;
    public int currentBoredom;

    //��ų ������
    public int[] skillAmount = new int[7];

    public int[] level = new int[7];

    public int luckyLevel; //�� ��ġ -10~10

    public int diceLevel; //����� ���� ��ġ 1~100

    //public int[] miniPrice = new int[7];//�� �˹� �� ������ �ְ� �׼�

    // �� ��ų�� �� ���� (0: ���, 1: ���� ����, 2: ���� �Ϸ�)
    public int[,] perkStates = new int[7, 2]; // [��ų �ε���, �� ���� �ε���(0: 5��, 1: 10��)]

    // ���õ� ���� ID (�� �ܸ��� ���� ID�� �ο�, �������� ������ -1)
    public int[,] selectedPerks = new int[7, 2]; //ù��°��0, �ι�°�� 1

    public int crimianalCount; //���� ��ġ, ���� �� 5�� ����, �Ϸ翡 1�� ����

    public bool[] itemOpen = new bool[100]; //������ ���ȴ��� ����, �ݷ��ǿ��� ������
    public bool[] notInvenItemOpen = new bool[20]; //������ ���ȴ��� ����, �ݷ��ǿ��� ������

    public bool[] jobOpen = new bool[28]; //���� ���ȴ��� ����, �ݷ��ǿ��� ������
    public bool[] actOpen = new bool[28]; //Ȱ�� ���ȴ��� ����, �ݷ��ǿ��� ������
    // ���� ������ �Ǵ� Clone �޼���
    public Status Clone()
    {
        Status newStatus = new Status();

        // �⺻ �� ����
        newStatus.currentHp = this.currentHp;
        newStatus.currentFatigue = this.currentFatigue;
        newStatus.currentSp = this.currentSp;
        newStatus.currentHungry = this.currentHungry;
        newStatus.currentAngry = this.currentAngry;
        newStatus.currentSadness = this.currentSadness;
        newStatus.currentBoredom = this.currentBoredom;
        newStatus.luckyLevel = this.luckyLevel;

        // 1���� �迭 ����
        newStatus.skillAmount = (int[])this.skillAmount.Clone();
        newStatus.level = (int[])this.level.Clone();
        //newStatus.miniPrice = (int[])this.miniPrice.Clone();

        // 2���� �迭 ����
        newStatus.perkStates = (int[,])this.perkStates.Clone();
        newStatus.selectedPerks = (int[,])this.selectedPerks.Clone();

        return newStatus;
    }
}
