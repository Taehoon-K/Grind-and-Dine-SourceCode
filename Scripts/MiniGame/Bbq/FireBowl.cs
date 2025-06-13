using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireBowl : Bowl
{
    [Header("Fire")]
    public GameObject burnt; //ź ����
    [SerializeField]
    private float cook;  // �ִ� ���� ����. ����Ƽ ������ ���Կ��� ������ ��.
    private float currentCook;
    [SerializeField]
    private float burn;  // �ִ� Ž ����.
    private float currentBurn;
    [SerializeField]
    private Image images_Gauge; //�丮���ΰ� ���� ������
    [SerializeField]
    private GameObject images_Cook; //�丮���϶� Ű�� ������ �̹���
    [SerializeField] private float gaugeFillSpeed; //�丮 �ӵ�
    [SerializeField] private GameObject steamFx; //�� fx
    [SerializeField] private GameObject fireFx;  //�ҳ����� fx
    [SerializeField] private Color normalColor; // ������ �⺻ ����
    [SerializeField] private Color dangerColor; // �������� �������� ����
    [SerializeField] private float burnTimeThreshold = 2.0f; // Ž ���¿��� �ҳ��� ���·� ��ȯ�Ǳ� �������� �ð�
    [SerializeField] private GameObject cookFx; //�������̶� ���� �Ҹ� ���� ������Ʈ

    [Header("Extinguish")] //�Ҳ���
    [SerializeField]
    private float extinguish;  // ��ȭ���� �ɸ��� �ð�.
    private float currentExtinguisher;

    private bool isCook; //�丮������
    public bool beCooked; //�;�����, �;����� Ʈ���� ���ø�
    public bool beBurnt; //������ ����
    public bool isBurning; //�ҳ�������
    private float burnTimer; // Ÿ�� �����ϱ� �������� �ð� ����

    public bool IsCook
    {
        get { return isCook; }
        set
        {
            isCook = value;
            images_Cook.SetActive(value); //������ Ʈ��� ������ ���̰�
            if (!isCook)
            {
            }
            else
            {
            }
        }
    }
    protected override void Update()
    {
        base.Update();
        CookGuage();
    }
    public override void Pickup(int id)
    {
        if (beCooked)
        {
            return;
        }
        base.Pickup(id);
    }
    private void CookGuage()
    {
        if (IsCook)
        {
            if (!cookFx.activeSelf) //��Ȱ��ȭ���
            {
                cookFx.SetActive(true); //Ȱ��ȭ
            }

            if (currentCook < cook)
            {
                // ���� ����: �������� ä��
                float fillAmount = gaugeFillSpeed * Time.deltaTime;
                currentCook += fillAmount;
                currentCook = Mathf.Clamp(currentCook, 0f, cook);
                images_Gauge.fillAmount = currentCook / cook;

                // �������� �׻� �ʷϻ� ����
                images_Gauge.color = normalColor;
            }
            else // �丮�� �� �;��� ��
            {
                beCooked = true;

                if (GetComponent<AudioSource>().enabled == false)
                {
                    GetComponent<AudioSource>().enabled = true; //Ÿ�̸� �Ҹ�
                }

                if (currentBurn < burn)
                {
                    // Ž ����: ������ ������ ���� ���������� ��ȭ, ���� FX �ѱ�
                    steamFx.SetActive(true);

                    float fillAmount = gaugeFillSpeed * Time.deltaTime;
                    currentBurn += fillAmount;
                    currentBurn = Mathf.Clamp(currentBurn, 0f, burn);

                    // �������� �ʰ����� ǥ��
                    images_Gauge.fillAmount = 1f;

                    // ���� ���¿������� ������ ���� ���� (�ʷ� -> ����)
                    images_Gauge.color = Color.Lerp(normalColor, dangerColor, currentBurn / burn);

                    
                }
                else //�� �������� ����
                {
                    beBurnt = true;
                    burnt.SetActive(true); //ź ������Ʈ Ȱ��ȭ
                    for (int i = 0; i < ingredient.Length; i++)
                    {
                        ingredient[i] = false; //���� ���� �� ����
                    }
                    burnTimer += Time.deltaTime;

                    if(burnTimer >= burnTimeThreshold + 20f) //���� Ÿ�� 10�� ������
                    {
                        //������ �׿���
                    }
                    else if (burnTimer >= burnTimeThreshold)
                    {
                        // �ҳ��� ���·� ��ȯ, �� FX �ѱ�
                        isBurning = true;
                        fireFx.SetActive(true);
                        // �ҳ��� ���¿��� �� �̻� ���°� ������ �ʵ��� �� �� ����
                    }
                }
            }
        }
        else
        {
            if (cookFx.activeSelf) //�������̶� �����Ҹ� Ȱ��ȭ���
            {
                cookFx.SetActive(false); //��Ȱ��ȭ
            }

            // �丮 ���� �ƴ� �� �ʱ�ȭ (�ʿ��� ���)
            if (GetComponent<AudioSource>().enabled == true)
            {
                GetComponent<AudioSource>().enabled = false;
            }

            burnTimer = 0f;
            //steamFx.SetActive(false);
            //fireFx.SetActive(false);
        }
    }
    public void PutOutFire() //�Ҳ����Լ�
    {
        if (isBurning) //��Ÿ���ְ� ��ȭ�� ���������
        {
            float fillAmount = gaugeFillSpeed * Time.deltaTime;
            currentExtinguisher += fillAmount;
            if (currentExtinguisher >= extinguish) //���� ��ȭ �ð� ä���
            {
                //�� ������
                isBurning = false;
                fireFx.SetActive(false);
            }

        } 
    }
}