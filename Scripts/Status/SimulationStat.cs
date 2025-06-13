using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimulStat", menuName = "SimulStat/SimulStat")]
public class SimulationStat : ScriptableObject
{
    public int hp;
    public float fatigue;
    public float hungry;
    public int angry;
    public int sadness;
    public int boredom;

    public int skillNumber;
    public int skillAmount;
}
