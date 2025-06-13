using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CookingPot : Bowl
{
    [Header("Fire")]
    public GameObject burnt; //탄 상태
    [SerializeField]
    private float cook;  // 최대 익힘 상태. 유니티 에디터 슬롯에서 지정할 것.
    private float currentCook;
    [SerializeField]
    private float burn;  // 최대 탐 상태.
    private float currentBurn;
    [SerializeField]
    private Image images_Gauge; //요리중인거 위에 게이지
    [SerializeField]
    private GameObject images_Cook; //요리중일때 키는 게이지 이미지
    [SerializeField] private float gaugeFillSpeed; //요리 속도
    [SerializeField] private GameObject steamFx; //김 fx
    [SerializeField] private GameObject fireFx;  //불났을시 fx
    [SerializeField] private Color normalColor; // 게이지 기본 색상
    [SerializeField] private Color dangerColor; // 게이지가 빨개지는 색상
    [SerializeField] private float burnTimeThreshold = 2.0f; // 탐 상태에서 불나는 상태로 전환되기 전까지의 시간

    [SerializeField] private GameObject cookFx; //가스불이랑 가스 소리 담은 오브젝트

    [Header("Extinguish")] //불끄기
    [SerializeField]
    private float extinguish;  // 소화까지 걸리는 시간.
    private float currentExtinguisher;

    private bool isCook; //요리중인지
    public bool isBurning; //불나고있음
    private int ids;
    private float burnTimer; // 타기 시작하기 전까지의 시간 측정

    public bool IsCook
    {
        get { return isCook; }
        set
        {
            isCook = value;
            images_Cook.SetActive(value); //이즈쿡 트루면 게이지 보이게
            cookFx.SetActive(value); //활성화
        }
    }
    protected override void Update()
    {
        base.Update();
        CookGuage();
    }
    public override void Pickup(int id)
    {
        //IsCook = ingredient.Any(x => x); // ingredient 배열 중 하나라도 true이면 result는 true가 됩니다.
        if (IsCook)
        {
            return;
        }
        ingredient[id] = true;
        IsCook = true;
        ids = id;
    }
    public int Pickout() //재료 내보내는 함수
    {
        IsCook = false;
        int index = ingredient.ToList().FindIndex(x => x); //트루인 재료 id 내보내기
        ingredient[index] = false;
        return index;

    }
    private void CookGuage()
    {
        if (IsCook)
        {
            if (currentCook < cook)
            {
                // 익힘 상태: 게이지를 채움
                float fillAmount = gaugeFillSpeed * Time.deltaTime;
                currentCook += fillAmount;
                currentCook = Mathf.Clamp(currentCook, 0f, cook);
                images_Gauge.fillAmount = currentCook / cook;

                // 게이지는 항상 초록색 유지
                images_Gauge.color = normalColor;
            }
            else // 요리가 다 익었을 때
            {
                CookDone(ids);

                if (GetComponent<AudioSource>().enabled == false)
                {
                    GetComponent<AudioSource>().enabled = true; //타이머 소리
                }

                if (currentBurn < burn)
                {
                    // 점점 타가는 상태: 게이지 색상을 점점 빨간색으로 변화, 연기 FX 켜기
                    steamFx.SetActive(true);

                    float fillAmount = gaugeFillSpeed * Time.deltaTime;
                    currentBurn += fillAmount;
                    currentBurn = Mathf.Clamp(currentBurn, 0f, burn);

                    // 게이지가 초과됨을 표현
                    images_Gauge.fillAmount = 1f;

                    // 익힌 상태에서부터 게이지 색상 변경 (초록 -> 빨강)
                    images_Gauge.color = Color.Lerp(normalColor, dangerColor, currentBurn / burn);


                }
                else //이 순간부터 탔음
                {
                    burnt.SetActive(true); //탄 오브젝트 활성화
                    for (int i = 0; i < 4; i++)
                    {
                        ingredient[i] = false; //원래 재료들 다 끄기
                    }
                    ingredient[4] = true; //탄 오브젝트 활성화
                    burnTimer += Time.deltaTime;

                    if (burnTimer >= burnTimeThreshold + 20f) //만약 타고도 10초 지나면
                    {
                        //터지고 겜오버
                    }
                    else if (burnTimer >= burnTimeThreshold)
                    {
                        // 불나는 상태로 전환, 불 FX 켜기
                        isBurning = true;
                        fireFx.SetActive(true);
                        // 불나는 상태에서 더 이상 상태가 변하지 않도록 할 수 있음
                    }
                }
            }
        }
        else
        {
            // 요리 중이 아닐 때 초기화 (필요한 경우)
            burnTimer = 0f;
            currentBurn = 0f;
            currentCook = 0f;
            burnt.SetActive(false);
            steamFx.SetActive(false);
            //fireFx.SetActive(false);

            if (GetComponent<AudioSource>().enabled == true)
            {
                GetComponent<AudioSource>().enabled = false; //타이머 소리
            }
        }
    }
    public void PutOutFire() //불끄기함수
    {
        if (isBurning) //불타고있고 소화기 쏘고있을시
        {
            float fillAmount = gaugeFillSpeed * Time.deltaTime;
            currentExtinguisher += fillAmount;
            if (currentExtinguisher >= extinguish) //만약 소화 시간 채우면
            {
                //불 꺼지게
                isBurning = false;
                fireFx.SetActive(false);
            }

        }
    }
    private void CookDone(int index)
    {
        ingredient[index] = false;
        ingredient[index + 2] = true;

    }
}
