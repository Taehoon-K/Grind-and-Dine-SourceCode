using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

public class SkillSelectPrompt : UiPrompt
{
    [SerializeField]
    private Image[] skillImage; // 두 개의 선택지 이미지
    [SerializeField]
    private LocalizeStringEvent[] skillName; // 두 개의 선택지 이름
    [SerializeField]
    private LocalizeStringEvent[] skillDescrep; // 두 개의 선택지 설명


    private int skillIndex; // 현재 스킬 인덱스
    private int perkLevelIndex; // 현재 퍽 레벨 인덱스 (0: 5렙, 1: 10렙)


    public void OpenScreen(int skillIdx, int levelIdx) // 화면 띄우기
    {
        skillIndex = skillIdx;
        perkLevelIndex = levelIdx;

        // PerkManager에서 PerkOption 가져오기
        PerkOption perkOption = StatusManager.instance.GetPerkOption(skillIndex, perkLevelIndex);

        if (perkOption != null && perkOption.skillImage.Length >= 2)
        {
            gameObject.SetActive(true);

            // UI 요소 설정
            skillImage[0].sprite = perkOption.skillImage[0];
            skillImage[1].sprite = perkOption.skillImage[1];

            skillName[0].StringReference = perkOption.skillName[0];
            skillName[1].StringReference = perkOption.skillName[1];

            skillDescrep[0].StringReference = perkOption.skillDescreption[0];
            skillDescrep[1].StringReference = perkOption.skillDescreption[1];
        }
        else
        {
            Debug.LogError("Perk option is invalid or missing data.");
        }
    }

    public void ButtonOn(bool isFirst) // 버튼 클릭 시
    {
        int selectedOption = isFirst ? 0 : 1; // 첫 번째 옵션 선택 여부
        bool success = StatusManager.instance.SelectPerk(skillIndex, perkLevelIndex, selectedOption);

        if (success)
        {
            Debug.Log($"Perk {selectedOption} selected for skill {skillIndex}, level {perkLevelIndex}.");

            // 특정 Perk 찾기
            PerkButton[] perks = FindObjectsOfType<PerkButton>();
            PerkButton targetPerk = null;

            foreach (PerkButton perk in perks)
            {
                if (perk.skillIndex == skillIndex && perk.perkLevelIndex == perkLevelIndex)
                {
                    targetPerk = perk;
                    break;
                }
            }

            if (targetPerk != null)
            {
                targetPerk.Render(); // Render 호출
            }
            else
            {
                Debug.LogError("No matching Perk found.");
            }
        }
        else
        {
            Debug.LogError("Failed to select perk. Check perk state or data.");
        }

        UIManager.instance.ExitSkillPrompt();
        gameObject.SetActive(false); // 화면 닫기
    }
}
