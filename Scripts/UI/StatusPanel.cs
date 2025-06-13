using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject[] screen;
    private bool firstPanel;
    public bool FirstPanel { get { return firstPanel; } 
        set { firstPanel = value; 
            screen[0].SetActive(value);
            screen[1].SetActive(!value);
        } }  //값을 변경하면 애니메이터 값도 자동으로 변경되도록
    [SerializeField]
    private Image[] statusImage;

    [SerializeField]
    private TextMeshProUGUI[] statusCurrent;

    //경험치 양 이미지
    [SerializeField]
    private Image[] skillBar;
    [SerializeField]
    private TextMeshProUGUI[] skillLevel;

    [SerializeField]
    private TextMeshProUGUI[] mood;

    [SerializeField]
    private GameObject[] moodle;

    [SerializeField]
    private EmotionChangeTooltip emotionTooltip;


    private float hp, hungry, fatigue;
    private int angry, despre, boredom;


    public Transform point; // 점의 위치를 표시할 Transform
    public Transform topVertex; // Top vertex of the triangle
    public Transform leftVertex; // Left vertex of the triangle
    public Transform rightVertex; // Right vertex of the triangle

    public float varA; // 첫 번째 변수 (topVertex에 해당)
    public float varB; // 두 번째 변수 (leftVertex에 해당)
    public float varC; // 세 번째 변수 (rightVertex에 해당)
    private void OnEnable() 
    {
        Render();

    }
    private void Render() //렌더링 시
    {
        hp = (float)StatusManager.instance.GetStatus().currentHp / 1000;
        hungry = (float)StatusManager.instance.GetStatus().currentHungry / 1000;
        fatigue = (float)StatusManager.instance.GetStatus().currentFatigue / 1000;
        statusImage[0].fillAmount = hp;
        statusImage[1].fillAmount = fatigue;
        statusImage[2].fillAmount = hungry;

        statusCurrent[0].text = ((int)(hp * 100)).ToString();
        statusCurrent[1].text = ((int)(fatigue * 100)).ToString();
        statusCurrent[2].text = ((int)(hungry * 100)).ToString();

        angry = StatusManager.instance.GetStatus().currentAngry;
        despre = StatusManager.instance.GetStatus().currentSadness;
        boredom = StatusManager.instance.GetStatus().currentBoredom;
        mood[0].text = angry / 10 + "/100";
        mood[1].text = despre / 10 + "/100";
        mood[2].text = boredom / 10 + "/100";;
        FirstPanel = true;

        for (int i = 0; i < skillBar.Length; i++)
        {
            skillBar[i].fillAmount = StatusManager.instance.GetExperienceProgress(i);
            skillLevel[i].text = StatusManager.instance.GetStatus().level[i].ToString();
        }
        RenderMoodle();

        emotionTooltip.Render(UpdateEmotionImage());

        StatusManager.instance.UpdatePerkStates(); //퍽 레벨 상태 업데이트
    }
    private void RenderMoodle() //상태이상 로드
    {
        Moodle[] aa = StatusManager.instance.GetMoodle();
        for (int i = 0; i < moodle.Length; i++)
        {
            moodle[i].SetActive(aa[i].isActive);
        }
    }

    private int UpdateEmotionImage() //스탯에 따라 무슨 감정인지 리턴하는 함수
    {
        Status status = StatusManager.instance.GetStatus();
        if (status.currentAngry <= 500 && status.currentSadness <= 500 && status.currentBoredom <= 500)
        {
            return 0;
        }
        else
        {
            // 500 이상인 값 중 가장 큰 값을 찾고 그에 해당하는 인덱스 활성화
            if (status.currentAngry >= status.currentSadness && status.currentAngry >= status.currentBoredom)
            {
                if (status.currentAngry >= 750)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            else if (status.currentSadness >= status.currentAngry && status.currentSadness >= status.currentBoredom)
            {
                if (status.currentSadness >= 750)
                {
                    return 4;
                }
                else
                {
                    return 3;
                }
            }
            else if (status.currentBoredom >= status.currentAngry && status.currentBoredom >= status.currentSadness)
            {
                if (status.currentBoredom >= 750)
                {
                    return 6;
                }
                else
                {
                    return 5;
                }
            }

            return 0;
        }
    }
}
