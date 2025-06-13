using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StartPoint
{
    public SceneTransitionManager.Location enteringFrom;

    //플레이어가 시작해야되는 위치
    public Transform playerStart;
}
