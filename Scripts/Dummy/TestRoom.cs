using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoom : MonoBehaviour
{
    [SerializeField] private int hp, hungry, fatigue, angry, sadness, boredom;

    void Start()
    {
        Status stat = new Status();

        stat.currentHp = hp;
        stat.currentAngry = angry;
        stat.currentHungry = hungry;
        stat.currentFatigue = fatigue;
        stat.currentSadness = sadness;
        stat.currentBoredom = boredom;  

        StatusManager.instance.LoadStatus(stat);

        StatusManager.instance.LoadGameStart(1);
    }


}
