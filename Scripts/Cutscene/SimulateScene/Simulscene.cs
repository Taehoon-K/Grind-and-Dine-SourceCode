using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


[CreateAssetMenu(fileName = "Simulscene", menuName ="Cutscene/Simulscene")]
public class Simulscene : ScriptableObject,IConditional
{
    public RuntimeAnimatorController playerAnimator;
    public Vector3 playerGenPoint;
    public Quaternion playerGenRotation;
    public ExtraSimulInfomation[] extraSimulInfomations;

    public Vector3 CameraStartPosition;

    public int skipTime;
    public int index; //해당 시뮬활동의 인덱스

    public LocalizedString localizedText; //게이지 바에 나타낼 텍스트 로컬라이즈
    public string audioSource; //필요시 넣을 효과음 이름

    [Header("Prefab")]
    public bool isPrefab; //만약 도구 필요해서 따로 도구 든 프리팹 써야될경우
    public GameObject man,woman;
}
[System.Serializable]
public class ExtraSimulInfomation
{
    public RuntimeAnimatorController extraAnimator;
    public Vector3 extraGenPoint;
    public Quaternion extraGenRotation;
}