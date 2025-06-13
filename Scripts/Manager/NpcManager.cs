using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class NpcManager : MonoBehaviour,ITimeTracker
{
    public static NpcManager Instance { get; private set; }
    bool paused; //컷신때 npc멈추게
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

    //npc 스폰 정지
    public void Pause()
    {
        paused = true;
        //모든 npc kill
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
    private void InitNPCLocations() //npc 정보에 npc스케줄 연결
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

    //npc 위치 호출
    public NPCLocationState GetNPCLocation(string name)
    {
        return npcLocations.Find(x => x.npc.CharacterName() == name);
    }

    //모든 npc데이터 로드
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

    public void ClockUpdate(GameTimestamp timestamp) //시간 업뎃마다 호출될함수
    {
        UpdateNPCLocations(timestamp);
    }

    public bool HasParent(string npc) //부모 집 있는지 여부
    {
        NPC targetNPC = Characters().Find(x => x.CharacterName() == npc);
        return targetNPC != null && targetNPC.hasParent;
    }
    public string GetParent(string npc)
    {
        NPC targetNPC = Characters().Find(x => x.CharacterName() == npc);
        return targetNPC.parentName;
    }

    private void UpdateNPCLocations(GameTimestamp timestamp) //시간마다 위치 업뎃
    {
        if (paused) return;

        for(int i = 0; i < npcLocations.Count; i++)
        {
            NPCLocationState npcLocator = npcLocations[i];
            SceneTransitionManager.Location previousLocation = npcLocator.location;
            //npc로 스케줄찾기
            NPCScheduleData schedule = npcSchedules.Find(x => x.npc == npcLocator.npc);
            if(schedule == null)
            {
                Debug.LogError("No schedule found for " + npcLocator.npc.name);
                continue;
            }

            //current time

            GameTimestamp.DayOfTheWeek dayOfWeek = timestamp.GetDayOfTheWeek();

            //스케줄이벤트 찾기
            List<ScheduleEvent> eventsToConsider = schedule.npcScheduleList.FindAll(x => x.time.hour <= timestamp.hour && (x.dayOfTheWeek == dayOfWeek || x.ignoreDayOfWeek));
            //이벤트 비었는지 체크
            if(eventsToConsider.Count < 1)
            {
                Debug.LogError("None found for" + npcLocator.npc.name);
                Debug.LogError(timestamp.hour);
                continue;
            }

            //해당하는 이벤트 중 가장 높은 시간 값 찾기
            int maxHour = eventsToConsider.Max(x => x.time.hour);
            eventsToConsider.RemoveAll(x => x.time.hour < maxHour);

            //높은 우선순위 이벤트 얻기
            ScheduleEvent eventToExcute = eventsToConsider.OrderByDescending(x => x.priority).First();
            //Debug.Log(eventToExcute.name);
            //Npc locator 값 설정
            npcLocations[i] = new NPCLocationState(schedule.npc, eventToExcute.location, eventToExcute.coord,eventToExcute.facing, eventToExcute.isSit);

            SceneTransitionManager.Location newLocation = eventToExcute.location;
            //만약 위치 바뀌었다면
            if(newLocation != previousLocation)
            {
                Debug.Log("New Location: " + newLocation + "   "+ npcLocator.npc.name+ previousLocation);
                //만약 우리가 있는 위치면
                if(SceneTransitionManager.Instance.currentLocation == newLocation)
                {
                    SpawnInNPC(schedule.npc, previousLocation);
                }
            }
        }
    }

    private void FirstUpdateNPCLocations() //맨 처음 로드 시 한번 npc위치 업뎃
    {
        GameTimestamp timestamp = TimeManager.instance.GetGameTimestamp();
        for (int i = 0; i < npcLocations.Count; i++)
        {
            NPCLocationState npcLocator = npcLocations[i];
            SceneTransitionManager.Location previousLocation = npcLocator.location;
            //npc로 스케줄찾기
            NPCScheduleData schedule = npcSchedules.Find(x => x.npc == npcLocator.npc);
            if (schedule == null)
            {
                Debug.LogError("No schedule found for " + npcLocator.npc.name);
                continue;
            }

            GameTimestamp.DayOfTheWeek dayOfWeek = timestamp.GetDayOfTheWeek();

            //스케줄이벤트 찾기
            List<ScheduleEvent> eventsToConsider = schedule.npcScheduleList.FindAll(x => x.time.hour <= timestamp.hour && (x.dayOfTheWeek == dayOfWeek || x.ignoreDayOfWeek));
            //이벤트 비었는지 체크
            if (eventsToConsider.Count < 1)
            {
                Debug.LogError("None found for" + npcLocator.npc.name);
                Debug.LogError(timestamp.hour);
                continue;
            }

            //해당하는 이벤트 중 가장 높은 시간 값 찾기
            int maxHour = eventsToConsider.Max(x => x.time.hour);
            eventsToConsider.RemoveAll(x => x.time.hour < maxHour);

            //높은 우선순위 이벤트 얻기
            ScheduleEvent eventToExcute = eventsToConsider.OrderByDescending(x => x.priority).First();
            //Debug.Log(eventToExcute.name);
            //Npc locator 값 설정
            npcLocations[i] = new NPCLocationState(schedule.npc, eventToExcute.location, eventToExcute.coord, eventToExcute.facing, eventToExcute.isSit);
        }
    }

    public void UpdateMinimapIcon() //npc 아이콘 변경
    {
        var npcs = FindObjectsOfType<InteractableCharacter>(); // 또는 NPC base class
        foreach (var npc in npcs)
        {
            npc.UpdateIcon();
        }
    }
}
