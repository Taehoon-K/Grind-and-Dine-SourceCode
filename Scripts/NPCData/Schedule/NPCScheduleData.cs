using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC Schedule")]
public class NPCScheduleData : ScriptableObject
{
    public NPC npc;
    public List<ScheduleEvent> npcScheduleList;
}
