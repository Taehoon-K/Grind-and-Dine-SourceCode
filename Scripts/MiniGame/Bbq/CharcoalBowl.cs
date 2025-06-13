using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharcoalBowl : Bowl
{
    [Header("Fire")]
    [SerializeField]
    private float cook;  // 최대 익힘 상태. 유니티 에디터 슬롯에서 지정할 것.
    private float currentCook;
    [SerializeField]
    private Image images_Gauge; //요리중인거 위에 게이지
    [SerializeField]
    private GameObject images_Cook; //요리중일때 키는 게이지 이미지
    [SerializeField] private float gaugeFillSpeed; //요리 속도
    [SerializeField] private GameObject steamFx; //김 fx
    [SerializeField] private GameObject crowbar; //집게 게임오브젝트

    private bool isCook; //요리중인지
    private bool beCooked; //익었는지, 익었으면 트레이 못올림

    public bool IsCook
    {
        get { return isCook; }
        set
        {
            isCook = value;
            images_Cook.SetActive(value); //이즈쿡 트루면 게이지 보이게
            crowbar.SetActive(!value);
        }
    }
    public bool BeCooked
    {
        get { return beCooked; }
        set
        {
            beCooked = value;
            if (!beCooked)
            {
                ingredient[0] = true;
                ingredient[1] = false;
                steamFx.SetActive(false);
            }
            else
            {
                ingredient[1] = true;
                ingredient[0] = false;
                steamFx.SetActive(true);
            }
        }
    }

    private void Start()
    {
        BeCooked = false;
    }
    protected override void Update()
    {
        base.Update();
        CookGuage();
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

            }
            else // 요리가 다 익었을 때
            {
                BeCooked = true;

                //SoundManager.instance.PlaySound3D("timerAlarm", gameObject.transform, 0, true, SoundType.SFX);
                if(GetComponent<AudioSource>().enabled == false)
                {
                    GetComponent<AudioSource>().enabled = true;
                }
            }
        }
        else
        {
            if(GetComponent<AudioSource>().enabled == true)
            {
                GetComponent<AudioSource>().enabled = false;
            }
            //SoundManager.instance.StopLoopSound("timerAlarm");
            // 요리 중이 아닐 때 초기화 (필요한 경우)
            //steamFx.SetActive(false);
            //fireFx.SetActive(false);
        }
    }

}
