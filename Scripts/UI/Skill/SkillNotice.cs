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
    [SerializeField] private float fillSpeed = 1f; // �ʴ� ä������ �ӵ�

    [SerializeField] private LocalizedString[] skillString; //��ų ��������Ʈ
    [SerializeField] private LocalizeStringEvent skillText; //��ų ��������Ʈ
    [SerializeField] private Sprite[] skillSprites; //��ų ��������Ʈ
    [SerializeField] private Image skillImage; //��������Ʈ �� �̹���

    [SerializeField] private TextMeshProUGUI skillNum; //��ų ���� ���� �� �ؽ�Ʈ

    [SerializeField] private GameObject levelUp;
    private int skillNumb;

    public void Render(float oldValue, float newValue, int skillIndex)
    {
        levelUp.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(AnimateFill(oldValue, newValue));

        skillImage.sprite = skillSprites[skillIndex]; //��ų �̹��� �ֱ�
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

            // ä��� �ִϸ��̼� (���� �ð�)
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
                break; // �� ������ �ʿ� ����
            }
        }
    }
}
