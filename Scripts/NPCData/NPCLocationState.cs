using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The location of the NPC at the point of time
/// </summary>
[System.Serializable]
public struct NPCLocationState
{
    public NPC npc;
    public SceneTransitionManager.Location location;
    public Vector3 coord;
    public Vector3 facing;
    public bool isSit;

    public NPCLocationState(NPC npc): this()
    {
        this.npc = npc;
    }
    public NPCLocationState(NPC npc, SceneTransitionManager.Location location, Vector3 coord,Vector3 facing,bool sit)
    {
        this.npc = npc;
        this.location = location;
        this.coord = coord;
        this.facing = facing;
        this.isSit = sit;
    }
}
