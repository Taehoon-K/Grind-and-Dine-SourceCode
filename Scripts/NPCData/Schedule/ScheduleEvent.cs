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

    public int priority; //우선순위

    public bool factorDate;

    public bool ignoreDayOfWeek; //요일 무시하는 용도

    [Header("Position")]
    public SceneTransitionManager.Location location;
    public Vector3 coord;
    public Vector3 facing;

    [Header("Sit")]
    public bool isSit; //앉는지 아닌지

}
