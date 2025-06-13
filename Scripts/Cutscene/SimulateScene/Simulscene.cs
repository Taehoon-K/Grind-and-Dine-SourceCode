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
    public int index; //�ش� �ù�Ȱ���� �ε���

    public LocalizedString localizedText; //������ �ٿ� ��Ÿ�� �ؽ�Ʈ ���ö�����
    public string audioSource; //�ʿ�� ���� ȿ���� �̸�

    [Header("Prefab")]
    public bool isPrefab; //���� ���� �ʿ��ؼ� ���� ���� �� ������ ��ߵɰ��
    public GameObject man,woman;
}
[System.Serializable]
public class ExtraSimulInfomation
{
    public RuntimeAnimatorController extraAnimator;
    public Vector3 extraGenPoint;
    public Quaternion extraGenRotation;
}