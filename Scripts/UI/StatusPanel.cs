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
        } }  //���� �����ϸ� �ִϸ����� ���� �ڵ����� ����ǵ���
    [SerializeField]
    private Image[] statusImage;

    [SerializeField]
    private TextMeshProUGUI[] statusCurrent;

    //����ġ �� �̹���
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


    public Transform point; // ���� ��ġ�� ǥ���� Transform
    public Transform topVertex; // Top vertex of the triangle
    public Transform leftVertex; // Left vertex of the triangle
    public Transform rightVertex; // Right vertex of the triangle

    public float varA; // ù ��° ���� (topVertex�� �ش�)
    public float varB; // �� ��° ���� (leftVertex�� �ش�)
    public float varC; // �� ��° ���� (rightVertex�� �ش�)
    private void OnEnable() 
    {
        Render();

    }
    private void Render() //������ ��
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

        StatusManager.instance.UpdatePerkStates(); //�� ���� ���� ������Ʈ
    }
    private void RenderMoodle() //�����̻� �ε�
    {
        Moodle[] aa = StatusManager.instance.GetMoodle();
        for (int i = 0; i < moodle.Length; i++)
        {
            moodle[i].SetActive(aa[i].isActive);
        }
    }

    private int UpdateEmotionImage() //���ȿ� ���� ���� �������� �����ϴ� �Լ�
    {
        Status status = StatusManager.instance.GetStatus();
        if (status.currentAngry <= 500 && status.currentSadness <= 500 && status.currentBoredom <= 500)
        {
            return 0;
        }
        else
        {
            // 500 �̻��� �� �� ���� ū ���� ã�� �׿� �ش��ϴ� �ε��� Ȱ��ȭ
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
