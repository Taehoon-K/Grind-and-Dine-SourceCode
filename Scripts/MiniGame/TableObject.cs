using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableObject : MonoBehaviour
{
    protected bool subOn = false; //서브 생성 여부
    public bool SubOn { get { return subOn; } set { subOn = value; } }
    protected bool drinkOn = false; //음료수 생성 여부
    public bool DrinkOn { get { return drinkOn; } set { drinkOn = value; } }
    protected bool mainOn = true;
    protected int mMenu, sMenu, dMenu;  //나중에 부울 값 초기화 무조건 넣어야됨,eattiming도 초기화
    public int myindex; //자기 테이블 번호 저장

    private bool isSubPlate = false; //이거 켜져있으면 설거지가는 접시 두개
    [SerializeField]
    protected GameObject dirtyPlate; //다먹을때 키는 더러운것들

    [SerializeField]
    protected Image images_Gauge; //치우기 게이지
    [SerializeField]
    protected GameObject images_Clean; //치우기 중일때 키는 게이지 이미지
    //[SerializeField]
    //private GameObject Fx_bubble; //치우기 중일때 키는 파티클 이미지
    protected bool isClean = false;  //치우기중인지

    protected float fillSpeed = 0.5f; // 게이지가 채워지는 속도
    protected float fillAmount; // 게이지 채워진 정도


    [SerializeField]
    private GameObject trayWash;

    public GameObject[] drinkObject; //음료 메쉬 켰다껐다 용도

    public void MenuNumber(int m,int s, int d)
    {
        mMenu = m;
        sMenu = s;
        dMenu = d;
    }

    public bool CheckTablePlate(int index2) //접시 놓으려할때 체크 ,인텍스는 들고있는 접시 id
    {
        if (index2 == mMenu && mainOn)
        {
            mainOn = false;
            EatGood();
            SoundManager.instance.PlaySound2D("hmmGood" + SoundManager.Range(1, 3)); //리액션 추가
            return true;
        }else if (index2 == sMenu && subOn)
        {
            subOn = false;
            isSubPlate = true;
            EatGood();
            SoundManager.instance.PlaySound2D("hmmGood" + SoundManager.Range(1, 3)); //리액션 추가
            return true;
        }
        else
        {
            return false;
        }    
    }


    public virtual bool CheckTableDrink(int index) //음료수 체크, 인덱스는 들고있는 음료수 아이디값
    {
        if (DrinkOn && TransfDrink(index)==dMenu) //음료수 값 일치하면
        {
            DrinkOn = false;
            EatGood();
            return true;
        }
        return false;
    }


    private int TransfDrink(int n)
    {
        switch (n)
        {
            case 50:
                return 0;
            case 51:
                return 1;
            case 52:
                return 2;
            default:
                return -1;
        }
    }

    protected void EatGood()
    {
        if (!subOn && !mainOn && !drinkOn)
        { //메뉴 다 나오면
            Debug.Log("eat");
            ChiTableManager.instance.HumanoidEat(myindex); //chitable 매니저 거쳐서 휴머노이드 컨트롤러에 함수 전달
        }
    }
    public virtual void EatFinish(bool angry = false) //다먹고 더러운 접시로 바꾸기
    {
        Debug.Log("Eatfinish 호출ㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹㄹ");
        /* Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
         if (isSubPlate)
         {
             Destroy(gameObject.transform.GetChild(1).GetChild(0).gameObject);
         }//원래 있던 오브젝트들 삭제*/

        if (gameObject.transform.childCount > 0 && gameObject.transform.GetChild(0).childCount > 0)
        {
            if (gameObject.transform.GetChild(0).GetChild(0).gameObject != null)
            {
                Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
            }
        }

        if (gameObject.transform.childCount > 1 && gameObject.transform.GetChild(1).childCount > 0)
        {
            if (gameObject.transform.GetChild(1).GetChild(0).gameObject != null)
            {
                Destroy(gameObject.transform.GetChild(1).GetChild(0).gameObject);
            }
        }

        dirtyPlate.SetActive(true);
        CalculMoney();
        SoundManager.instance.PlaySound2D("casher" + SoundManager.Range(1, 2)); //돈소리 추가
        gameObject.layer = 15; //카메라레이용으로 레이캐스트 바꾸기
        
    }
    public void TableAngry() //화나서 나갈때 테이블 더럽게하는용도
    {
        dirtyPlate.SetActive(true);
    }
    public bool IsClean
    {
        get { return isClean; }
        set
        {
            images_Clean.SetActive(value);
            //Fx_bubble.SetActive(value);
            isClean = value;
        }
    }
    private void Update()
    {
        if (isClean)
        {
            //Debug.Log(fillAmount);
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp01(fillAmount); // fillAmount 값이 0과 1 사이에 있도록 클램핑
            images_Gauge.fillAmount = fillAmount; // 게이지 이미지에 채워진 정도 반영
            if (fillAmount == 1)
            {
                fillAmount = 0; //게이지 처음부터
                Reset1(); //접시 다 없애고 초기화
            }
        }
    }

    public virtual void Reset1()
    {
        gameObject.layer = 14;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        mainOn = true;
        isSubPlate = false;
        dirtyPlate.SetActive(false);
        //테이블매니저 오브젝트 키기
        ChiTableManager.instance.TableOnAgain(myindex);

        drinkObject[dMenu].SetActive(false); //고기 메쉬 끄기

    }

    protected virtual void CalculMoney()
    {
        if (isSubPlate)
        {
            int a = Random.Range(11, 21) * 100 + 1500;
            ChiTableManager.instance.AddTip(a);
        }
        else
        {
            int a = Random.Range(1, 11) * 100 + 1500;
            ChiTableManager.instance.AddTip(a);
        }

        /*int tip = Random.Range(1, 21);
        if (StatusManager.instance != null && GetLuckBasedBoolean())
        {
            int a = Random.Range(50, 101) * 10;
            ChiTableManager.instance.AddTip(a);
        }*/
    }
    private bool GetLuckBasedBoolean()
    {
        int luck = StatusManager.instance.GetLuckLevel();
        // 운 수치를 확률로 변환 (-10 → 25%, 0 → 50%, 10 → 75%)
        float probability = 0.5f + (luck * 0.025f);

        // 0 ~ 1 랜덤 값 생성 후 확률과 비교
        return Random.value < probability;
    }

}
