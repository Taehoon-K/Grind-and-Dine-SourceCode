using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Moodle
{
    public bool isActive; //Ȱ��ȭ����� ����
    public int timeLeft; //���ӽð�
}
[System.Serializable]
public class MoodleImform
{
    public int index; //���� �ε���
    public bool isActive; //Ȱ��ȭ����� ����
    public int timeLeft; //���ӽð�

    public float probability; //���� �ɸ� Ȯ��
}
