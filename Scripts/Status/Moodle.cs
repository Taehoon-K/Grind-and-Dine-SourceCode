using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Moodle
{
    public bool isActive; //활성화됬는지 여부
    public int timeLeft; //지속시간
}
[System.Serializable]
public class MoodleImform
{
    public int index; //무들 인덱스
    public bool isActive; //활성화됬는지 여부
    public int timeLeft; //지속시간

    public float probability; //무들 걸릴 확률
}
