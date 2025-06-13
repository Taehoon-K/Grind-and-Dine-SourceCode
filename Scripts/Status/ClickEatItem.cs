using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEatItem : MonoBehaviour
{
    [SerializeField] private int itemCode; //클릭 시 바로 먹을 아이템 코드
    

    public void InteractiveThisItem() //만약 이 음식 클릭했다면
    {
        gameObject.SetActive(false);
        FindObjectOfType<Throwing>().EatReadyCutscene(itemCode); //먹기 실행
    }
}
