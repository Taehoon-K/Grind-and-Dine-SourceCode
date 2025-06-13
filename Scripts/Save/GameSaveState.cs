using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveState
{
    public GameBlackboard blackboard;
    public GameTimestamp timestamp;
    public int money;
    public string inven;
    public string name;
    public int difficulty;
    public int totalDept;
    public int weekDept;
    public int failDept;
    public Moodle[] moodle;
    public Status stat;
    public bool sex;

    //public PP.InventorySystem.Item[] invenItem;

    //npc °ü°è
    //public List<NPCRelationshipState> relationships;
    public List<bool> secrets;
    public QuestSaveWrapper questData;

    public int totalMoney;

    public Dictionary<string, GameTimestamp> lootingObjects;

    public GameSaveState
        (GameBlackboard blackboard,
        GameTimestamp timestamp, 
        int money, 
        string inven,
        string name,
        int difficulty,
       // List<NPCRelationshipState> relationships,
        List<bool> secrets,
        QuestSaveWrapper questData,
        int totalMoney,
        int totalDept,
        int weekDept,
        int failDept,
        Moodle [] moodle,
        Status stat,
        bool sex,
        Dictionary<string, GameTimestamp> lootingObjects)
    {
        this.blackboard = blackboard;
        this.timestamp = timestamp;
        this.money = money;
        this.inven = inven;
        this.name = name;
        this.difficulty = difficulty;
        //this.invenItem = invenItem;
        //this.relationships = relationships;
        this.secrets = secrets;
        this.questData = questData;
        this.totalMoney = totalMoney; //ÃÑ¼Òµæ
        this.totalDept = totalDept;
        this.weekDept = weekDept;
        this.failDept = failDept;
        this.moodle = moodle;
        this.stat = stat;
        this.sex = sex;
        this.lootingObjects = lootingObjects;
    }
}
