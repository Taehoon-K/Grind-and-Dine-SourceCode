using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Sprite frontCard; //ī�� �ո� �̹���, resultPanel���� ������
    private Sprite backCard; //ī�� �޸� �̹���
    private Image rend;
    private bool coroutineAllowed;

    private void OnEnable()
    {
        rend = GetComponent<Image>();
        if(backCard == null)
        {
            backCard = rend.sprite;
        }
        rend.sprite = backCard;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        coroutineAllowed = true;
    }
    public void OnClick() //ī�� ������ ����� �Լ�
    {
        if (coroutineAllowed)
        {
            StartCoroutine(RotateCard());
        }
    }

    public void AfterRotate() //������ ī��� ���ư��� �Լ�
    {
        Invoke(nameof(AA), 1f);
    }
    private void AA()
    {
        StartCoroutine(RotateCard());
    }
    private IEnumerator RotateCard()
    {
        coroutineAllowed = false;

        for(float i = 0f; i<= 180f; i += 10f)
        {
            transform.rotation = Quaternion.Euler(0f, i, 0f);
            if (i == 90f)
            {
                rend.sprite = frontCard;
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }


    }
}
