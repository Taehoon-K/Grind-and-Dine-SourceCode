using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCoffee : MonoBehaviour
{
    [SerializeField]
    private BoxCollider coffeeColider,sideColider;

    public GameObject[] sideObject;

    private bool sideOn, coffeeOn; //사이드랑 커피 있는지 여부
    public int sideId;
    private bool[] ingredient;
    private bool mixOn;

    public void SetCoffee(bool[] igre, bool mix, bool on)
    {
        coffeeOn = on;
        coffeeColider.enabled = on;
        ingredient = igre;
        mixOn = mix;
    }
    public bool GetCoffee() //이미 커피 있는지 여부
    {
        return coffeeOn;
    }
    public void OffCoffee() //커피 가져갈때 호출되는 함수, 데스크 레이어
    {
        coffeeOn = false; ingredient = null;
        coffeeColider.enabled = false;
    }
    public void SetSide(int id, bool on)
    {
        sideOn = on;
        sideColider.enabled = on;
        sideId = id;

        if (!on)
        {
            OffSide();
        }
        //사이드박스콜라이더는 태크를 아이템으로 해서, id로 다시 가져갈 수 있게
    }
    private void OffSide() //사이드 다 끄기
    {
        for (int i = 0; i < sideObject.Length; i++) 
        {
            sideObject[i].SetActive(false);
        }
    }

    public void RingBell() //완료 종 누르면 호출될 함수
    {
        //종소리 추라
        SoundManager.instance.PlaySound2D("bell"); //딸깍 소리 재생
        //커피 없으면 리턴
        if (!coffeeOn)
        {
            CoffeeTableManager.instance.NoticeCreate("noCoffee_key");
            //뭔가 빠졌다는 안내문
            return;
        }
        //검사 코드
        if (CoffeeTableManager.instance.CheckCoffee(ingredient, mixOn, sideOn, sideId))
        {
            CalculMoney(); //돈 계산
            Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
            OffCoffee(); //커피 삭제
            SetSide(0,false); //사이드 삭제
        }
        
    }
    private void CalculMoney()
    {
        if (sideOn)
        {
            int a = Random.Range(1, 11) * 100 + 800;
            CoffeeTableManager.instance.AddTip(a);
        }
        else
        {
            int a = Random.Range(1, 8) * 100 + 800;
            CoffeeTableManager.instance.AddTip(a);
        }
        /*
        int tip = Random.Range(1, 101);
        if (StatusManager.instance != null && tip <= StatusManager.instance.GetStatus().luckyLevel)
        {
            int a = Random.Range(50, 101) * 10;
            CoffeeTableManager.instance.AddTip(a);
        }*/
    }
}
