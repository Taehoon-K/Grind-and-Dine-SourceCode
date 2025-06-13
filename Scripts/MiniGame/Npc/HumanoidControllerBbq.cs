using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidControllerBbq : HumanoidController
{
    public override void Hello() //비헤이비어 트리에서 호출
    {
        animator.SetTrigger("isHello");
        ChiTableManager.instance.SetOrderUp(index, true); //이 테이블에 음식 놓을 수 있음
        
    }
    public void Order()
    {
        chatBubble_main.SetActive(true); //말풍선 키기
        chatBubble_main.transform.GetChild(mMenu).gameObject.SetActive(true); //메인메뉴 번호따라 자식 오브젝트 키기
        if (subOn)
        {
            chatBubble_sub.SetActive(true);
            chatBubble_sub.transform.GetChild(sMenu).gameObject.SetActive(true); //서브메뉴 번호따라 자식 오브젝트 키기
        }
        if (drinkOn)
        {
            chatBubble_drink.SetActive(true);
            chatBubble_drink.transform.GetChild(dMenu).gameObject.SetActive(true); //드링크메뉴 번호따라 자식 오브젝트 키기
        }
        SoundManager.instance.PlaySound3D("blabla", transform);
        //GetComponent<AudioSource>().enabled = true; //웅얼웅얼 오디오 키기
    }
}
