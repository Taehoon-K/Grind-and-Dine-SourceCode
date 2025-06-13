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
    private float cook;  // �ִ� ���� ����. ����Ƽ ������ ���Կ��� ������ ��.
    private float currentCook;
    [SerializeField]
    private float burn;  // �ִ� Ž ����.
    private float currentBurn;
    [SerializeField]
    private Image images_Gauge; //�丮���ΰ� ���� ������
    [SerializeField]
    private GameObject images_Cook; //�丮���϶� Ű�� ������ �̹���
    [SerializeField] private float gaugeFillSpeed; //�丮 �ӵ�
    [SerializeField] private GameObject fx;
    [SerializeField] private GameObject cookFx; //Ƣ��� �Ҹ� ���� ������Ʈ
    [SerializeField] private Color normalColor; // ������ �⺻ ����
    [SerializeField] private Color dangerColor; // �������� �������� ����

    public bool canPlaced
    {
        get { return canPla; }
        set
        {
            canPla = value;
        }
    } //�̹� ���� �ö� �ִ��� Ȯ�ο���, ����Ŵ������� �����
    private bool canPla = true;
    private int index_;
    private bool isCook =false;  //�丮������
    private GameObject objec; //Ƣ��� ���� ������Ʈ
    bool IsCook { get { return isCook; } set { isCook = value; animator.SetBool("isCook", value);
            images_Cook.SetActive(value); //������ Ʈ��� ������ ���̰�
            if (!isCook)
            {
                //objec.tag = "ItemAnother"; //�ٽ� ���� �±׷�
                objec.GetComponent<BoxCollider>().enabled = true;
                objec.GetComponent<MeshCollider>().enabled = true;
                //currentCook = 0; //������ �ʱ�ȭ
                //currentBurn = 0;
            }
            else
            {
                //objec.tag = "Untagged"; //Ƣ��� ��� ���� ������ �±� �ٲٱ�
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
    public bool CheckActive() //Ƣ��� ��� �������� ����
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
        Collider[] colliders = realItem[index_].GetComponentsInChildren<Collider>(); // ��� �ݶ��̴� ������Ʈ�� ã��
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;  // �� �ݶ��̴��� Ȱ��ȭ ���¸� false�� ����
        }
    }
    public void StartCo(int index)
    {
        index_ = index;
        StartCoroutine(DelayCoroution()); //��Ƣ �������ڸ��� �ٷ� �ֿ����°� ������
    }
    public void OnMachine()
    {
        StartCoroutine(OnMachineCo());

    }
    
    private IEnumerator OnMachineCo()  //Ƣ��� ���� ����Ǵ� �Լ�
    {
        // ���� �ڷ�ƾ�� ���� ���� ���, ���� ����
        if (isCoroutineRunning)
            yield break;
        
        // �ڷ�ƾ ����
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
            else //�� �������µ� ��� ������
            {
                if (GetComponent<AudioSource>().enabled == false)
                {
                    GetComponent<AudioSource>().enabled = true; //Ÿ�̸� �Ҹ�
                }

                //���� �̹��� ����

                if (currentBurn >= burn) //Ÿ��
                {
                    burnItem[index_].SetActive(true);
                    wellItem[index_].SetActive(false);
                    
                }
                else //������
                {
                    fx.SetActive(true); //���� ����

                    wellItem[index_].SetActive(true);
                    realItem[index_].SetActive(false); 
                    
                    float fillAmount = gaugeFillSpeed * Time.deltaTime;
                    currentBurn += fillAmount;
                    currentBurn = Mathf.Clamp(currentBurn, 0f, burn);

                    // �������� �ʰ����� ǥ��
                    images_Gauge.fillAmount = 1f;

                    // ���� ���¿������� ������ ���� ���� (�ʷ� -> ����)
                    images_Gauge.color = Color.Lerp(normalColor, dangerColor, currentBurn / burn);
                }
                
            }

        }
        else
        {
            fx.SetActive(false); //���� ����
            cookFx.SetActive(false);
            if (GetComponent<AudioSource>().enabled == true)
            {
                GetComponent<AudioSource>().enabled = false; //Ÿ�̸� �Ҹ�
            }
        }
    }
    
    public void Initilaize()
    {
        if (!CheckActive())
        {
            currentCook = 0; //������ �ʱ�ȭ
            currentBurn = 0;
        }
    }
}
