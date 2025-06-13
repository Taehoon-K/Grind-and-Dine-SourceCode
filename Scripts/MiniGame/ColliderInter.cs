using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderInter : MonoBehaviour
{ 
    public int[] requireId;
    public GameObject[] previewItem;
    //[SerializeField]
    public GameObject[] realItem;
    [SerializeField]
    private GameObject[] burnItem;
    [SerializeField]
    private GameObject[] wellItem;

    [SerializeField]
    private float cook;  // 최대 익힘 상태. 유니티 에디터 슬롯에서 지정할 것.
    private float currentCook;
    [SerializeField]
    private float burn;  // 최대 탐 상태.
    private float currentBurn;
    [SerializeField]
    private Image images_Gauge; //요리중인거 위에 게이지
    [SerializeField]
    private GameObject images_Cook; //요리중일때 키는 게이지 이미지
    [SerializeField] private float gaugeFillSpeed; //요리 속도
    [SerializeField] private GameObject fx;
    [SerializeField] private GameObject cookFx; //튀기는 소리 담은 오브젝트
    [SerializeField] private Color normalColor; // 게이지 기본 색상
    [SerializeField] private Color dangerColor; // 게이지가 빨개지는 색상

    public bool canPlaced
    {
        get { return canPla; }
        set
        {
            canPla = value;
        }
    } //이미 무언가 올라가 있는지 확인여부, 빌드매니저에서 사용중
    private bool canPla = true;
    private int index_;
    private bool isCook =false;  //요리중인지
    private GameObject objec; //튀김기 안의 오브젝트
    bool IsCook { get { return isCook; } set { isCook = value; animator.SetBool("isCook", value);
            images_Cook.SetActive(value); //이즈쿡 트루면 게이지 보이게
            if (!isCook)
            {
                //objec.tag = "ItemAnother"; //다시 원래 태그로
                objec.GetComponent<BoxCollider>().enabled = true;
                objec.GetComponent<MeshCollider>().enabled = true;
                //currentCook = 0; //게이지 초기화
                //currentBurn = 0;
            }
            else
            {
                //objec.tag = "Untagged"; //튀김기 사용 도중 못집게 태그 바꾸기
                objec.GetComponent<BoxCollider>().enabled = false;
                objec.GetComponent<MeshCollider>().enabled = false;
            }
        } } 
    private Animator animator;
    private bool isCoroutineRunning = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        currentCook = 0;
    }

    private void Update()
    {
        GaugeUpdate();
        CookGuage();
    }
    public bool CheckActive() //튀김기 사용 가능한지 여부
    {
        foreach (GameObject obj in realItem)
        {
            if (obj.activeSelf)
            {
                objec = obj;
                return true;
            }
        }
        foreach (GameObject obj in burnItem)
        {
            if (obj.activeSelf)
            {
                objec = obj;
                return true;
            }
        }
        foreach (GameObject obj in wellItem)
        {
            if (obj.activeSelf)
            {
                objec = obj;
                return true;
            }
        }
        return false;
    }  
    IEnumerator DelayCoroution()
    {
        yield return new WaitForSeconds(0.1f);
        realItem[index_].SetActive(true);
        Collider[] colliders = realItem[index_].GetComponentsInChildren<Collider>(); // 모든 콜라이더 컴포넌트를 찾음
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;  // 각 콜라이더의 활성화 상태를 false로 설정
        }
    }
    public void StartCo(int index)
    {
        index_ = index;
        StartCoroutine(DelayCoroution()); //감튀 내려놓자마자 바로 주워지는거 방지용
    }
    public void OnMachine()
    {
        StartCoroutine(OnMachineCo());

    }
    
    private IEnumerator OnMachineCo()  //튀김기 사용시 실행되는 함수
    {
        // 이전 코루틴이 실행 중인 경우, 실행 중지
        if (isCoroutineRunning)
            yield break;
        
        // 코루틴 실행
        isCoroutineRunning = true;
        IsCook = !IsCook;
        
        yield return new WaitForSeconds(0.5f);
        isCoroutineRunning = false;
    }
    private void GaugeUpdate()
    {
        images_Gauge.fillAmount = (float)currentCook / cook;
    }
    private void CookGuage()
    {
        if (IsCook)
        {
            cookFx.SetActive(true);
            if (currentCook<cook)
            {
                images_Gauge.color = normalColor;
                float fillAmount = gaugeFillSpeed * Time.deltaTime;
                currentCook += fillAmount;
                currentCook = Mathf.Clamp(currentCook, 0f, cook);
            }
            else //다 익혀졌는데 계속 익힐때
            {
                if (GetComponent<AudioSource>().enabled == false)
                {
                    GetComponent<AudioSource>().enabled = true; //타이머 소리
                }

                //위험 이미지 띄우기

                if (currentBurn >= burn) //타면
                {
                    burnItem[index_].SetActive(true);
                    wellItem[index_].SetActive(false);
                    
                }
                else //익으면
                {
                    fx.SetActive(true); //연기 생성

                    wellItem[index_].SetActive(true);
                    realItem[index_].SetActive(false); 
                    
                    float fillAmount = gaugeFillSpeed * Time.deltaTime;
                    currentBurn += fillAmount;
                    currentBurn = Mathf.Clamp(currentBurn, 0f, burn);

                    // 게이지가 초과됨을 표현
                    images_Gauge.fillAmount = 1f;

                    // 익힌 상태에서부터 게이지 색상 변경 (초록 -> 빨강)
                    images_Gauge.color = Color.Lerp(normalColor, dangerColor, currentBurn / burn);
                }
                
            }

        }
        else
        {
            fx.SetActive(false); //연기 생성
            cookFx.SetActive(false);
            if (GetComponent<AudioSource>().enabled == true)
            {
                GetComponent<AudioSource>().enabled = false; //타이머 소리
            }
        }
    }
    
    public void Initilaize()
    {
        if (!CheckActive())
        {
            currentCook = 0; //게이지 초기화
            currentBurn = 0;
        }
    }
}
