using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // 피로도
    [SerializeField]
    private int hp;  // 최대 피로도. 유니티 에디터 슬롯에서 지정할 것.
    private int currentHp;

    // 피로도가 줄어드는 속도
    [SerializeField]
    private float hpDecreaseTime;
    private float currentHpDecreaseTime;

    // 스태미나
    [SerializeField]
    private int sp;  // 최대 스태미나. 유니티 에디터 슬롯에서 지정할 것.
    private int currentSp;

    // 스태미나 증가량
    [SerializeField]
    private int spIncreaseSpeed;

    // 스태미나 재회복 딜레이 시간
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // 스태미나 감소 여부
    private bool spUsed;


    // 배고픔
    [SerializeField]
    private int hungry;  // 최대 배고픔. 유니티 에디터 슬롯에서 지정할 것.
    private int currentHungry;

    // 배고픔이 줄어드는 속도
    [SerializeField]
    private float hungryDecreaseTime;
    private float currentHungryDecreaseTime;


    // 만족감
    [SerializeField]
    private int stress;  // 최대 스트레스. 유니티 에디터 슬롯에서 지정할 것.
    private int currentStress;

    // 필요한 이미지
    [SerializeField]
    private Image[] images_Gauge;

    // 각 상태를 대표하는 인덱스
    private const int HP = 0, SP = 1, HUNGRY = 2, STRESS = 3;

    private int hungry_state=0;
    private int hungry_amount;
    private int stress_state = 0;
    private int stress_amount;
    public bool stamina_state = false;

    float tt; //타임스케일용
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
                 //나중에 기절 추가 돈있으면 기절 후 병원, 돈없고 3번 기절하면 사망
        }
        else if(hungry_state == 1)//먹는중
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
            Debug.Log("피로도 수치가 0 이 되었습니다.");
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
            Debug.Log("캐릭터의 체력이 0이 되었습니다!!");
    }

    public void IncreaseHungry(int _count)
    {
        hungry_state = 1; // 먹는상태
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
        if (stamina_state == true)//달리는중
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
        if (stress_state == 1)//늘어남
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
        else if (stress_state == 2)//줄어듬
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
        stress_state = 1; // 스트레스 받는상태
        stress_amount = _count;
    }
    public void DecreaseStress(int _count)
    {
        stress_state = 2; // 스트레스 줄어드는상태
        stress_amount = _count;
    }
}