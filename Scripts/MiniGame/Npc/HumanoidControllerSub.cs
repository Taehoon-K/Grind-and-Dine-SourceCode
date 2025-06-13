using UnityEngine;
using UnityEngine.AI;

public class HumanoidControllerSub : MonoBehaviour
{
    [Header("IK")]
    [Tooltip("If true then this script will control IK configuration of the character.")]
    public bool isIKActive = false;
    public Transform leftFootPosition = default;
    public Transform rightFootPosition = default;

    [Header("Interaction Offsets")]
    [SerializeField, Tooltip("An offset applied to the position of the character when they sit.")]
    protected float sittingOffset = 0.25f;
    protected int index;

    protected Quaternion desiredRotation = default;
    protected bool isRotating = false;

    protected Animator animator;

    [SerializeField]
    private bool isSub2;

    [SerializeField]
    protected GameObject character; //캐릭터 지정해서 랜덤 돌리기 함수 호출할 용도

    [SerializeField]
    protected GameObject chopsticks; //먹을 때 킬 젓가락 메쉬

    public bool isEat =false;

    protected NpcAnimator agent;

    public Collider[] cols = null;
    protected Rigidbody[] rbs = null;

    private void Start()
    {
        agent = GetComponent<NpcAnimator>();
        animator = GetComponent<Animator>();
        index = ChiTableManager.instance.tableId; //테이블 번호 받아옴

        cols = GetComponentsInChildren<Collider>();
        rbs = GetComponentsInChildren<Rigidbody>();

        Dead(false); //리지드바디 끄기
    }


    public virtual void Sit()
    {
        agent.enabled = false;
        Transform sitPosition;
        if (isSub2 && ChiTableManager.instance is BbqTableManager)
        {
            sitPosition = ((BbqTableManager)ChiTableManager.instance).tableSub2Chair[index].transform.GetChild(0); //의자 로테이션 각도 알아내기
            leftFootPosition = ((BbqTableManager)ChiTableManager.instance).tableSub2Chair[index].transform.GetChild(1);
            rightFootPosition = ((BbqTableManager)ChiTableManager.instance).tableSub2Chair[index].transform.GetChild(2);
        }
        else
        {
            sitPosition = ChiTableManager.instance.tableSubChair[index].transform.GetChild(0); //의자 로테이션 각도 알아내기
            leftFootPosition = ChiTableManager.instance.tableSubChair[index].transform.GetChild(1);
            rightFootPosition = ChiTableManager.instance.tableSubChair[index].transform.GetChild(2);
        }

        TurnTo(sitPosition.rotation); //중요

        // Vector3 pos = sitPosition.position;
        //pos.z -= sittingOffset; // slide back in the chair a little

        isIKActive = true;
        animator.SetBool("isSit", true);
        Vector3 newPosition = gameObject.transform.position;
        newPosition.x = sitPosition.position.x;
        newPosition.z = sitPosition.position.z;
        gameObject.transform.position = newPosition;
    }

    public void Stand()
    {
        agent.enabled = true;
        isRotating = false;

        isIKActive = false;
        animator.SetBool("isSit", false);
        animator.SetTrigger("isEatEnd");

    }

    protected void TurnTo(Quaternion rotation)
    {
        desiredRotation = rotation;
        isRotating = true;
    }

    protected void Update()
    {
        if (isRotating && transform.rotation != desiredRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, 0.05f);
        }
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            Dead(true);
        }*/
    }

    public virtual void Eat()
    {
        chopsticks.SetActive(true); //젓가락 키기
        animator.SetTrigger("isEat");//먹는 애니
        //ChiTableManager.instance.SetOrderUp(index, false); //테이블에 음식 못놓게 바꾸기
        Invoke(nameof(EatEnd),40f);//몇초 지나면 다먹고 나가는 함수 호출

    }
    protected virtual void EatEnd() //다먹었을때
    {
        chopsticks.SetActive(false);
        isEat = true; //비헤이비어 트리 호출용
        //ChiTableManager.instance.SetTableOn(index); //테이블매니저 다시 자리 키기 나중에 치우면 키게 바꿀수도?
    }
    public void Appearance()
    {
        character.GetComponent<AdvancedPeopleSystem.CharacterCustomization>().Randomize(); //랜덤 실행
    }

    public void Destroy()
    {
        Destroy(gameObject); //비헤이비어트리 마지막 호출
    }
    protected void OnAnimatorIK()
    {
        if (!isIKActive) return;

        if (rightFootPosition != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPosition.position);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootPosition.rotation);
        }
        if (leftFootPosition != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPosition.position);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootPosition.rotation);
        }
    }
    public virtual void Angry() //화났을시 나가는 코드, BBQ에서만 일단 쓸듯
    {
        chopsticks.SetActive(false);
        //ChiTableManager.instance.SetOrderUp(index, false); //테이블에 음식 못놓게 바꾸기
        ChiTableManager.instance.AngryOn(index); //테이블 오브젝트에 레이어 바꾸게 전달용도
        isEat = true;
    }

    public void Dead(bool isEnable) //뭐 맞으면 래그돌 되는 함수
    {
        
        if (isEnable)
        {
            ((BbqTableManager)ChiTableManager.instance).HumanoidAngry(index); //angry On
            isRotating = false;
            isIKActive = false;
            animator.SetBool("isSit", false);
            animator.enabled = !isEnable;

            GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().enabled = !isEnable;
            GetComponent<NavMeshAgent>().enabled = !isEnable;
            GetComponent<NpcAnimator>().enabled = !isEnable;
            agent.enabled = !isEnable; //npc Animator 스크립트 끄기
        }
        foreach (Collider col in cols)
        {
            col.enabled = isEnable;
        }
        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = isEnable;
            rb.isKinematic = !isEnable;
        }


    }
}

