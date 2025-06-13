using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ThrowingMini : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public Transform trashPoint;
    public GameObject[] objectToThrow;

    [Header("Settings")]
    public float throwCooldown;

    [Header("Throwing")]
    public float throwForce;
    public float throwUpwardForce;

    public bool readyToThrow { get; set; }
    bool tDown;
    private Animator animatorFirst;

    [SerializeField]
    private float throwInvoke;    
    [SerializeField]
    private float drinkInvoke;
    [HideInInspector]
    public int equipID;
    [SerializeField]
    private GameObject[] objectToEquipF;
    [SerializeField]
    private GameObject[] washDishes;

    [SerializeField]
    private GameObject animator;

    public UnityEvent<int> onPlace; //������ ��ġ �̺�Ʈ
    private CameraRayMini cameraRayMini;

    public int id_;
    private void Awake()
    {
        //animator = GetComponent<Animator>();
        animatorFirst = animator.GetComponent<Animator>();
        cameraRayMini = GetComponent<CameraRayMini>();
    }

    private void Start()
    {
        readyToThrow = false;
    }

    /*private void Update()
    {
        tDown = Input.GetButtonDown("Throw");
        if (tDown&& readyToThrow&&id_>=50) //���ǰ͸� ������
        {
            ThrowReady();
        }
    }
    private void ThrowReady()
    {
        //animator.SetTrigger("isThrow");
        animatorFirst.SetTrigger("isThrow");
        animatorFirst.SetBool("isHold", false);
        Invoke(nameof(Throw), throwInvoke);
    }

    private void Throw()
    {
        if (id_ >= 50)  //����� ������
        {
            objectToEquipF[id_ - 32].SetActive(false);
        }
        readyToThrow = false;

        GameObject projectile = null;

        if (id_ >= 50)  //����� ������
        {
            projectile = Instantiate(objectToThrow[id_-32], attackPoint.position, cam.rotation);
        }

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // add force
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

    }*/


    public void Equips(int index)
    {

        if (readyToThrow)
        {
            return;
        }
        else
        {
            animatorFirst.SetBool("isHold",true);
            id_ = index;

            if (id_ >= 50)  //����� ������
            {
                objectToEquipF[id_-50].SetActive(true);
            } 
            Invoke(nameof(RTT), 0.05f);
            //readyToThrow = true;  //Ƣ��⿡ �ִ°� ���ڸ��� �ٽ� ���� �ؿ� �����Լ��� �ű�
            onPlace.Invoke(id_);
        }       
    }
    public void EquipsHoldItem()
    {
         animatorFirst.SetBool("isHold", true);
         Invoke(nameof(RTT), 0.05f);
    }
    private void RTT()
    {
        readyToThrow = true;
    }

    public void Ground()  //���� ������ �� ���� �Լ�
    {
        animatorFirst.SetBool("isHold",false);
        //objectToEquipF[id_].SetActive(false);
        if (id_ >= 50)  //����� ������
        {
            objectToEquipF[id_ - 50].SetActive(false);
        }
        Invoke(nameof(readyToTh), 0.05f);

    }
    public void PlateGround()  //���� ���������� �� ���� �Լ�
    {
        animatorFirst.SetBool("isHold", false);
        Invoke(nameof(readyToTh), 0.05f);
    }
    private void readyToTh()
    {
        readyToThrow = false;
    }
    public void WashDishOn() //�������Ҷ� ȣ��Ǵ� �Լ�
    {
        animatorFirst.SetBool("isWashDish", true);
        washDishes[0].SetActive(true); //���� 
        washDishes[1].SetActive(true); //������
    }
    public void WashDishOff() //������ �׸��Ҷ� ȣ��Ǵ� �Լ�
    {
        //Debug.Log("fdadsfa");
        animatorFirst.SetBool("isWashDish", false);
        washDishes[0].SetActive(false);
        washDishes[1].SetActive(false);
    }
    public void WashOne()  //������ �ϳ� �Ϸ��� �� TrayWash���� �̺�Ʈ ȣ���ϴ� �Լ�
    {
        animatorFirst.SetTrigger("washOne");
    }
}