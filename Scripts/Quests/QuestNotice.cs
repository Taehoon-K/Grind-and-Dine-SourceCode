using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class QuestNotice : MonoBehaviour
{
    [SerializeField] LocalizeStringEvent questTitle;
    [SerializeField] GameObject startQuest,endQuest; //����Ʈ ���۰� �Ϸ� text
    [SerializeField] AudioClip[] sound;
    [SerializeField] private AudioSource audios;

    public void Render(int isStart, LocalizedString localizedString)
    {
        StartCoroutine(DelayedRender(isStart, localizedString));
    }

    private IEnumerator DelayedRender(int isStart, LocalizedString localizedString)
    {
        // 1. �ؽ�Ʈ ����
        questTitle.StringReference = localizedString;

        // 2. UI ��� Ȱ��ȭ ���� ����
        if (isStart == 0)
        {
            endQuest.SetActive(false);
            startQuest.SetActive(true);
            audios.clip = sound[0];
        }
        else
        {
            startQuest.SetActive(false);
            endQuest.SetActive(true);
            audios.clip = sound[1];
        }

        // 3. �� ������ ��ٷ� �ؽ�Ʈ�� ������ ����ǰ� ��
        yield return null;

        // 4. ���̾ƿ� ���� ����
        Canvas.ForceUpdateCanvases();

        gameObject.SetActive(false);
        gameObject.SetActive(true);

        // 5. ���� ���
        audios.Play();
    }
}
