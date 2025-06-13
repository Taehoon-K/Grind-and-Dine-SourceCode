using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CoffeeBowl : Bowl
{
    // TableState 객체를 포함하는 리스트 초기화
    private List<TableState> coffeeMenu = new List<TableState>
    {                              //언더샷 오버샷 우유   얼음  딸시   초시   카시    크림  블렌딩
        new TableState(new bool[8] { true, false, false, true, false, false, false, false}, false), //아메리카노
        new TableState(new bool[8] { false, false, true, true, true, false, false, true}, true),
        new TableState(new bool[8] { false, false, true, true, false, true, false, true}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, false, true}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, true, true}, true), //카라멜 프랍
        new TableState(new bool[8] { false, false, true, true, true, false, false, false}, false),
        new TableState(new bool[8] { false, false, true, true, false, true, false, false}, false),
        new TableState(new bool[8] { false, true, true, true, false, false, false, false}, false),
        new TableState(new bool[8] { false, true, true, true, false, false, true, false}, false),  //카라멜 마끼
                                
        new TableState(new bool[8] { false, false, true, true, true, false, false, false}, true), //프랍 크림 전단계들
        new TableState(new bool[8] { false, false, true, true, false, true, false, false}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, false, false}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, true, false}, true), 
    };

    [SerializeField]
    private bool mix = false; // 블렌딩 여부를 나타냄

    [SerializeField]
    private GameObject[] mixResult; //완성된 메뉴 생김새 넣을것
    [SerializeField]
    private GameObject mixBad; //섞었을때 망했을시
    [SerializeField]
    private GameObject straw; //완성 시 컵뚜껑이랑 빨대 등

    private bool isCombinationActive = false; // 현재 조합이 활성 상태인지 확인

    protected override void Update()
    {
        if (!isCombinationActive && !mix) //만약 완성조합 상태가 아니고 mix도 아니라면
        {
            // 배열 크기가 같다고 가정
            for (int i = 0; i < ingredient.Length; i++)
            {
                if (ingredient[i])
                {
                    ingredients[i].SetActive(true);
                }
                else
                {
                    ingredients[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < ingredient.Length; i++)
            {
                ingredients[i].SetActive(false);
            }
        }
        
        // 현재 상태가 메뉴와 일치하는지 확인하고 결과 활성화
        CheckAndActivateMixResult();
    }

    private void CheckAndActivateMixResult()
    {
        for (int i = 0; i < coffeeMenu.Count; i++)
        {
            var menu = coffeeMenu[i];
            if (IsMatch(menu))
            {
                ActivateMixResult(i);
                return;
            }
        }

        if (mix) //만약 섞은 상태라면
        {
            if (mixBad.activeSelf == false)
            {
                mixBad.SetActive(true);
            }
        }
        else
        {
            // 조건에 맞는 조합이 없으면 mixResult 비활성화
            DeactivateMixResult();
            if(mixBad.activeSelf == true)
            {
                mixBad.SetActive(false);
            }
        }
        
    }
    private bool IsMatch(TableState menu)
    {
        if (mix != menu.isCook) return false; // 믹스 상태가 다르면 일치하지 않음

        for (int i = 0; i < ingredient.Length; i++)
        {
            if (ingredient[i] != menu.ingre[i])
                return false; // 하나라도 일치하지 않으면 false
        }

        return true; // 모두 일치하면 true
    }

    private void ActivateMixResult(int index)
    {
        isCombinationActive = true;
        for (int i = 0; i < mixResult.Length; i++)
        {
            mixResult[i].SetActive(i == index); // 해당 메뉴에 맞는 결과만 활성화
        }
    }

    private void DeactivateMixResult()
    {
        isCombinationActive = false;
        foreach (var result in mixResult)
        {
            result.SetActive(false);
        }
    }

    public void SetMix(bool value)
    {
        mix = value;
    }
    public bool GetMix()
    {
        return mix;
    }
    public override void Pickup() //쓰레기 버리는 함수
    {
        Destroy(gameObject);
    }
    public override void Pickup(int id) //재료 누를시 호출됨
    {
        if(id == 2 && ingredient[0]) //만약 우유 클릭시, 그리고 샷 있을시 오버샷으로 변경
        {
            ingredient[0] = false;
            ingredient[1] = true;
        }
        if(id == 0 && ingredient[2]) //우유 있을때 샷 추가시
        {
            ingredient[1] = true;
            return;  //오버샷 추가후 종료
        }
        if (id < 0)
        {
            return;
        }
        else if (id <= 6 && mix) //섞은상태고 크림 빼고면 추가 안됨
        {
            //나중에 ui 띄우기
            return;
        }
        else if (id == 4 && (ingredient[5] || ingredient[6]))
        {
            return;
        }
        else if (id == 5 && (ingredient[4] || ingredient[6]))
        {
            return;
        }
        else if (id == 6 && (ingredient[5] || ingredient[4]))
        {
            return;
        }
        else if (id == 7 && !mix )
        {
            return;
        }
        else if (id == 9)
        {
            SetMix(true);
            SoundManager.instance.PlaySound2D("blender");
            return;
        }
        //해당 재료 담기는 코드
        ingredient[id] = true;
    }
}
