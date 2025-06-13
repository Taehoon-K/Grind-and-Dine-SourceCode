using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

public class SkillSelectPrompt : UiPrompt
{
    [SerializeField]
    private Image[] skillImage; // �� ���� ������ �̹���
    [SerializeField]
    private LocalizeStringEvent[] skillName; // �� ���� ������ �̸�
    [SerializeField]
    private LocalizeStringEvent[] skillDescrep; // �� ���� ������ ����


    private int skillIndex; // ���� ��ų �ε���
    private int perkLevelIndex; // ���� �� ���� �ε��� (0: 5��, 1: 10��)


    public void OpenScreen(int skillIdx, int levelIdx) // ȭ�� ����
    {
        skillIndex = skillIdx;
        perkLevelIndex = levelIdx;

        // PerkManager���� PerkOption ��������
        PerkOption perkOption = StatusManager.instance.GetPerkOption(skillIndex, perkLevelIndex);

        if (perkOption != null && perkOption.skillImage.Length >= 2)
        {
            gameObject.SetActive(true);

            // UI ��� ����
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

    public void ButtonOn(bool isFirst) // ��ư Ŭ�� ��
    {
        int selectedOption = isFirst ? 0 : 1; // ù ��° �ɼ� ���� ����
        bool success = StatusManager.instance.SelectPerk(skillIndex, perkLevelIndex, selectedOption);

        if (success)
        {
            Debug.Log($"Perk {selectedOption} selected for skill {skillIndex}, level {perkLevelIndex}.");

            // Ư�� Perk ã��
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
                targetPerk.Render(); // Render ȣ��
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
        gameObject.SetActive(false); // ȭ�� �ݱ�
    }
}
