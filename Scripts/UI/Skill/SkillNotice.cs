using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class SkillNotice : MonoBehaviour
{
    [SerializeField] private Image oldBar;
    [SerializeField] private Image newBar;
    [SerializeField] private float fillSpeed = 1f; // 초당 채워지는 속도

    [SerializeField] private LocalizedString[] skillString; //스킬 스프라이트
    [SerializeField] private LocalizeStringEvent skillText; //스킬 스프라이트
    [SerializeField] private Sprite[] skillSprites; //스킬 스프라이트
    [SerializeField] private Image skillImage; //스프라이트 들어갈 이미지

    [SerializeField] private TextMeshProUGUI skillNum; //스킬 레벨 숫자 들어갈 텍스트

    [SerializeField] private GameObject levelUp;
    private int skillNumb;

    public void Render(float oldValue, float newValue, int skillIndex)
    {
        levelUp.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(AnimateFill(oldValue, newValue));

        skillImage.sprite = skillSprites[skillIndex]; //스킬 이미지 넣기
        skillText.StringReference = skillString[skillIndex];
        skillNumb = StatusManager.instance.GetStatus().level[skillIndex];
        skillNum.text = skillNumb.ToString();
    }

    private IEnumerator AnimateFill(float oldValue, float newValue)
    {
        Debug.Log(oldValue + "    asdfsda" + newValue);

        oldBar.fillAmount = oldValue;
        newBar.fillAmount = oldValue;

        float targetValue = newValue;

        while (targetValue > 0f)
        {
            float fillFrom = newBar.fillAmount;
            float fillTo = Mathf.Min(1f, targetValue);
            float duration = 0.5f;
            float elapsed = 0f;

            // 채우기 애니메이션 (고정 시간)
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                newBar.fillAmount = Mathf.Lerp(fillFrom, fillTo, t);
                yield return null;
            }

            if (Mathf.Approximately(fillTo, 1f))
            {
                yield return new WaitForSeconds(0.2f);
                oldBar.gameObject.SetActive(false);
                newBar.fillAmount = 0f;
                targetValue -= 1f;

                levelUp.SetActive(true);
                skillNum.text = skillNumb.ToString();
            }
            else
            {
                break; // 더 진행할 필요 없음
            }
        }
    }
}
