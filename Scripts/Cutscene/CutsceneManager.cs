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

    //���� �ִ� ��� ���� ����Ʈ
    Dictionary<string, NavMeshAgent> actors;
    //�÷��̾� ������
    [SerializeField] Kupa.Player player;

    [SerializeField] string sceneName;
    [SerializeField] string nextSceneName;

    [Header("SimulScene")]
    [SerializeField] private GameObject manExtra, womanExtra;
    private GameObject[] spawnedExtraNPCs; // ������ NPC���� ������ �迭
    GameObject prefab; //�÷��̾� ������
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
        if (GameTimeStateManager.instance == null && sceneName != "StartBus") //���� ù ���� �ƽ��̸�, �׸��� ��ŸƮ������ ����, ���÷��� ��Ŭ�� ������ ȣ���ų��
        {
            OnLocationLoad();
        }
    }
    const string CUTSCENE_PREFIX = "Cutscene";

    Queue<CutsceneAction> actionsToExecute;
    public Action onCutsceneStop;

    public void StartCut(int time) //����Ƽ �̺�Ʈ�� ����, ��Ʈ���Ŵ��� ������ �ƽ� �ߵ���Ű�� �뵵
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

        //�ƽ� �ִ��� üũ
        if (!cutsceneToPlay) return;
        Debug.Log($"the cutscene to play is {cutsceneToPlay.name}");

        StartCutsceneSequence(cutsceneToPlay);
    }



    public void StartSimulateScene(Simulscene simulsceneToPlay, bool isRisk = false, Action actionRisk = null, Action actionNone = null,int whatSimul = 0,bool isCriminal = false) //����ũ �ൿ�� ���� �׼� �ΰ�
    {
        //�ð��� �÷��̾� ��Ȱ��ȭ
        if (TimeManager.instance != null)
        {
            TimeManager.instance.TimeTicking = false; //Ÿ�ӽ�����
            player = FindAnyObjectByType<Kupa.Player>();
            player.enabled = false;
            player.GetComponent<CharacterController>().enabled = false;
            NpcManager.Instance.Pause();
        }

        if (!simulsceneToPlay.isPrefab) //�׳� �ù��̸�
        {
            if (PlayerStats.IsWoman) //���ڸ�
            {
                prefab = Instantiate(womanExtra, simulsceneToPlay.playerGenPoint, simulsceneToPlay.playerGenRotation);//����

            }
            else //���ڸ�
            {
                prefab = Instantiate(manExtra, simulsceneToPlay.playerGenPoint, simulsceneToPlay.playerGenRotation);//����
            }
            /*AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponent<AdvancedPeopleSystem.CharacterCustomization>();
            var saved = cc.GetSavedCharacterDatas();

            // �ش� �̸��� ���� �����Ͱ� �ִ��� Ȯ��
            for (int i = 0; i < saved.Count; i++)
            {
                //Debug.Log("Character data appliedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa for: " + saved[i].name);
                if (saved[i].name == 5.ToString())
                {
                    cc.ApplySavedCharacterData(saved[i]); // ���ϴ� �̸��� ĳ���� ������ ����
                    Debug.Log("Character data applied for: " + saved[i].name);
                    break; // ã���� �� �̻� �ݺ����� �ʰ� ����
                }
            }
            prefab.layer = 0;
            if (prefab.GetComponent<Animator>() == null)
            {
                prefab.AddComponent<Animator>();
                Debug.Log("Animator ������Ʈ�� ��� ���� �߰��߽��ϴ�.");
            }
            prefab.GetComponent<Animator>().runtimeAnimatorController = simulsceneToPlay.playerAnimator; //�ִϸ��̼� ��Ʈ�ѷ� ����
            prefab.GetComponent<Animator>().avatar = player.GetComponent<Animator>().avatar; //�÷��̾� �ù�ĳ���� �ƹ�Ÿ ����
            foreach (Transform child in prefab.GetComponentsInChildren<Transform>(true)) //�ڽĿ�����Ʈ�� ���̾�鵵 �� ����Ʈ�� �ٲ�
            {
                child.gameObject.layer = 0;
            }*/
        }
        else //���� ���� �� ������ ����Ÿ�
        {
            if (PlayerStats.IsWoman) //���ڸ�
            {
                prefab = Instantiate(simulsceneToPlay.woman, simulsceneToPlay.playerGenPoint, simulsceneToPlay.playerGenRotation);//����

            }
            else //���ڸ�
            {
                prefab = Instantiate(simulsceneToPlay.man, simulsceneToPlay.playerGenPoint, simulsceneToPlay.playerGenRotation);//����
            }
            
        }
        AdvancedPeopleSystem.CharacterCustomization cc = prefab.GetComponent<AdvancedPeopleSystem.CharacterCustomization>();
        var saved = cc.GetSavedCharacterDatas();

        // �ش� �̸��� ���� �����Ͱ� �ִ��� Ȯ��
        for (int i = 0; i < saved.Count; i++)
        {
            //Debug.Log("Character data appliedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa for: " + saved[i].name);
            if (saved[i].name == 5.ToString())
            {
                cc.ApplySavedCharacterData(saved[i]); // ���ϴ� �̸��� ĳ���� ������ ����
                Debug.Log("Character data applied for: " + saved[i].name);
                break; // ã���� �� �̻� �ݺ����� �ʰ� ����
            }
        }
        prefab.layer = 0;
        if (prefab.GetComponent<Animator>() == null)
        {
            prefab.AddComponent<Animator>();
            Debug.Log("Animator ������Ʈ�� ��� ���� �߰��߽��ϴ�.");
        }
        prefab.GetComponent<Animator>().runtimeAnimatorController = simulsceneToPlay.playerAnimator; //�ִϸ��̼� ��Ʈ�ѷ� ����
        prefab.GetComponent<Animator>().avatar = player.GetComponent<Animator>().avatar; //�÷��̾� �ù�ĳ���� �ƹ�Ÿ ����
        foreach (Transform child in prefab.GetComponentsInChildren<Transform>(true)) //�ڽĿ�����Ʈ�� ���̾�鵵 �� ����Ʈ�� �ٲ�
        {
            child.gameObject.layer = 0;
        }

        prefab.GetComponent<Animator>().Rebind();
        prefab.GetComponent<Animator>().Update(0); // ������Ʈ�� ������ ȣ���� ��� �ݿ�



        spawnedExtraNPCs = new GameObject[simulsceneToPlay.extraSimulInfomations.Length]; // �迭 ũ�� ����
        for (int i = 0; i < simulsceneToPlay.extraSimulInfomations.Length; i++)
        {
            GameObject npc;
            ExtraSimulInfomation info = simulsceneToPlay.extraSimulInfomations[i];

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                // NPC ����
                npc = Instantiate(womanExtra, info.extraGenPoint, info.extraGenRotation);
                npc.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().Randomize();
                spawnedExtraNPCs[i] = npc; // �迭�� ����
            }
            else
            {
                npc = Instantiate(manExtra, info.extraGenPoint, info.extraGenRotation);
                npc.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().Randomize();
                spawnedExtraNPCs[i] = npc; // �迭�� ����
            }

            npc.layer = 0;
            // �ִϸ����� ��Ʈ�ѷ� ����
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
            UIManager.instance?.OnSimulGauge(simulsceneToPlay.localizedText); //�ùķ��̼� ������ Ű��

            if (simulsceneToPlay.index >= 0) //�۾��� �ÿ��� �������� ���⼭ ���ϱ�
            {
                StatusManager.instance.SimulationCalcul(simulsceneToPlay.index); //���� ���� ����
                TimeManager.instance.SkipTime(simulsceneToPlay.skipTime); //�ð� ��ŵ, �۾��� �� �ð� ��ŵ ���߿� �ϱ�
            }
            StartCoroutine(InvokeDMethodAfterDelay(8.1f)); // 8�� �� ������ �Լ� ����
        }
        else  //���� ����ũ �ִ� �ൿ�̸�
        {
            CutsceneCamManager.Instance.OrbitAroundObject(simulsceneToPlay.CameraStartPosition, newPosition, 0.01f); //ī�޶� ����

            if (simulsceneToPlay.index >= 0) //�ε��� 0 �̻�
            {
                StatusManager.instance.SimulationRiskCalcul(simulsceneToPlay.index); //���� ���� ����
            }
            TimeManager.instance.SkipTime(simulsceneToPlay.skipTime); //�ð� ��ŵ, �۾��� �� �ð� ��ŵ ���߿� �ϱ�
            UIManager.instance.TriggerCardPanel(whatSimul, (bool isCaught) =>
            {
                if (isCaught) //���� ����ũ �ɷȴٸ�
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
        spawnedExtraNPCs = null; // �迭 �ʱ�ȭ

        if (TimeManager.instance != null)
        {
            //�ð��� �÷��̾� Ȱ��ȭ
            TimeManager.instance.TimeTicking = true;
            player.gameObject.SetActive(true);
            player.enabled = true;
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<Kupa.Player>().ResetCamrea();

            NpcManager.Instance.Continue();
        }

        if(simulScene.index < 0) //���� �۾��̿�����
        {
            UIManager.instance.TriggerSimulPrompt((simulScene.index * -1) -10); //�̴ϰ��� ����г� ����

            TimeManager.instance.SkipTime(simulScene.skipTime); //�г� ��� �� �ð���ŵ�ϱ�, �����̶� ���� �̻� ���ʽ��� �� �ݿ��ɼ��ְ�
        }
    }




    #region DefaultCutscene
    public void StartCutsceneSequence(Cutscene cutsceneToPlay)
    {
        //�ð��� �÷��̾� ��Ȱ��ȭ
        if (TimeManager.instance != null)
        {
            TimeManager.instance.TimeTicking = false; //Ÿ�ӽ�����
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
        //�׼� ������ �����ϰ� �ѹ� ��� ���������� ȣ��
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
            //�ð��� �÷��̾� Ȱ��ȭ
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
                //����޽��� �ı�
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
        //���� ����� ���� ���� �ƽ����� ��ü
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
                 //������ Ű ��
                 GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();
                 //�̺�Ʈ�� �̹� �÷��̉���� üũ
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
        //����޽����� ��� ������ �ܹ�Ʈ
        NavMesh.SamplePosition(position, out NavMeshHit hit, 10f, NavMesh.AllAreas);
        position = hit.position;
        Debug.Log($"CUTSCENE: trying to add/move {actor} on {position}");

        //������ �����Ʈ ������Ʈ
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
