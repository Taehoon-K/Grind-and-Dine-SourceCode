using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BbqTableManager : ChiTableManager
{
    public GameObject[] tableSub2Chair; //서브2 의자 오브젝트
    [SerializeField]
    private GameObject npcSub2; //복제할 서브2 npc
    [SerializeField]
    private Transform npcSub2GenPo; //npc 서브2 생성 위치
    private GameObject[] projectileSub2 = new GameObject[8];

    protected void Awake()
    {
        // ChiTableManager의 Awake를 호출하여 부모 클래스의 싱글톤 설정
        base.Awake();

        // 추가적으로 B 클래스에서 수행할 초기화 작업이 있다면 여기에 작성합니다.
    }
    protected override void createOrder(int count) //손님 생성 함수
    {
        subChatOn = false;
        drinkChatOn = false; //초기화

        int falseCount = count;

        // 폴스 갯수 중 랜덤
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
        mainMenu = Random.Range(0, 4); //0~3 메뉴중 랜덤

        int peopleNum = Random.Range(0, 6); //0~5 사이 난수 생성, 손님 수 결정용
                                            //0: 1명, 1~3: 2명, 4~5: 3명

        if (peopleNum >= 2)
        {
            tableObject[i1].GetComponent<TableObject>().SubOn = true;
            subChatOn = true; //서브메뉴 말풍선도 생김
            subMenu = Random.Range(0, 6); //0에서 5번 메뉴 중 랜덤
        }

        int rando1 = Random.Range(0, 2); //50퍼, 드링크 생성 여부
        if (rando1 == 0)
        {
            drinkChatOn = true;
            tableObject[i1].GetComponent<TableObject>().DrinkOn = true;
            drinkMenu = Random.Range(0, 3); //0에서 2번 메뉴 중 랜덤
        }

        tableObject[i1].GetComponent<TableObject>().MenuNumber(mainMenu, subMenu, drinkMenu);

        projectile[i1] = Instantiate(npc, npcGenPo); //npc 생성
        projectile[i1].GetComponent<HumanoidController>().Appearance(); //npc 외형 결정하는 함수 호출
        projectile[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().GetVariable("table").SetValue(tableChair[tableId]);
        //앉을 테이블 오브젝트 할당       
        projectile[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;
        if (peopleNum >= 1) //2명 이상일때 npcSub 생성
        {
            projectileSub[i1] = Instantiate(npcSub, npcSubGenPo);
            projectileSub[i1].GetComponent<HumanoidControllerSub>().Appearance(); //npc 외형 결정하는 함수 호출
            projectileSub[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().GetVariable("table").SetValue(tableSubChair[tableId]);
            projectileSub[i1].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = true;

            if (peopleNum >= 4) //3명 이상일시 sub2 생성
            {
                projectileSub2[i1] = Instantiate(npcSub2, npcSub2GenPo);
                projectileSub2[i1].GetComponent<HumanoidControllerSub>().Appearance(); //npc 외형 결정하는 함수 호출
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
        if (projectileSub[index] != null) //만약 서브손님도 있다면
        {
            projectileSub[index].GetComponent<HumanoidControllerSub>().Angry();
        }
        if (projectileSub2[index] != null) //만약 서브2손님도 있다면
        {
            projectileSub2[index].GetComponent<HumanoidControllerSub>().Angry();
        }
    }

    public override void HumanoidEat(int index)
    {
        projectile[index].GetComponent<HumanoidController>().Eat(); //eat 함수 호출
        if (projectileSub[index] != null) //만약 서브손님도 있다면
        {
            projectileSub[index].GetComponent<HumanoidControllerSub>().Eat();
        }
        if (projectileSub2[index] != null) //만약 서브2손님도 있다면
        {
            projectileSub2[index].GetComponent<HumanoidControllerSub>().Eat();
        }
    }
}
