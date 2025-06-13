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

    public UnityEvent<int> onPlace; //아이템 배치 이벤트
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
        if (tDown&& readyToThrow&&id_>=50) //마실것만 던지게
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
        if (id_ >= 50)  //음료수 아이템
        {
            objectToEquipF[id_ - 32].SetActive(false);
        }
        readyToThrow = false;

        GameObject projectile = null;

        if (id_ >= 50)  //음료수 아이템
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

            if (id_ >= 50)  //음료수 아이템
            {
                objectToEquipF[id_-50].SetActive(true);
            } 
            Invoke(nameof(RTT), 0.05f);
            //readyToThrow = true;  //튀김기에 있는거 줍자마자 다시 놔서 밑에 지연함수로 옮김
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

    public void Ground()  //땅에 놓았을 때 실행 함수
    {
        animatorFirst.SetBool("isHold",false);
        //objectToEquipF[id_].SetActive(false);
        if (id_ >= 50)  //음료수 아이템
        {
            objectToEquipF[id_ - 50].SetActive(false);
        }
        Invoke(nameof(readyToTh), 0.05f);

    }
    public void PlateGround()  //접시 내려놓았을 때 실행 함수
    {
        animatorFirst.SetBool("isHold", false);
        Invoke(nameof(readyToTh), 0.05f);
    }
    private void readyToTh()
    {
        readyToThrow = false;
    }
    public void WashDishOn() //설거지할때 호출되는 함수
    {
        animatorFirst.SetBool("isWashDish", true);
        washDishes[0].SetActive(true); //접시 
        washDishes[1].SetActive(true); //수세미
    }
    public void WashDishOff() //설거지 그만할때 호출되는 함수
    {
        //Debug.Log("fdadsfa");
        animatorFirst.SetBool("isWashDish", false);
        washDishes[0].SetActive(false);
        washDishes[1].SetActive(false);
    }
    public void WashOne()  //설거지 하나 완료할 때 TrayWash에서 이벤트 호출하는 함수
    {
        animatorFirst.SetTrigger("washOne");
    }
}