using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "SkillInfo/PerkOption", fileName = "NewPerkOption")]
public class PerkOption : ScriptableObject
{
    //public int id; // ���� ID
    public Sprite[] skillImage; // ��ų ���� �̹���
    public LocalizedString[] skillName; // ��ų �̸�
    public LocalizedString[] skillDescreption; // ��ų ����
  //  public LocalizedString[] skillEffect; // ��ų ȿ��
}
