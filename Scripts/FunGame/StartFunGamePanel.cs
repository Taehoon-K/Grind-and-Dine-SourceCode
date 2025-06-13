using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UIManager;

public class StartFunGamePanel : UiPrompt
{
    [Header("Scene")]
    [SerializeField]
    private SceneTransitionManager.Location locationToSwitch;

    /*[Header("Others")]
    [SerializeField] private Button leftButton;  // 왼쪽 버튼
    [SerializeField] private Button rightButton; // 오른쪽 버튼
    [SerializeField]
    private TextMeshProUGUI wageText;   // 돈 값을 표시할 Text UI*/

    [SerializeField] private Simulscene simulscene; //시뮬레이션 시 띄울 시뮬씬
    [SerializeField] private GameObject simulButton; //시뮬레이션 버튼, 배팅 돈 0일때만 활성화

    private readonly int[] moneyValues = { 0, 5000, 10000, 20000, 30000, 40000, 50000, 100000 }; // 가능한 돈 값
    private int currentIndex = 0; // 현재 선택된 돈 값의 인덱스
    

    public void Confirm() //확인 버튼 누를시 호출될 함수
    {
      /*  int selectedMoney = moneyValues[currentIndex];
        PlayerPrefs.SetInt("SelectedMoney", selectedMoney); //건 돈 저장*/
        //씬 이동
        SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);
    }
    public void Simulate() //시뮬 버튼 누를시 호출될 함수
    {
        CutsceneManager.Instance.StartSimulateScene(simulscene); //시뮬씬 실행

    }
    private void Start()
    {
        // 초기 값 설정
        //UpdateMoneyText();

       /* // 버튼에 이벤트 추가
        leftButton.onClick.AddListener(DecreaseMoney);
        rightButton.onClick.AddListener(IncreaseMoney);*/
    }
    protected override void OnEnable()
    {
        base.OnEnable();
       /* currentIndex = 0;
        UpdateMoneyText();*/
    }

    /*private void UpdateMoneyText()
    {
        // 돈 값을 업데이트
        wageText.text = moneyValues[currentIndex].ToString("N0") + "\u20A9"; // "1,000원" 형식으로 표시
        
        if (currentIndex == 0) //돈 안걸었을때
        {
            simulButton.SetActive(true);
        }
        else
        {
            simulButton.SetActive(false);
        }
    }
    
    private void DecreaseMoney()
    {
        // 왼쪽 버튼 클릭 시 돈 값 감소
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateMoneyText();
        }
    }

    private void IncreaseMoney()
    {
        // 오른쪽 버튼 클릭 시 돈 값 증가
        if (currentIndex < moneyValues.Length - 1)
        {
            if (moneyValues[currentIndex + 1] <= PlayerStats.GetMoney()) //현재 돈보다 작은 값만 설정 가능
            {
                currentIndex++;
                UpdateMoneyText();
            }
        }
    }*/
}
