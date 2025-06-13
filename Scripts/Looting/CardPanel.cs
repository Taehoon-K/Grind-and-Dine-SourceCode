using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : UiPrompt
{
    [Header("Card")]
    [SerializeField] private bool[] card;
    [SerializeField]
    protected GameObject[] cardObject; //6개의 카드

    [SerializeField] private Sprite[] sprites; //각 상황별 그림
    [SerializeField] private Sprite noneCard; //아무것도 없는 앞면 그림

    [SerializeField] private float startX = 0.25f; //카드 젤 왼쪽
    [SerializeField] private float endX = 0.55f;   //젤 오른쪽

    private int badCount; //걸리는 카드 갯수
    private System.Action<bool> onCardResultCallback; //다른 함수에서 호출할때 bool 되돌려줄 용도
    private int currentSituation; //무슨 상황인지
    private bool isCatch; //걸렸는지 여부

    public void PanelOn(int whatSituation, System.Action<bool> onResult,bool isCrimial = false) //패널 켜질때 호출
    {
        currentSituation = whatSituation;
        onCardResultCallback = onResult;

        int cardNum = Mathf.Max((4 - PlayerStats.Difficulty), 2);  //총 카드 개수 구하기
        card = new bool[cardNum]; //배열 길이 바꾸기

        for (int i = 0; i < cardObject.Length; i++) //카드 갯수 이상인 카드 오브젝트 비활성화
        {
            if (i >= cardNum && cardObject[i] != null)
            {
                cardObject[i].SetActive(false);
            }
            else
            {
                cardObject[i].SetActive(true);
            }
        }

        for (int i = 0; i < cardNum; i++) //카드 내려놓는 위치 재정렬
        {
            float normalizedX = (cardNum == 1) ? 0.5f : i / (float)(cardNum - 1);
            float posX = Mathf.Lerp(startX, endX, normalizedX);

            cardObject[i].GetComponent<UIFixedAnimation>().finalV3.x = posX;
        }

        if (isCrimial) //범죄라면
        {
            badCount = Mathf.Min(StatusManager.instance.GetCrimianl(), cardNum); //안좋은 카드 갯수 구하기
            if (badCount == 0)
            {
                badCount = 1; //최소 1로 맞추기
            }

            if (StatusManager.instance.GetSelectedPerk(2, 1) == 0) //만약 퍽 찍었다면
            {
                badCount /= 2; //확률 절반 감소
            }
        }
        else //범죄 아니라면
        {
            badCount = 1;
        }

        for (int i = 0; i < cardNum; i++) //초기화
        {
            card[i] = false;
        }
        // badCount 값만큼 트루 넣기
        for (int i = 0; i < badCount; i++)
        {
            card[i] = true;
        }

        // 위치를 섞는 방식, 배열을 무작위로 섞기
        card = card.OrderBy(_ => Random.value).ToArray();

        for (int i = 0; i < card.Length; i++)
        {
            if (card[i]) //만약 해당 카드가 걸리는 카드면
            {
                cardObject[i].GetComponent<Card>().frontCard = sprites[currentSituation];  //카드 앞면 채워넣기
            }
            else
            {
                cardObject[i].GetComponent<Card>().frontCard = noneCard;  //카드 앞면 채워넣기
            }
        }
        Invoke(nameof(ButtonOn), cardNum*0.5f); //3초 뒤에 버튼 켜지기 실행
    }

    public void ClickCard(int index) //카드 선택시 호출되는 함수
    {
        isCatch = card[index]; //당첨 카드인지 여부

        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Button>().interactable = false;  //모든 카드 비활성화
            if (i != index && cardObject[i].activeSelf)
            {
                cardObject[i].GetComponent<Card>().AfterRotate(); //나머지 카드 돌리기
            }
        }

        Invoke(nameof(PanelOff), 2f); //3초 뒤에 버튼 켜지기 실행
        
    }

    public void PanelOff() //카드 패널 꺼지는 함수, 확인 버튼이랑 연결할것
    {
        // 결과 콜백 실행
        onCardResultCallback?.Invoke(isCatch);
        gameObject.SetActive(false); //카드 패널 꺼지기
    }
    protected void ButtonOn() //버튼 켜지는 함수
    {
        for (int i = 0; i < cardObject.Length; i++)
        {
            cardObject[i].GetComponent<Button>().interactable = true;  //카드 버튼 활성화
        }

    }
}
