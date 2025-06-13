using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : InteractableItem
{
    public bool[] ingredient;
    public GameObject[] ingredients; //음식 재료들

    public Vector3 savedPosition;
    public Quaternion savedRotation;
    public Vector3 savedScale;

    protected virtual void Update()
    {
        // 배열 크기가 같다고 가정
        for (int i = 0; i < ingredient.Length; i++)
        {
            if (ingredient[i])
            {
                // 해당 인덱스의 게임 오브젝트를 활성화
                ingredients[i].SetActive(true);
            }
            else
            {
                // 해당 인덱스의 게임 오브젝트를 비활성화
                ingredients[i].SetActive(false);
            }
        }
    }
    public override void Pickup() //쓰레기 버리는 함수
    {
        Destroy(gameObject);
        /*
        // 모든 ingredient가 false인지 확인하는 변수
        bool allFalse = true;

        // ingredient 배열을 순회하면서 모든 값이 false인지 확인
        for (int i = 0; i < ingredient.Length; i++)
        {
            if (ingredient[i])
            {
                allFalse = false;
                break; // 하나라도 true가 있으면 더 이상 확인할 필요가 없음
            }
        }

        if (allFalse)
        {
            // 모든 값이 false라면 게임 오브젝트를 파괴
            Destroy(gameObject);
        }
        else
        {
            // 그렇지 않으면 모든 값을 false로 변경
            for (int i = 0; i < ingredient.Length; i++)
            {
                ingredient[i] = false;
            }
        }*/
    }
    public virtual void Pickup(int id) //재료 누를시 호출됨
    {
        if (id < 0)
        {
            return;
        }
        else if(id == 2 && (ingredient[3] || ingredient[4]))
        {
            return;
        }
        else if (id == 3 && (ingredient[2] || ingredient[4]))
        {
            return;
        }
        else if (id == 4 && (ingredient[3] || ingredient[2]))
        {
            return;
        }
        else if (ingredient[0] && id == 1)
        {
            return;
        }
        else if (ingredient[1] && id == 0)
        {
            return;
        }
        //해당 재료 담기는 코드
        ingredient[id] = true;
    }

    public void CleanIngre() //재료 싹 없애는 함수
    {
        for (int i = 0; i < ingredient.Length; i++)
        {
            ingredient[i] = false;
        }
    }
}
