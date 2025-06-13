using Kupa;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    //씬에 있는 모든 액터 리스트
    Dictionary<string, NavMeshAgent> actors;
    //플레이어 프리팹
    [SerializeField] Kupa.Player player;

    [SerializeField] string sceneName;
    [SerializeField] string nextSceneName;

    [Header("SimulScene")]
    [SerializeField] private GameObject manExtra, womanExtra;
    private GameObject[] spawnedExtraNPCs; // 생성된 NPC들을 저장할 배열
    GameObject prefab; //플레이어 프리팹
    private Simulscene simulScene;

    string location;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (GameTimeStateManager.instance == null && sceneName != "StartBus") //만약 첫 시작 컷신이면, 그리고 스타트버스는 빼기, 스플래시 스클린 끝나고 호출시킬거
        {
            OnLocationLoad();
        }
    }
    const string CUTSCENE_PREFIX = "Cutscene";

    Queue<CutsceneAction> actionsToExecute;
    public Action onCutsceneStop;

    public void StartCut(int time) //유니티 이벤트로 실행, 씬트랜매니저 없을때 컷신 발동시키는 용도
    {
        Invoke(nameof(OnLocationLoad), time);
    }
    public void OnLocationLoad()
    {
        if(SceneTransitionManager.Instance != null)
        {
            //Get current Scene
            location = SceneTransitionManager.Instance.currentLocation.ToString();
        }
        else
        {
            location = sceneName;
        }
        

        //Load in all the candidate cutscenes
        Cutscene[] candidates = Resources.LoadAll<Cutscene>("Cutscenes/" + location);
        Cutscene cutsceneToPlay = GetCutsceneToPlay(candidates);

        //컷신 있는지 체크
        if (!cutsceneToPlay) return;
        Debug.Log($"the cutscene to play is {cutsceneToPlay.name}");

        StartCutsceneSequence(cutsceneToPlay);
    }



    public void StartSimulateScene(Simulscene simulsceneToPlay, bool isRisk = false, Action actionRisk = null, Action actionNone = null,int whatSimul = 0,bool isCriminal = false) //리스크 행동시 각자 액션 두개
    {
        //시간과 플레이어 비활성화
        if (TimeManager.instance != null)
        {
            TimeManager.instance.TimeTicking = false; //타임스토프
            player = FindAnyObjectByType<Kupa.Player>();
            player.enabled = false;
            player.GetComponent<CharacterController>().enabled = false;
            NpcManager.Instance.Pause();
        }

        if (!simulsceneToPlay.isPrefab) //그냥 시뮬이면
        {
            if (PlayerStats.IsWoman) //여자면
            {
                prefab = Instantiate(womanExtra, simulsceneToPlay.playerGenPoint, simulsceneToPlay.playerGenRotation);//복제

            }
            else //남자면
            {
                prefab = Instantiate(manExtra, simulsceneToPlay.playerGenPoint, simulsceneToPlay.playerGenRotation);//복제
            }
            /*AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponent<AdvancedPeopleSystem.CharacterCustomization>();
            var saved = cc.GetSavedCharacterDatas();

            // 해당 이름을 가진 데이터가 있는지 확인
            for (int i = 0; i < saved.Count; i++)
            {
                //Debug.Log("Character data appliedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa for: " + saved[i].name);
                if (saved[i].name == 5.ToString())
                {
                    cc.ApplySavedCharacterData(saved[i]); // 원하는 이름의 캐릭터 데이터 적용
                    Debug.Log("Character data applied for: " + saved[i].name);
                    break; // 찾으면 더 이상 반복하지 않고 종료
                }
            }
            prefab.layer = 0;
            if (prefab.GetComponent<Animator>() == null)
            {
                prefab.AddComponent<Animator>();
                Debug.Log("Animator 컴포넌트가 없어서 새로 추가했습니다.");
            }
            prefab.GetComponent<Animator>().runtimeAnimatorController = simulsceneToPlay.playerAnimator; //애니메이션 컨트롤러 설정
            prefab.GetComponent<Animator>().avatar = player.GetComponent<Animator>().avatar; //플레이어 시뮬캐릭에 아바타 설정
            foreach (Transform child in prefab.GetComponentsInChildren<Transform>(true)) //자식오브젝트의 레이어들도 다 디폴트로 바꿈
            {
                child.gameObject.layer = 0;
            }*/
        }
        else //만약 물건 든 프리팹 세울거면
        {
            if (PlayerStats.IsWoman) //여자면
            {
                prefab = Instantiate(simulsceneToPlay.woman, simulsceneToPlay.playerGenPoint, simulsceneToPlay.playerGenRotation);//복제

            }
            else //남자면
            {
                prefab = Instantiate(simulsceneToPlay.man, simulsceneToPlay.playerGenPoint, simulsceneToPlay.playerGenRotation);//복제
            }
            
        }
        AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponent<AdvancedPeopleSystem.CharacterCustomization>();
        var saved = cc.GetSavedCharacterDatas();

        // 해당 이름을 가진 데이터가 있는지 확인
        for (int i = 0; i < saved.Count; i++)
        {
            //Debug.Log("Character data appliedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa for: " + saved[i].name);
            if (saved[i].name == 5.ToString())
            {
                cc.ApplySavedCharacterData(saved[i]); // 원하는 이름의 캐릭터 데이터 적용
                Debug.Log("Character data applied for: " + saved[i].name);
                break; // 찾으면 더 이상 반복하지 않고 종료
            }
        }
        prefab.layer = 0;
        if (prefab.GetComponent<Animator>() == null)
        {
            prefab.AddComponent<Animator>();
            Debug.Log("Animator 컴포넌트가 없어서 새로 추가했습니다.");
        }
        prefab.GetComponent<Animator>().runtimeAnimatorController = simulsceneToPlay.playerAnimator; //애니메이션 컨트롤러 설정
        prefab.GetComponent<Animator>().avatar = player.GetComponent<Animator>().avatar; //플레이어 시뮬캐릭에 아바타 설정
        foreach (Transform child in prefab.GetComponentsInChildren<Transform>(true)) //자식오브젝트의 레이어들도 다 디폴트로 바꿈
        {
            child.gameObject.layer = 0;
        }

        prefab.GetComponent<Animator>().Rebind();
        prefab.GetComponent<Animator>().Update(0); // 업데이트를 강제로 호출해 즉시 반영



        spawnedExtraNPCs = new GameObject[simulsceneToPlay.extraSimulInfomations.Length]; // 배열 크기 설정
        for (int i = 0; i < simulsceneToPlay.extraSimulInfomations.Length; i++)
        {
            GameObject npc;
            ExtraSimulInfomation info = simulsceneToPlay.extraSimulInfomations[i];

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                // NPC 생성
                npc = Instantiate(womanExtra, info.extraGenPoint, info.extraGenRotation);
                npc.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().Randomize();
                spawnedExtraNPCs[i] = npc; // 배열에 저장
            }
            else
            {
                npc = Instantiate(manExtra, info.extraGenPoint, info.extraGenRotation);
                npc.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().Randomize();
                spawnedExtraNPCs[i] = npc; // 배열에 저장
            }

            npc.layer = 0;
            // 애니메이터 컨트롤러 설정
            Animator animator = npc.GetComponent<Animator>();
            if (animator != null && info.extraAnimator != null)
            {
                animator.runtimeAnimatorController = info.extraAnimator;
            }
        }

        Vector3 newPosition = prefab.transform.position;
        newPosition.y += 1.5f;

        if (!isRisk)
        {
            CutsceneCamManager.Instance.OrbitAroundObject(simulsceneToPlay.CameraStartPosition, newPosition, 0.1f);
            UIManager.instance?.OnSimulGauge(simulsceneToPlay.localizedText); //시뮬레이션 게이지 키기

            if (simulsceneToPlay.index >= 0) //작업일 시에는 스탯증감 여기서 안하기
            {
                StatusManager.instance.SimulationCalcul(simulsceneToPlay.index); //스탯 증감 실행
                TimeManager.instance.SkipTime(simulsceneToPlay.skipTime); //시간 스킵, 작업일 시 시간 스킵 나중에 하기
            }
            StartCoroutine(InvokeDMethodAfterDelay(8.1f)); // 8초 후 끝내는 함수 실행
        }
        else  //만약 리스크 있는 행동이면
        {
            CutsceneCamManager.Instance.OrbitAroundObject(simulsceneToPlay.CameraStartPosition, newPosition, 0.01f); //카메라 고정

            if (simulsceneToPlay.index >= 0) //인덱스 0 이상만
            {
                StatusManager.instance.SimulationRiskCalcul(simulsceneToPlay.index); //스탯 증감 실행
            }
            TimeManager.instance.SkipTime(simulsceneToPlay.skipTime); //시간 스킵, 작업일 시 시간 스킵 나중에 하기
            UIManager.instance.TriggerCardPanel(whatSimul, (bool isCaught) =>
            {
                if (isCaught) //만약 리스크 걸렸다면
                {
                    actionRisk?.Invoke();
                }
                else
                {
                    actionNone?.Invoke();
                }
                EndSimulCutscene();
            },isCriminal);
        }

        if(!string.IsNullOrEmpty(simulsceneToPlay.audioSource))
        {
            SoundManager.instance.PlaySound2D(simulsceneToPlay.audioSource, 0, true);
        }
        simulScene = simulsceneToPlay;
    }
    private IEnumerator InvokeDMethodAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndSimulCutscene();
    }

    public void EndSimulCutscene()
    {
        CutsceneCamManager.Instance.StopOrbit();
        Destroy(prefab);
        prefab = null;

        if (!string.IsNullOrEmpty(simulScene.audioSource))
        {
            SoundManager.instance.StopLoopSound(simulScene.audioSource);
        }

        if (spawnedExtraNPCs == null) return;

        for (int i = 0; i < spawnedExtraNPCs.Length; i++)
        {
            if (spawnedExtraNPCs[i] != null)
            {
                Destroy(spawnedExtraNPCs[i]);
            }
        }
        spawnedExtraNPCs = null; // 배열 초기화

        if (TimeManager.instance != null)
        {
            //시간과 플레이어 활성화
            TimeManager.instance.TimeTicking = true;
            player.gameObject.SetActive(true);
            player.enabled = true;
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<Kupa.Player>().ResetCamrea();

            NpcManager.Instance.Continue();
        }

        if(simulScene.index < 0) //만약 작업이였으면
        {
            UIManager.instance.TriggerSimulPrompt((simulScene.index * -1) -10); //미니게임 결과패널 띄우기

            TimeManager.instance.SkipTime(simulScene.skipTime); //패널 띄운 후 시간스킵하기, 무들이랑 상태 이상 보너스들 다 반영될수있게
        }
    }




    #region DefaultCutscene
    public void StartCutsceneSequence(Cutscene cutsceneToPlay)
    {
        //시간과 플레이어 비활성화
        if (TimeManager.instance != null)
        {
            TimeManager.instance.TimeTicking = false; //타임스토프
            player = FindAnyObjectByType<Kupa.Player>();
            player.enabled = false;
            player.GetComponent<CharacterController>().enabled = false;
            NpcManager.Instance.Pause();

            //reset the actors
            actors = new();

            //save to the blackboard
            GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
            blackboard.SetValue((CUTSCENE_PREFIX + cutsceneToPlay.name), true);
        }

        //convert into a queue
        actionsToExecute = new Queue<CutsceneAction>(cutsceneToPlay.action);

        UpdateCutscene();
    }
    public void UpdateCutscene()
    {
        //Check if there are any more actions in the queue
        if (actionsToExecute.Count == 0)
        {
            EndCutscene();
            
            return;
        }
        //Dequeue
        CutsceneAction actionToExecute = actionsToExecute.Dequeue();
        //액션 시퀀스 시작하고 한번 펑션 끝날때마다 호출
        actionToExecute.Init(() => { UpdateCutscene(); });
    }
    public void EndCutscene()
    {
        if(GameTimeStateManager.instance == null)
        {
            LoadingSceneController.Instance.LoadScene(nextSceneName);
            return;
        }
        Debug.Log("endddddddddddddddd");
        //clean up the actors
        ClearActors();

        if (TimeManager.instance != null)
        {
            //시간과 플레이어 활성화
            TimeManager.instance.TimeTicking = true;
            player.gameObject.SetActive(true);
            player.enabled = true;
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<Kupa.Player>().ResetCamrea();

            NpcManager.Instance.Continue();

            if (location == "HospitalCut") 
            {
                SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.Proto);
            
            }
        }
        
        onCutsceneStop?.Invoke();
    }

    void ClearActors()
    {
        foreach(var actor in actors)
        {
            if(actor.Key == "Player")
            {
                //내비메쉬만 파괴
                Destroy(actor.Value);
                continue;
            }

            Destroy(actor.Value.gameObject);
        }
        actors.Clear();
    }

    public static Cutscene GetCutsceneToPlay(Cutscene[] candidates)
    {
        Cutscene cutsceneToPlay = null;
        //가장 컨디션 점수 높은 컷신으로 교체
        int highestConditionScore = -1;
        foreach(Cutscene candidate in candidates)
        {
            if(GameTimeStateManager.instance == null)
            {
                return candidate;
            }

            //check if candidate is recurring
            if (!candidate.recurring)
            {
                 //블랙보드 키 겟
                 GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
                 //이벤트가 이미 플레이됬는지 체크
                 if (blackboard.ContainsKey(CUTSCENE_PREFIX + candidate.name)) continue;
                
            }


            if(candidate.CheckConditions(out int score))
            {
                if(score > highestConditionScore)
                {
                    highestConditionScore = score;
                    cutsceneToPlay = candidate;
                    Debug.Log("Will play + candidate.name");
                }
            }
        }
        return cutsceneToPlay;
    }

    public void AddOrMoveActor(string actor, Vector3 position,Quaternion rotation, Action onExecutionComplete)
    {
        //내비메쉬에서 장소 포지션 콘버트
        NavMesh.SamplePosition(position, out NavMeshHit hit, 10f, NavMesh.AllAreas);
        position = hit.position;
        Debug.Log($"CUTSCENE: trying to add/move {actor} on {position}");

        //액터의 무브먼트 컴포넌트
        NavMeshAgent actorMovement;
        bool actorExists = actors.TryGetValue(actor, out actorMovement);

        if (actorExists)
        {
            actorMovement.SetDestination(position);
            StartCoroutine(WaitForDestination(actorMovement, position, onExecutionComplete));
            return;
        }

        if(actor == "Player")
        {
            //give it a navmeshAgent
            actorMovement = player.gameObject.AddComponent<NavMeshAgent>();
            actors.Add("Player", actorMovement);
            actorMovement.SetDestination(position);
            StartCoroutine(WaitForDestination(actorMovement, position, onExecutionComplete));
            return;
        }
        Debug.Log($"CUTSCENE: {actor} doesnt exist. creating actor at {position}");
        //get npc
        NPC characterData = NpcManager.Instance.Characters().Find(x => x.name == actor);
        GameObject npcObj = Instantiate(characterData.prefab, position, rotation);
        actors.Add(actor, npcObj.GetComponent<NavMeshAgent>());
        onExecutionComplete?.Invoke();
    }

    public void KillActor(string actor)
    {
        GameObject objToDestroy = actors[actor].gameObject;
        actors.Remove(actor);
        Destroy(objToDestroy);
    }

    IEnumerator WaitForDestination(NavMeshAgent actorAgent, Vector3 destination, Action onExecutionComplete)
    {
        while(Vector3.SqrMagnitude(actorAgent.transform.position - destination) > 1f)
        {
            if (actorAgent == null) break;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("destination Complete");
        //mark execution as complete
        onExecutionComplete?.Invoke();
    }
}
#endregion
