using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Sprite frontCard; //카드 앞면 이미지, resultPanel에서 보낼것
    private Sprite backCard; //카드 뒷면 이미지
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
    public void OnClick() //카드 누르면 실행될 함수
    {
        if (coroutineAllowed)
        {
            StartCoroutine(RotateCard());
        }
    }

    public void AfterRotate() //나머지 카드들 돌아가는 함수
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
