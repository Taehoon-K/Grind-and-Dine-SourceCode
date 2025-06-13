using UnityEngine;

public class HumanoidController : HumanoidControllerSub
{

    protected bool subOn=false; //서브 생성 여부
    protected bool drinkOn=false; //음료수 생성 여부
    protected int mMenu, sMenu, dMenu;


    [SerializeField]
    protected GameObject chatBubble_main;
    [SerializeField]
    protected GameObject chatBubble_sub;
    [SerializeField]
    protected GameObject chatBubble_drink;



    protected void Start()
    {
        agent = GetComponent<NpcAnimator>();
        animator = GetComponent<Animator>();
        index = ChiTableManager.instance.tableId; //테이블 번호 받아옴
        subOn = ChiTableManager.instance.subChatOn; //서브메뉴 여부 받아옴
        drinkOn = ChiTableManager.instance.drinkChatOn; //드링크메뉴 여부 받아옴

        mMenu = ChiTableManager.instance.mainMenu; //메뉴 번호 받아오기
        sMenu = ChiTableManager.instance.subMenu;
        dMenu = ChiTableManager.instance.drinkMenu;

        cols = GetComponentsInChildren<Collider>();
        rbs = GetComponentsInChildren<Rigidbody>();
        Dead(false);
    }


    public override void Sit()
    {
        agent.enabled = false;
        Transform sitPosition;
        sitPosition = ChiTableManager.instance.tableChair[index].transform.GetChild(0); //의자 로테이션 각도 알아내기
        leftFootPosition = ChiTableManager.instance.tableChair[index].transform.GetChild(1);
        rightFootPosition = ChiTableManager.instance.tableChair[index].transform.GetChild(2);
        TurnTo(sitPosition.rotation); //중요

        isIKActive = true;
        animator.SetBool("isSit", true);
        Vector3 newPosition = gameObject.transform.position;
        newPosition.x = sitPosition.position.x;
        newPosition.z = sitPosition.position.z;
        gameObject.transform.position = newPosition;
    }


    public virtual void Hello() //비헤이비어 트리에서 호출
    {
        animator.SetTrigger("isHello");
        ChiTableManager.instance.SetOrderUp(index,true); //이 테이블에 음식 놓을 수 있음
        chatBubble_main.SetActive(true); //말풍선 키기
        chatBubble_main.transform.GetChild(mMenu).gameObject.SetActive(true); //메인메뉴 번호따라 자식 오브젝트 키기
        if (subOn)
        {
            chatBubble_sub.SetActive(true);
            chatBubble_sub.transform.GetChild(sMenu).gameObject.SetActive(true); //서브메뉴 번호따라 자식 오브젝트 키기
        }
        if (drinkOn)
        {
            chatBubble_drink.SetActive(true);
            chatBubble_drink.transform.GetChild(dMenu).gameObject.SetActive(true); //드링크메뉴 번호따라 자식 오브젝트 키기
        }
        SoundManager.instance.PlaySound3D("blabla", transform);
       // GetComponent<AudioSource>().enabled = true; //웅얼웅얼 오디오 키기
    }
    public override void Eat()
    {
        base.Eat();
       // animator.SetTrigger("isEat");//먹는 애니
        ChiTableManager.instance.SetOrderUp(index, false); //테이블에 음식 못놓게 바꾸기
        chatBubble_main.SetActive(false);//말풍선 끄기
        chatBubble_sub.SetActive(false);
        chatBubble_drink.SetActive(false);
        //Invoke(nameof(EatEnd),20f);//몇초 지나면 다먹고 나가는 함수 호출

    }
    public override void Angry()
    {
        base.Angry();
        chatBubble_main.SetActive(false);//말풍선 끄기
        chatBubble_sub.SetActive(false);
        chatBubble_drink.SetActive(false);
        SoundManager.instance.PlaySound3D("angry"+SoundManager.Range(1,4), transform);
    }
    protected override void EatEnd()
    {
        base.EatEnd();
        ChiTableManager.instance.SetTableOn(index); //테이블매니저 다시 자리 키기 나중에 치우면 키게 바꿀수도?
    }
}

