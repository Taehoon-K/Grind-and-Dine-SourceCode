using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class QuestNotice : MonoBehaviour
{
    [SerializeField] LocalizeStringEvent questTitle;
    [SerializeField] GameObject startQuest,endQuest; //퀘스트 시작과 완료 text
    [SerializeField] AudioClip[] sound;
    [SerializeField] private AudioSource audios;

    public void Render(int isStart, LocalizedString localizedString)
    {
        StartCoroutine(DelayedRender(isStart, localizedString));
    }

    private IEnumerator DelayedRender(int isStart, LocalizedString localizedString)
    {
        // 1. 텍스트 적용
        questTitle.StringReference = localizedString;

        // 2. UI 요소 활성화 상태 변경
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

        // 3. 한 프레임 기다려 텍스트가 실제로 적용되게 함
        yield return null;

        // 4. 레이아웃 강제 갱신
        Canvas.ForceUpdateCanvases();

        gameObject.SetActive(false);
        gameObject.SetActive(true);

        // 5. 사운드 재생
        audios.Play();
    }
}
