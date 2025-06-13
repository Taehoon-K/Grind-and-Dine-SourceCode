using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharcoalBowl : Bowl
{
    [Header("Fire")]
    [SerializeField]
    private float cook;  // �ִ� ���� ����. ����Ƽ ������ ���Կ��� ������ ��.
    private float currentCook;
    [SerializeField]
    private Image images_Gauge; //�丮���ΰ� ���� ������
    [SerializeField]
    private GameObject images_Cook; //�丮���϶� Ű�� ������ �̹���
    [SerializeField] private float gaugeFillSpeed; //�丮 �ӵ�
    [SerializeField] private GameObject steamFx; //�� fx
    [SerializeField] private GameObject crowbar; //���� ���ӿ�����Ʈ

    private bool isCook; //�丮������
    private bool beCooked; //�;�����, �;����� Ʈ���� ���ø�

    public bool IsCook
    {
        get { return isCook; }
        set
        {
            isCook = value;
            images_Cook.SetActive(value); //������ Ʈ��� ������ ���̰�
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
                // ���� ����: �������� ä��
                float fillAmount = gaugeFillSpeed * Time.deltaTime;
                currentCook += fillAmount;
                currentCook = Mathf.Clamp(currentCook, 0f, cook);
                images_Gauge.fillAmount = currentCook / cook;

            }
            else // �丮�� �� �;��� ��
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
            // �丮 ���� �ƴ� �� �ʱ�ȭ (�ʿ��� ���)
            //steamFx.SetActive(false);
            //fireFx.SetActive(false);
        }
    }

}
