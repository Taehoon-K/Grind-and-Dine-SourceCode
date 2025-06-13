using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class NpcManager : MonoBehaviour,ITimeTracker
{
    public static NpcManager Instance { get; private set; }
    bool paused; //�ƽŶ� npc���߰�
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    List<NPC> npcs = null;
    List<NPCScheduleData> npcSchedules;

    [SerializeField]
    List<NPCLocationState> npcLocations;

    private void OnEnable()
    {
        NPCScheduleData[] schedules = Resources.LoadAll<NPCScheduleData>("Schedule");
        npcSchedules = schedules.ToList();
        InitNPCLocations();
    }

    //npc ���� ����
    public void Pause()
    {
        paused = true;
        //��� npc kill
        InteractableCharacter[] npcs = Object.FindObjectsByType<InteractableCharacter>(sortMode: default);
        foreach(InteractableCharacter npc in npcs)
        {
            Destroy(npc.gameObject);
        }
    }
    public void Continue()
    {
        paused = false;
        RenderNPCs();
    }

    private void Start()
    {
        TimeManager.instance.RegisterTracker(this);
        SceneTransitionManager.Instance.onLoactionLoad.AddListener(RenderNPCs);

        FirstUpdateNPCLocations();
    }
    private void InitNPCLocations() //npc ������ npc������ ����
    {
        npcLocations = new List<NPCLocationState>();
        foreach(NPC npc in Characters())
        {
            npcLocations.Add(new NPCLocationState(npc));
        }
    }

    private void RenderNPCs()
    {
        foreach(NPCLocationState npc in npcLocations)
        {
            if(npc.location == SceneTransitionManager.Instance.currentLocation)
            {
                GameObject npcInstance =  Instantiate(npc.npc.prefab, npc.coord, Quaternion.Euler(npc.facing));
                //npcInstance.GetComponent<InteractableCharacter>().UpdateIcon();

                if (npc.isSit)
                {
                    Animator anim = npcInstance.GetComponent<Animator>();

                    if (anim != null)
                    {
                        anim.SetBool("Sit", true);
                    }
                }
            }
        }
    }

    //npc ��ġ ȣ��
    public NPCLocationState GetNPCLocation(string name)
    {
        return npcLocations.Find(x => x.npc.CharacterName() == name);
    }

    //��� npc������ �ε�
    public List<NPC> Characters()
    {
        if (npcs != null&& npcs.Count > 0) 
        {
            return npcs; 
        }
        NPC[] npcDataBase = Resources.LoadAll<NPC>("NPC");
        npcs = npcDataBase.ToList();
        return npcs;
    }

    private void SpawnInNPC(NPC npc, SceneTransitionManager.Location comingFrom)
    {
        Transform start = LocationManager.Instance.GetPlayerStartingPosition(comingFrom);
        Instantiate(npc.prefab, start.position, start.rotation);
    }

    public void ClockUpdate(GameTimestamp timestamp) //�ð� �������� ȣ����Լ�
    {
        UpdateNPCLocations(timestamp);
    }

    public bool HasParent(string npc) //�θ� �� �ִ��� ����
    {
        NPC targetNPC = Characters().Find(x => x.CharacterName() == npc);
        return targetNPC != null && targetNPC.hasParent;
    }
    public string GetParent(string npc)
    {
        NPC targetNPC = Characters().Find(x => x.CharacterName() == npc);
        return targetNPC.parentName;
    }

    private void UpdateNPCLocations(GameTimestamp timestamp) //�ð����� ��ġ ����
    {
        if (paused) return;

        for(int i = 0; i < npcLocations.Count; i++)
        {
            NPCLocationState npcLocator = npcLocations[i];
            SceneTransitionManager.Location previousLocation = npcLocator.location;
            //npc�� ������ã��
            NPCScheduleData schedule = npcSchedules.Find(x => x.npc == npcLocator.npc);
            if(schedule == null)
            {
                Debug.LogError("No schedule found for " + npcLocator.npc.name);
                continue;
            }

            //current time

            GameTimestamp.DayOfTheWeek dayOfWeek = timestamp.GetDayOfTheWeek();

            //�������̺�Ʈ ã��
            List<ScheduleEvent> eventsToConsider = schedule.npcScheduleList.FindAll(x => x.time.hour <= timestamp.hour && (x.dayOfTheWeek == dayOfWeek || x.ignoreDayOfWeek));
            //�̺�Ʈ ������� üũ
            if(eventsToConsider.Count < 1)
            {
                Debug.LogError("None found for" + npcLocator.npc.name);
                Debug.LogError(timestamp.hour);
                continue;
            }

            //�ش��ϴ� �̺�Ʈ �� ���� ���� �ð� �� ã��
            int maxHour = eventsToConsider.Max(x => x.time.hour);
            eventsToConsider.RemoveAll(x => x.time.hour < maxHour);

            //���� �켱���� �̺�Ʈ ���
            ScheduleEvent eventToExcute = eventsToConsider.OrderByDescending(x => x.priority).First();
            //Debug.Log(eventToExcute.name);
            //Npc locator �� ����
            npcLocations[i] = new NPCLocationState(schedule.npc, eventToExcute.location, eventToExcute.coord,eventToExcute.facing, eventToExcute.isSit);

            SceneTransitionManager.Location newLocation = eventToExcute.location;
            //���� ��ġ �ٲ���ٸ�
            if(newLocation != previousLocation)
            {
                Debug.Log("New Location: " + newLocation + "   "+ npcLocator.npc.name+ previousLocation);
                //���� �츮�� �ִ� ��ġ��
                if(SceneTransitionManager.Instance.currentLocation == newLocation)
                {
                    SpawnInNPC(schedule.npc, previousLocation);
                }
            }
        }
    }

    private void FirstUpdateNPCLocations() //�� ó�� �ε� �� �ѹ� npc��ġ ����
    {
        GameTimestamp timestamp = TimeManager.instance.GetGameTimestamp();
        for (int i = 0; i < npcLocations.Count; i++)
        {
            NPCLocationState npcLocator = npcLocations[i];
            SceneTransitionManager.Location previousLocation = npcLocator.location;
            //npc�� ������ã��
            NPCScheduleData schedule = npcSchedules.Find(x => x.npc == npcLocator.npc);
            if (schedule == null)
            {
                Debug.LogError("No schedule found for " + npcLocator.npc.name);
                continue;
            }

            GameTimestamp.DayOfTheWeek dayOfWeek = timestamp.GetDayOfTheWeek();

            //�������̺�Ʈ ã��
            List<ScheduleEvent> eventsToConsider = schedule.npcScheduleList.FindAll(x => x.time.hour <= timestamp.hour && (x.dayOfTheWeek == dayOfWeek || x.ignoreDayOfWeek));
            //�̺�Ʈ ������� üũ
            if (eventsToConsider.Count < 1)
            {
                Debug.LogError("None found for" + npcLocator.npc.name);
                Debug.LogError(timestamp.hour);
                continue;
            }

            //�ش��ϴ� �̺�Ʈ �� ���� ���� �ð� �� ã��
            int maxHour = eventsToConsider.Max(x => x.time.hour);
            eventsToConsider.RemoveAll(x => x.time.hour < maxHour);

            //���� �켱���� �̺�Ʈ ���
            ScheduleEvent eventToExcute = eventsToConsider.OrderByDescending(x => x.priority).First();
            //Debug.Log(eventToExcute.name);
            //Npc locator �� ����
            npcLocations[i] = new NPCLocationState(schedule.npc, eventToExcute.location, eventToExcute.coord, eventToExcute.facing, eventToExcute.isSit);
        }
    }

    public void UpdateMinimapIcon() //npc ������ ����
    {
        var npcs = FindObjectsOfType<InteractableCharacter>(); // �Ǵ� NPC base class
        foreach (var npc in npcs)
        {
            npc.UpdateIcon();
        }
    }
}
