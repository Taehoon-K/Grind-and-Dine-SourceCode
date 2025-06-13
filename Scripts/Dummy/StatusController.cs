using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // �Ƿε�
    [SerializeField]
    private int hp;  // �ִ� �Ƿε�. ����Ƽ ������ ���Կ��� ������ ��.
    private int currentHp;

    // �Ƿε��� �پ��� �ӵ�
    [SerializeField]
    private float hpDecreaseTime;
    private float currentHpDecreaseTime;

    // ���¹̳�
    [SerializeField]
    private int sp;  // �ִ� ���¹̳�. ����Ƽ ������ ���Կ��� ������ ��.
    private int currentSp;

    // ���¹̳� ������
    [SerializeField]
    private int spIncreaseSpeed;

    // ���¹̳� ��ȸ�� ������ �ð�
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // ���¹̳� ���� ����
    private bool spUsed;


    // �����
    [SerializeField]
    private int hungry;  // �ִ� �����. ����Ƽ ������ ���Կ��� ������ ��.
    private int currentHungry;

    // ������� �پ��� �ӵ�
    [SerializeField]
    private float hungryDecreaseTime;
    private float currentHungryDecreaseTime;


    // ������
    [SerializeField]
    private int stress;  // �ִ� ��Ʈ����. ����Ƽ ������ ���Կ��� ������ ��.
    private int currentStress;

    // �ʿ��� �̹���
    [SerializeField]
    private Image[] images_Gauge;

    // �� ���¸� ��ǥ�ϴ� �ε���
    private const int HP = 0, SP = 1, HUNGRY = 2, STRESS = 3;

    private int hungry_state=0;
    private int hungry_amount;
    private int stress_state = 0;
    private int stress_amount;
    public bool stamina_state = false;

    float tt; //Ÿ�ӽ����Ͽ�
    float tt_stre,tt_sp,tt_spRecover;

    void Start()
    {
        currentHp = hp;
        currentSp = sp;
        currentHungry = hungry;
        currentStress = 0;
    }

    void Update()
    {
        Hp();
        Hungry();
        Stress();
        SPRechargeTime();
        SPRecover();
        GaugeUpdate();
        DecreaseStamina();
    }

    private void GaugeUpdate()
    {
        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[STRESS].fillAmount = (float)currentStress / stress;
        if (images_Gauge[HP].fillAmount <= 0.2)
        {
            images_Gauge[HP].color = Color.red;
        }
        else
        {
            images_Gauge[HP].color = Color.white;
        }

        if (images_Gauge[HUNGRY].fillAmount <= 0.2)
        {
            images_Gauge[HUNGRY].color = Color.red;
        }
        else
        {
            images_Gauge[HUNGRY].color = Color.white;
        }

        if (images_Gauge[STRESS].fillAmount >= 0.8)
        {
            images_Gauge[STRESS].color = Color.red;
        }
        else
        {
            images_Gauge[STRESS].color = Color.white;
        }
    }

    private void Hungry()
    {
        if (hungry_state == 0)
        {
            if (currentHungry > 0)
            {
                if (currentHungryDecreaseTime <= hungryDecreaseTime)
                    currentHungryDecreaseTime+=Time.deltaTime;
                else
                {
                    currentHungry--;
                    currentHungryDecreaseTime -= hungryDecreaseTime;
                }
            }
            //else
                 //���߿� ���� �߰� �������� ���� �� ����, ������ 3�� �����ϸ� ���
        }
        else if(hungry_state == 1)//�Դ���
        {
            tt += 0.02f;
            if (tt < hungry_amount&&currentHungry < hungry)
            {
                currentHungry++;
            }
            else
            {
                hungry_state = 0;
                tt = 0;
            }
        }
    }

    private void Hp()
    {
        if (currentHp > 0)
        {
            if (currentHpDecreaseTime <= hpDecreaseTime)
                currentHpDecreaseTime+=Time.deltaTime;
            else
            {
                currentHp--;
                currentHpDecreaseTime = 0;
            }
        }
        else
            Debug.Log("�Ƿε� ��ġ�� 0 �� �Ǿ����ϴ�.");
    }
    public void IncreaseHP(int _count)
    {
        if (currentHp + _count < hp)
            currentHp += _count;
        else
            currentHp = hp;
    }

    public void DecreaseHP(int _count)
    {
        /*if (currentDp > 0)
        {
            DecreaseDP(_count);
            return;
        }*/
        currentHp -= _count;

        if (currentHp <= 0)
            Debug.Log("ĳ������ ü���� 0�� �Ǿ����ϴ�!!");
    }

    public void IncreaseHungry(int _count)
    {
        hungry_state = 1; // �Դ»���
        hungry_amount=_count;
    }
    public void DecreaseHungry(int _count)
    {
        if (currentHungry - _count < 0)
            currentHungry = 0;
        else
            currentHungry -= _count;
    }

    private void DecreaseStamina()
    {       
        if (stamina_state == true)//�޸�����
        {
            spUsed = true;
            currentSpRechargeTime = 0;

            tt_sp += Time.deltaTime;
            if (tt_sp > 0.05f && currentSp > 0)
            {
                currentSp-=8;
                tt_sp = 0;
            }
            else
            {
                stamina_state = false;
            }
        }
        
    }
    public void DecreaseStamina2(int _count)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - _count > 0)
        {
            currentSp -= _count;
        }
        else
            currentSp = 0;
    }
    private void SPRechargeTime()
    {
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime++;
            else
                spUsed = false;
        }
    }
    private void SPRecover()
    {
        if (!spUsed && currentSp < sp)
        {
            tt_spRecover += Time.deltaTime;
            if (tt_spRecover >= 0.05f)
            {
                currentSp += spIncreaseSpeed;
                tt_spRecover -= 0.05f;
            }        
        }
 
    }
    public int GetCurrentSP()
    {
        return currentSp;
    }
    private void Stress()
    {
        if (stress_state == 1)//�þ
        {
            tt_stre += 0.02f;
            if (tt_stre < stress_amount && currentStress < stress)
            {
                currentStress++;
            }
            else
            {
                stress_state = 0;
                tt_stre = 0;
            }
        }
        else if (stress_state == 2)//�پ��
        {
            tt_stre += 0.02f;
            if (tt_stre < stress_amount && currentStress >0)
            {
                currentStress--;
            }
            else
            {
                stress_state = 0;
                tt_stre = 0;
            }
        }
    }
    public void IncreaseStress(int _count)
    {
        stress_state = 1; // ��Ʈ���� �޴»���
        stress_amount = _count;
    }
    public void DecreaseStress(int _count)
    {
        stress_state = 2; // ��Ʈ���� �پ��»���
        stress_amount = _count;
    }
}