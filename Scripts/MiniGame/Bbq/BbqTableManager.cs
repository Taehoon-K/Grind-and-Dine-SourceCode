using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BbqTableManager : ChiTableManager
{
    public GameObject[] tableSub2Chair; //����2 ���� ������Ʈ
    [SerializeField]
    private GameObject npcSub2; //������ ����2 npc
    [SerializeField]
    private Transform npcSub2GenPo; //npc ����2 ���� ��ġ
    private GameObject[] projectileSub2 = new GameObject[8];

    protected void Awake()
    {
        // ChiTableManager�� Awake�� ȣ���Ͽ� �θ� Ŭ������ �̱��� ����
        base.Awake();

        // �߰������� B Ŭ�������� ������ �ʱ�ȭ �۾��� �ִٸ� ���⿡ �ۼ��մϴ�.
    }
    protected override void createOrder(int count) //�մ� ���� �Լ�
    {
        subChatOn = false;
        drinkChatOn = false; //�ʱ�ȭ

        int falseCount = count;

        // ���� ���� �� ����
        int randomIndex = Random.Range(0, falseCount);
        // Find the random false element and set it to true
        int currentIndex = 0;
        for (int i = 0; i < tableOn.Length; i++)
        {
            if (!tableOn[i])
            {
                if (currentIndex == randomIndex)
                {
                    tableOn[i] = true;
                    tableId = i;
                    Debug.Log("Random false element changed to true at index: " + i);
                    i1 = i;
                    break;
                }
                currentIndex++;
            }
        }
        mainMenu = Random.Range(0, 4); //0~3 �޴��� ����

        int peopleNum = Random.Range(0, 6); //0~5 ���� ���� ����, �մ� �� ������
                                            //0: 1��, 1~3: 2��, 4~5: 3��

        if (peopleNum >= 2)
        {
            tableObject[i1].GetComponent<TableObject>().SubOn = true;
            subChatOn = true; //����޴� ��ǳ���� ����
            subMenu = Random.Range(0, 6); //0���� 5�� �޴� �� ����
        }

        int rando1 = Random.Range(0, 2); //50��, �帵ũ ���� ����
        if (rando1 == 0)
        {
            drinkChatOn = true;
            tableObject[i1].GetComponent<TableObject>().DrinkOn = true;
            drinkMenu = Random.Range(0, 3); //0���� 2�� �޴� �� ����
        }

        tableObject[i1].GetComponent<TableObject>().MenuNumber(mainMenu, subMenu, drinkMenu);

        projectile[i1] = Instantiate(npc, npcGenPo); //npc ����
        projectile[i1].GetComponent<HumanoidController>().Appearance(); //npc ���� �����ϴ� �Լ� ȣ��
        projectile[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().GetVariable("table").SetValue(tableChair[tableId]);
        //���� ���̺� ������Ʈ �Ҵ�       
        projectile[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;
        if (peopleNum >= 1) //2�� �̻��϶� npcSub ����
        {
            projectileSub[i1] = Instantiate(npcSub, npcSubGenPo);
            projectileSub[i1].GetComponent<HumanoidControllerSub>().Appearance(); //npc ���� �����ϴ� �Լ� ȣ��
            projectileSub[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().GetVariable("table").SetValue(tableSubChair[tableId]);
            projectileSub[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;

            if (peopleNum >= 4) //3�� �̻��Ͻ� sub2 ����
            {
                projectileSub2[i1] = Instantiate(npcSub2, npcSub2GenPo);
                projectileSub2[i1].GetComponent<HumanoidControllerSub>().Appearance(); //npc ���� �����ϴ� �Լ� ȣ��
                projectileSub2[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().GetVariable("table").SetValue(tableSub2Chair[tableId]);
                projectileSub2[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;
            }
        }
    }

    public void HumanoidOrder(int index)
    {
        projectile[index].GetComponent<HumanoidControllerBbq>().Order();
    }
    public void HumanoidAngry(int index)
    {
        projectile[index].GetComponent<HumanoidControllerSub>().Angry();
        if (projectileSub[index] != null) //���� ����մԵ� �ִٸ�
        {
            projectileSub[index].GetComponent<HumanoidControllerSub>().Angry();
        }
        if (projectileSub2[index] != null) //���� ����2�մԵ� �ִٸ�
        {
            projectileSub2[index].GetComponent<HumanoidControllerSub>().Angry();
        }
    }

    public override void HumanoidEat(int index)
    {
        projectile[index].GetComponent<HumanoidController>().Eat(); //eat �Լ� ȣ��
        if (projectileSub[index] != null) //���� ����մԵ� �ִٸ�
        {
            projectileSub[index].GetComponent<HumanoidControllerSub>().Eat();
        }
        if (projectileSub2[index] != null) //���� ����2�մԵ� �ִٸ�
        {
            projectileSub2[index].GetComponent<HumanoidControllerSub>().Eat();
        }
    }
}
