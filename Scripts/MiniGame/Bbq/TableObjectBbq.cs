using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableObjectBbq : TableObject
{

    // TableState 객체를 포함하는 리스트 초기화
    private List<TableState> sidemenu = new List<TableState>
    {
        new TableState(new bool[8] { false, false, true, false, false, true, false, false }, true),
        new TableState(new bool[8] { false, false, false, true, false, true, false, false }, true),
        new TableState(new bool[8] { false, true, false, false, true, false, true, true }, false),
        new TableState(new bool[8] { false, true, true, false, false, false, true, true }, false),
        new TableState(new bool[8] { true, false, true, false, false, false, false, false }, false),
        new TableState(new bool[8] { true, false, false, true, false, true, false, false }, true)
    };

    public bool isCharcoal; //숯 올려졌는지 체크
    private bool isAngry;
    private bool isSub; //서브메뉴 있는지
    public GameObject[] meatObject; //고기 메쉬 켰다껐다 용도
    //public GameObject[] drinkObject; //음료 메쉬 켰다껐다 용도
    public GameObject fxOther; //연기랑 불판 오브젝트
    public GameObject fireFx; //숯 올리면 불올라오게
    private bool isTip; //팁 줄건지 여부
    public bool CheckTableCharcoal() //숯 검사하는 코드
    {
        if (!isCharcoal) //숯 안올려져있으면
        {
            isCharcoal = true;
            fireFx.SetActive(true);
            ((BbqTableManager)ChiTableManager.instance).HumanoidOrder(myindex);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CheckTableMain(int index) //고기 검사하는 코드
    {
        if (!isCharcoal) //숯 안올려져있으면
        {
            return false;
        }
        else
        {
            if(index == mMenu && mainOn)
            {
                mainOn = false;
                EatGood();
                SoundManager.instance.PlaySound2D("hmmGood" + SoundManager.Range(1, 3)); //리액션 추가
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public void CheckTableSub(bool[] ingre, bool isCook, bool isFireBowl, bool isBurn) //냉면 검사 코드
    {
        if(isBurn || sidemenu[sMenu].isCook != isCook)
        {
            Angry();
            return;
        }

        // 일치하는 요소의 개수를 세기 위한 변수
        int matchCount = 0;

        if(isFireBowl == sidemenu[sMenu].isCook) //만약 그릇 종류 같으면
        {
            matchCount++;
        }
        // ingre 배열의 각 요소를 기준 상태와 비교
        for (int i = 0; i < ingre.Length; i++)
        {
            if (ingre[i] == sidemenu[sMenu].ingre[i])
            {
                matchCount++;
            }
        }

        // 일치하는 요소 개수에 따라 조건 분기
        if (matchCount == 9) //모두 일치
        {
            subOn = false;
            EatGood();
            isTip = true;
            isSub = true; //서브 가격 책정
            SoundManager.instance.PlaySound2D("hmmGood" + SoundManager.Range(1, 3)); //리액션 추가
        }
        else if (matchCount >= 7)
        {
            subOn = false;
            EatGood();
            isSub = true; //서브 가격 책정
            SoundManager.instance.PlaySound2D("hmmQuestion" + SoundManager.Range(1, 3)); //리액션 추가
        }
        else
        {
            // 두 개 이하 일치할 경우
            Angry();
        }

    }
    public override bool CheckTableDrink(int index) //음료수 체크, 인덱스는 들고있는 음료수 아이디값
    {
        if (DrinkOn && index == dMenu) //음료수 값 일치하면
        {
            DrinkOn = false;
            EatGood();
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (isClean)
        {
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp01(fillAmount); // fillAmount 값이 0과 1 사이에 있도록 클램핑
            images_Gauge.fillAmount = fillAmount; // 게이지 이미지에 채워진 정도 반영
            if (fillAmount == 1)
            {
                //fillAmount = 0; //게이지 처음부터
                Reset1(); //접시 다 없애고 초기화
            }
        }
    }
    public override void Reset1()
    {
        gameObject.layer = 14;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        mainOn = true;
        dirtyPlate.SetActive(false);
        //테이블매니저 오브젝트 키기
        ChiTableManager.instance.TableOnAgain(myindex);
        isAngry = false;
        isCharcoal = false;
        meatObject[mMenu].SetActive(false); //고기 메쉬 끄기
        drinkObject[dMenu].SetActive(false); //고기 메쉬 끄기
        fxOther.SetActive(false); //불판 연기 끄기
        fireFx.SetActive(false); //불판 불 끄기
        isTip = false;
        isSub = false;
        fillAmount = 0; //게이지 초기화
        if (gameObject.transform.GetChild(0).childCount > 0 &&
                gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Bowl>() != null)
        {
            Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
        }

    }
    public override void EatFinish(bool angry = false) //다먹고 더러운 접시로 바꾸기
    {
        if (!isAngry && !angry) //화 안났으면
        {
            meatObject[mMenu].SetActive(false);
            if (gameObject.transform.GetChild(0).childCount > 0 && 
                gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Bowl>() != null)
            {
                gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Bowl>().CleanIngre();
            }//그릇 속 재료들 없애기
            dirtyPlate.SetActive(true);
            CalculMoney();
            //ChiTableManager.instance.Money += 3000; //돈 3000원 추가, 나중에 바꾸기
            SoundManager.instance.PlaySound2D("casher" + SoundManager.Range(1, 2)); //돈소리 추가
        }
        gameObject.layer = 15; //카메라레이용으로 레이캐스트 바꾸기      
    }

    public void Angry() //안먹고 나감
    {
        isAngry = true;
        ((BbqTableManager)ChiTableManager.instance).HumanoidAngry(myindex);
    }

    protected override void CalculMoney() //돈 계산하는 함수
    {
        if (isSub & isTip)
        {
            int a = Random.Range(11, 21) * 100 + 2000;
            ChiTableManager.instance.AddTip(a);
        }
        else
        {
            int a = Random.Range(1, 11) * 100 + 2000;
            ChiTableManager.instance.AddTip(a);
        }
    }
}
