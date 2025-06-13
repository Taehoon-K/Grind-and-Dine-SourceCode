using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterMovement))]
public class InteractableCharacter : MonoBehaviour
{
    public NPC npc;

    //원래 위치 저장용
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
        //위치 정보 얻기
        NPCLocationState locationState = NpcManager.Instance.GetNPCLocation(npc.CharacterName());
        movement.MoveTo(locationState);
        StartCoroutine(LookAt(Quaternion.Euler(locationState.facing)));
    }

    public void Pickup() //상호작용할 시
    {
        movement.ToggleMovement(false); //멈추기
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
        if(!isTurning) //원래 위치로 돌아갔을때만 defaultRota 업뎃
        {
            defaultRotation = transform.rotation; //원래 위치 저장
        }

        if (movement.isSit) //만약 앉아있는 경우
        {
            return;
        }

        //플레이어 위치 가져오기
        Transform player = FindObjectOfType<PlayerYarn>().transform;

        //거리계산
        Vector3 dir = player.position - transform.position;
        //npc안뒤집어지게 y축 고정
        dir.y = 0;
        //vector to quaternion
        Quaternion lookRot = Quaternion.LookRotation(dir);

        StartCoroutine(LookAt(lookRot));
    }

    IEnumerator LookAt(Quaternion lookRot) //자연스럽게 회전 위한 코루틴
    {
        //코루틴 실행중인지 확인
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
                yield break; //코루틴 종료
            }
            if (!movement.IsMoving()) //안움직일때만
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, 720 * Time.fixedDeltaTime);
            }

            yield return new WaitForFixedUpdate();
        }
        isTurning = false;
    }

    public void ResetRotation() //원래 위치로 회전 돌아가기
    {
        //StartCoroutine(LookAt(defaultRotation));
    }
    #endregion

    public bool EligibleForGift()
    {
        //만약 처음 만났다면
        if (RelationshipStats.FirstMeeting(npc))
        {
            return false;
        }

        //이미 선물횟수 채웠다면
        if (!RelationshipStats.GiftGivenPossible(npc))
        {
            return false;
        }

        return true;
    }

    public void UpdateIcon() //미니맵 아이콘 업데이트
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
