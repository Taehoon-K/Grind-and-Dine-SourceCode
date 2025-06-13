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

    //스킬 레벨들
    public int[] skillAmount = new int[7];

    public int[] level = new int[7];

    public int luckyLevel; //운 수치 -10~10

    public int diceLevel; //설득용 난수 수치 1~100

    //public int[] miniPrice = new int[7];//각 알바 시 벌었던 최고 액수

    // 각 스킬의 퍽 상태 (0: 잠김, 1: 선택 가능, 2: 선택 완료)
    public int[,] perkStates = new int[7, 2]; // [스킬 인덱스, 퍽 레벨 인덱스(0: 5렙, 1: 10렙)]

    // 선택된 퍽의 ID (각 퍽마다 고유 ID를 부여, 선택하지 않으면 -1)
    public int[,] selectedPerks = new int[7, 2]; //첫번째는0, 두번째는 1

    public int crimianalCount; //범죄 수치, 범죄 시 5씩 증가, 하루에 1씩 감소

    public bool[] itemOpen = new bool[100]; //아이템 열렸는지 여부, 콜렉션에서 참조용
    public bool[] notInvenItemOpen = new bool[20]; //아이템 열렸는지 여부, 콜렉션에서 참조용

    public bool[] jobOpen = new bool[28]; //직업 열렸는지 여부, 콜렉션에서 참조용
    public bool[] actOpen = new bool[28]; //활동 열렸는지 여부, 콜렉션에서 참조용
    // 복사 생성자 또는 Clone 메서드
    public Status Clone()
    {
        Status newStatus = new Status();

        // 기본 값 복사
        newStatus.currentHp = this.currentHp;
        newStatus.currentFatigue = this.currentFatigue;
        newStatus.currentSp = this.currentSp;
        newStatus.currentHungry = this.currentHungry;
        newStatus.currentAngry = this.currentAngry;
        newStatus.currentSadness = this.currentSadness;
        newStatus.currentBoredom = this.currentBoredom;
        newStatus.luckyLevel = this.luckyLevel;

        // 1차원 배열 복사
        newStatus.skillAmount = (int[])this.skillAmount.Clone();
        newStatus.level = (int[])this.level.Clone();
        //newStatus.miniPrice = (int[])this.miniPrice.Clone();

        // 2차원 배열 복사
        newStatus.perkStates = (int[,])this.perkStates.Clone();
        newStatus.selectedPerks = (int[,])this.selectedPerks.Clone();

        return newStatus;
    }
}
