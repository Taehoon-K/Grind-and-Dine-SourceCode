using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterMovement))]
public class InteractableCharacter : MonoBehaviour
{
    public NPC npc;

    //���� ��ġ �����
    Quaternion defaultRotation;
    private bool isTurning =false;

    CharacterMovement movement;

    [Header("Minimap Icon")]
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite questionIcon;
    [SerializeField] private Sprite exclamationIcon;
    private SpriteRenderer iconImage;

    private void Start()
    {
        movement = GetComponent<CharacterMovement>();

        GameTimeStateManager.instance.onIntervalUpdate.AddListener(OnIntervalUpdate);

        iconImage = GetComponentInChildren<SynchronousRotation>().GetComponent<SpriteRenderer>();
        UpdateIcon();
    }
    public void OnIntervalUpdate()
    {
        //��ġ ���� ���
        NPCLocationState locationState = NpcManager.Instance.GetNPCLocation(npc.CharacterName());
        movement.MoveTo(locationState);
        StartCoroutine(LookAt(Quaternion.Euler(locationState.facing)));
    }

    public void Pickup() //��ȣ�ۿ��� ��
    {
        movement.ToggleMovement(false); //���߱�
        gameObject.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().PlayBlendshapeAnimation("Talk", 2);
        //LookAtPlayer();
    }
    public void ToggleOn()
    {
        movement.ToggleMovement(true);
    }

    #region Rotation
    void LookAtPlayer()
    {
        if(!isTurning) //���� ��ġ�� ���ư������� defaultRota ����
        {
            defaultRotation = transform.rotation; //���� ��ġ ����
        }

        if (movement.isSit) //���� �ɾ��ִ� ���
        {
            return;
        }

        //�÷��̾� ��ġ ��������
        Transform player = FindObjectOfType<PlayerYarn>().transform;

        //�Ÿ����
        Vector3 dir = player.position - transform.position;
        //npc�ȵ��������� y�� ����
        dir.y = 0;
        //vector to quaternion
        Quaternion lookRot = Quaternion.LookRotation(dir);

        StartCoroutine(LookAt(lookRot));
    }

    IEnumerator LookAt(Quaternion lookRot) //�ڿ������� ȸ�� ���� �ڷ�ƾ
    {
        //�ڷ�ƾ ���������� Ȯ��
        if (isTurning)
        {
            isTurning = false;
        }
        else
        {
            isTurning = true;
        }
        while(transform.rotation != lookRot)
        {
            if (!isTurning)
            {
                yield break; //�ڷ�ƾ ����
            }
            if (!movement.IsMoving()) //�ȿ����϶���
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, 720 * Time.fixedDeltaTime);
            }

            yield return new WaitForFixedUpdate();
        }
        isTurning = false;
    }

    public void ResetRotation() //���� ��ġ�� ȸ�� ���ư���
    {
        //StartCoroutine(LookAt(defaultRotation));
    }
    #endregion

    public bool EligibleForGift()
    {
        //���� ó�� �����ٸ�
        if (RelationshipStats.FirstMeeting(npc))
        {
            return false;
        }

        //�̹� ����Ƚ�� ä���ٸ�
        if (!RelationshipStats.GiftGivenPossible(npc))
        {
            return false;
        }

        return true;
    }

    public void UpdateIcon() //�̴ϸ� ������ ������Ʈ
    {
        bool isQuestTarget = QuestManager.instance.IsNpcCurrentQuestTarget(npc);
        bool isQuestGiver = QuestManager.instance.IsNpcQuestGiver(npc);

        if (isQuestTarget)
            iconImage.sprite = exclamationIcon; // !
        else if (isQuestGiver)
            iconImage.sprite = questionIcon;    // ?
        else
            iconImage.sprite = defaultIcon;
    }
}
