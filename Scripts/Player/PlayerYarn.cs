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

    private bool isCutscene; //�ƽ��Ͻ�
    private Action cutscene;
    //public bool giftSucceedYarn { get; private set; } //���� �޾��� ��

    InteractableCharacter character; //��ȭ�ϴ� ���� npc ����
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

    public void StartCutsceneSpeech(string node, Action aa) //�ƽ� ���̾�α� ���ۿ�
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
            player.LookNpc(target.transform); //npc �ٶ󺸱�

            Debug.Log("CheckFoorNearByNpc");
            character = target;
            target.Pickup(); //��� npc ��ȣ�ۿ� ��ũ��Ʈ ȣ��
            string node = QuestManager.instance.CheckNpcInteractionCondition(target.npc); //�ش� npc�� �����ִ� ����Ʈ ���������� �˻�
            var eventDialogue = CheckEventDialogue(target.npc.dialogues);
            if (RelationshipStats.FirstMeeting(target.npc)) //npc ó�� �����°��� �˻�
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeFirst());
                dialogueInput.enabled = true;
                RelationshipStats.UnlockCharacter(target.npc);
            }        
            else if(!string.IsNullOrEmpty(node)) //�ش� npc�� �����ִ� ����Ʈ ���������� �˻�
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(node);
                dialogueInput.enabled = true;
            }
            else if (RelationshipStats.IsFirstConversationOfTheDay(target.npc)) //�׳� ù ��ȭ����
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeFirstOfDay());
                dialogueInput.enabled = true;
                RelationshipStats.AddFriendPoints(target.npc, 20);

                NPCRelationshipState relationship = RelationshipStats.GetRelationship(target.npc);
                relationship.hasTalkedToday = true;

                StatusManager.instance.AddExperience(4, 40); //ī������ ����ġ 40 �߰�
            }
            else if (eventDialogue != null) // �̺�Ʈ ��ȭ���� �˻�
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(eventDialogue);
                dialogueInput.enabled = true;

                RelationshipStats.AddFriendPoints(target.npc, 20);
                NPCRelationshipState relationship = RelationshipStats.GetRelationship(target.npc);
                //relationship.hasTalkedToday = true; //�׳� ù ��ȭ �� ��� ����
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
            player.LookNpc(targetTrans); //npc �ٶ󺸱�

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
            target.Pickup(); //��� npc ��ȣ�ۿ� ��ũ��Ʈ ȣ��

            string node = QuestManager.instance.CheckItemDeliveryCondition(item.ID, target.npc);
            if (!string.IsNullOrEmpty(node)) 
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(node);
                dialogueInput.enabled = true;
                return true;
            }

            if(item.ID == 60 && !RelationshipStats.GetRelationship(target.npc).loveWith) //���� ���ɶ��, �׸��� ��ʹ� ���� �ƴ϶��
            {
                NPCRelationshipState relation = RelationshipStats.GetRelationship(target.npc);
                if (target.npc.canLove && relation.friendshipPoints >= 2000) //���� ���� ���� �����
                {
                    if (relation.villain) //������ ��
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeReject());
                        dialogueInput.enabled = true;
                        RelationshipStats.AddFriendPoints(target.npc, -2500); //��Ʈ -2ĭ���� ����
                    }
                    else //���� �ƴ� ��
                    {
                        FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeAccept());
                        dialogueInput.enabled = true;
                        RelationshipStats.UpdateLoveStatus(target.npc, true); //��� ���·� �ٲ�
                    }
                    return true; //�ɴٹ� �޾ư�
                }
                else
                {
                    FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeImposs());
                    dialogueInput.enabled = true;
                    //�Ұ� ��Ʈ
                    return false;
                }
                
            }

            if (!target.EligibleForGift()) //���� �ȸ����ų� Ƚ�� �ȵǼ� ���ϸ�
            {
                FindObjectOfType<DialogueRunner>().StartDialogue(target.npc.TalkToNodeNoGift());
                dialogueInput.enabled = true;
                return false;
            }

            bool isBirthday = RelationshipStats.IsBirthday(target.npc);

            //��������Ʈ ���
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
            RelationshipStats.AddFriendPoints(target.npc, pointsToAdd); //ȣ���� ����
            RelationshipStats.GiftGivenSucess(target.npc); //Ƚ�� �ö󰡰�
            return true;
        }
        return false;
    }
    public void CompleteDialogue() //���̾�α׳����� �̺�Ʈ�� ȣ����Լ�
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
            character.OnIntervalUpdate(); //�ٽ� �Ȱ�
        }

        player.StopLooking(); //�ٶ󺸴� �ڷ�ƾ �����Ű��
    }

    public string CheckEventDialogue(DialogueCondition[] conditions) //�̺�Ʈ �����ϴ� ��� �ִ��� üũ
    {
        string node = null;
        int highestConditionScore = -1;
        foreach(DialogueCondition condition in conditions)
        {
            //���� �켱���� ���� ����� ã��
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


