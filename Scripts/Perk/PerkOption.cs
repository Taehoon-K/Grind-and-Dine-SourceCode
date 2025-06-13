using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "SkillInfo/PerkOption", fileName = "NewPerkOption")]
public class PerkOption : ScriptableObject
{
    //public int id; // 고유 ID
    public Sprite[] skillImage; // 스킬 관련 이미지
    public LocalizedString[] skillName; // 스킬 이름
    public LocalizedString[] skillDescreption; // 스킬 설명
  //  public LocalizedString[] skillEffect; // 스킬 효과
}
