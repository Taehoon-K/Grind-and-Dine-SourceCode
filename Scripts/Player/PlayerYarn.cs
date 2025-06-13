using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using PP.InventorySystem;
using System;
using System.Linq;
#if USE_INPUTSYSTEM && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class PlayerYarn : MonoBehaviour
{

    public float interactionRadius = 2.0f;

    private DialogueAdvanceInput dialogueInput;
    Kupa.Player player;

    private bool isCutscene; //컷신일시
    private Action cutscene;
    //public bool giftSucceedYarn { get; private set; } //선물 받았을 시

    InteractableCharacter character; //대화하는 중인 npc 저장
    void Start()
    {
        dialogueInput = FindObjectOfType<DialogueAdvanceInput>();
        dialogueInput.enabled = false;
        player = FindObjectOfType<Kupa.Player>();

    }


    void Update()
    {
        // Remove all player control when we're in dialogue
        if (FindObjectOfType<DialogueRunner>().IsDialogueRunning == true)
        {
            return;
        }

        // every time we LEAVE dialogue we have to make sure we disable the input again
        if (dialogueInput.enabled)
        {
            dialogueInput.enabled = false;

        }



        // Detect if we want to start a conversation
#if USE_INPUTSYSTEM && ENABLE_INPUT_SYSTEM
            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                CheckForNearbyNPC ();
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
        /*if (Input.GetKeyUp(KeyCode.Q)&&player.isRun==false)
        {
            CheckForNearbyNPC();
        }*/
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }*/
#endif
    }

    public void StartCutsceneSpeech(string node, Action aa) //컷신 다이얼로그 시작용
    {
        isCutscene = true;
        cutscene = aa;
        FindObjectOfType<DialogueRunner>().StartDialogue(node);
        dialogueInput.enabled = true;   
    }
    public void CheckForNearbyNPC(InteractableCharacter target)
    {
        if (target != null)
        {
            player.LookNpc(target.transform); //npc 바라보기

            Debug.Log("CheckFoorNearByNpc");
            character = target;
            target.Pickup(); //상대 npc 상호작용 스크립트 호출
            string node = QuestManager.instance.CheckNpcInteractionCondition(target.npc); //해당 npc와 관련있는 퀘스트 진행중인지 검사
            var eventDialogue = CheckEventDialogue(target.npc.dialogues);
            if (RelationshipStats.FirstMeeting(target.npc)) //npc 처음 만나는건지 검사
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeFirst());
                dialogueInput.enabled = true;
                RelationshipStats.UnlockCharacter(target.npc);
            }        
            else if(!string.IsNullOrEmpty(node)) //해당 npc와 관련있는 퀘스트 진행중인지 검사
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(node);
                dialogueInput.enabled = true;
            }
            else if (RelationshipStats.IsFirstConversationOfTheDay(target.npc)) //그날 첫 대화인지
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeFirstOfDay());
                dialogueInput.enabled = true;
                RelationshipStats.AddFriendPoints(target.npc, 20);

                NPCRelationshipState relationship = RelationshipStats.GetRelationship(target.npc);
                relationship.hasTalkedToday = true;

                StatusManager.instance.AddExperience(4, 40); //카리스마 경험치 40 추가
            }
            else if (eventDialogue != null) // 이벤트 대화인지 검사
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(eventDialogue);
                dialogueInput.enabled = true;

                RelationshipStats.AddFriendPoints(target.npc, 20);
                NPCRelationshipState relationship = RelationshipStats.GetRelationship(target.npc);
                //relationship.hasTalkedToday = true; //그날 첫 대화 겸 계속 진행
            }  
            else
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNode());
                dialogueInput.enabled = true;
            }
        }
    }

    public void CheckForNearbyNPC(string target,Transform targetTrans)
    {
        
        if (target != null)
        {
            player.LookNpc(targetTrans); //npc 바라보기

            FindObjectOfType<DialogueRunner>().StartDialogue(target);
            dialogueInput.enabled = true;
        }
    }

    public bool CheckForNearbyNPCGift(ItemData item, InteractableCharacter target)
    {
       /* var allParticipants = new List<InteractableCharacter>(FindObjectsOfType<InteractableCharacter>());
        var target = allParticipants.Find(delegate (InteractableCharacter p)
        {
            return string.IsNullOrEmpty(p.npc.TalkToNode()) == false && // has a conversation node?
            (p.transform.position - this.transform.position)// is in range?
            .magnitude <= interactionRadius;
        });*/
        if (target != null)
        {
            target.Pickup(); //상대 npc 상호작용 스크립트 호출

            string node = QuestManager.instance.CheckItemDeliveryCondition(item.ID, target.npc);
            if (!string.IsNullOrEmpty(node)) 
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(node);
                dialogueInput.enabled = true;
                return true;
            }

            if(item.ID == 60 && !RelationshipStats.GetRelationship(target.npc).loveWith) //만약 부케라면, 그리고 사귀는 사이 아니라면
            {
                NPCRelationshipState relation = RelationshipStats.GetRelationship(target.npc);
                if (target.npc.canLove && relation.friendshipPoints >= 2000) //만약 연애 가능 상대라면
                {
                    if (relation.villain) //빌런일 시
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeReject());
                        dialogueInput.enabled = true;
                        RelationshipStats.AddFriendPoints(target.npc, -2500); //하트 -2칸으로 감소
                    }
                    else //빌런 아닐 시
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeAccept());
                        dialogueInput.enabled = true;
                        RelationshipStats.UpdateLoveStatus(target.npc, true); //사귐 상태로 바꿈
                    }
                    return true; //꽃다발 받아감
                }
                else
                {
                    FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeImposs());
                    dialogueInput.enabled = true;
                    //불가 멘트
                    return false;
                }
                
            }

            if (!target.EligibleForGift()) //만약 안만났거나 횟수 안되서 못하면
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeNoGift());
                dialogueInput.enabled = true;
                return false;
            }

            bool isBirthday = RelationshipStats.IsBirthday(target.npc);

            //우정포인트 계산
            int pointsToAdd = 0;
            switch (RelationshipStats.GetReactionToGift(target.npc, item))
            {
                case RelationshipStats.GiftReaction.Best:
                    if (isBirthday)
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeBirthdayGood());
                        dialogueInput.enabled = true;
                    }
                    else
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeBest());
                        dialogueInput.enabled = true;
                    }
                    pointsToAdd = 80;
                    break;


                case RelationshipStats.GiftReaction.Good:
                    if (isBirthday)
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeBirthdayGood());
                        dialogueInput.enabled = true;
                    }
                    else
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeGood());
                        dialogueInput.enabled = true;
                    }
                    pointsToAdd = 45;
                    break;


                case RelationshipStats.GiftReaction.Soso:
                    if (isBirthday)
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeBirthdaySoso());
                        dialogueInput.enabled = true;
                    }
                    else
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeSoso());
                        dialogueInput.enabled = true;
                    }
                    pointsToAdd = 20;
                    break;


                case RelationshipStats.GiftReaction.Bad:
                    if (isBirthday)
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeBirthdayBad());
                        dialogueInput.enabled = true;
                    }
                    else
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeBad());
                        dialogueInput.enabled = true;
                    }
                    pointsToAdd = -20;
                    break;


                case RelationshipStats.GiftReaction.Worst:
                    if (isBirthday)
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeBirthdayBad());
                        dialogueInput.enabled = true;
                    }
                    else
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeWorst());
                        dialogueInput.enabled = true;
                    }
                    pointsToAdd = -40;
                    break;


                case RelationshipStats.GiftReaction.Impossible:
                    FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeNoGift());
                    dialogueInput.enabled = true;
                    return false;
                    break;

            }
            if (isBirthday)
            {
                pointsToAdd *= 8;
            }
            RelationshipStats.AddFriendPoints(target.npc, pointsToAdd); //호감도 증가
            RelationshipStats.GiftGivenSucess(target.npc); //횟수 올라가게
            return true;
        }
        return false;
    }
    public void CompleteDialogue() //다이얼로그끝나면 이벤트로 호출될함수
    {
        if (isCutscene)
        {
            isCutscene = false;
            cutscene?.Invoke();
            cutscene = null;
            return;
        }
        if(character != null)
        {
            character.ResetRotation();
            character.ToggleOn();
            character.OnIntervalUpdate(); //다시 걷게
        }

        player.StopLooking(); //바라보는 코루틴 종료시키기
    }

    public string CheckEventDialogue(DialogueCondition[] conditions) //이벤트 만족하는 경우 있는지 체크
    {
        string node = null;
        int highestConditionScore = -1;
        foreach(DialogueCondition condition in conditions)
        {
            //가장 우선순위 높은 컨디션 찾기
            if(condition.CheckConditions(out int score))
            {
                if(score > highestConditionScore)
                {
                    highestConditionScore = score;
                    node = condition.yarnString;
                    Debug.Log("Will Play: " + condition.id);
                }
            }
        }

        return node;
    }
}


