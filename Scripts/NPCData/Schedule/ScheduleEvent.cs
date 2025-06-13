using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct ScheduleEvent
{
    public string name;
    [Header("Conditions")]
    public GameTimestamp time;
    public GameTimestamp.DayOfTheWeek dayOfTheWeek;

    public int priority; //�켱����

    public bool factorDate;

    public bool ignoreDayOfWeek; //���� �����ϴ� �뵵

    [Header("Position")]
    public SceneTransitionManager.Location location;
    public Vector3 coord;
    public Vector3 facing;

    [Header("Sit")]
    public bool isSit; //�ɴ��� �ƴ���

}
