using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraManager : MonoBehaviour, ITimeTracker
{
    [SerializeField] private float checkDistance = 70f; // �÷��̾���� �Ÿ� ����
    [SerializeField]
    private GameObject[] extraNpc;
    [SerializeField] private int[] startTime;
    [SerializeField] private int[] endTime;
    //[SerializeField] private bool[] isReverse; //���� �ð��� ���̰� �ƴ϶��, �� 0���� a����, b���� 24������� Ʈ��

    [Header("Envrions")]
    [SerializeField] private AudioClip[] buskingAudio; //����ŷ �� ���� ����� �����

    [Header("Police")]
    [SerializeField] private GameObject police;
    [SerializeField] private Vector3 policeGenPoint;
    [SerializeField] private Quaternion policeGenRotation;

    [Header("Sean")]
    [SerializeField] private GameObject sean;
   /* [SerializeField] private Vector3 seanGenPoint;
    [SerializeField] private Quaternion seanGenRotation;*/

    public static ExtraManager instance = null;
    PlayerYarn playeryarn;
    Transform player;

    [Header("Kickboard")]
    [SerializeField] private Transform[] kickboardTransform;

    private int index;
    [Header("InterObjects")]
    [SerializeField] public GameObject[] interObjects;

    private void Awake()
    {
        if (instance != null && instance != this) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        if (GameTimeStateManager.instance.isDayChange) //���� ���ο� ���̸�
        {
            GameTimeStateManager.instance.isDayChange = false;
            //�Ϸ� ���� ������ �͵�

            for(int i = 0; i < interObjects.Length; i++)
            {
                interObjects[i].SetActive(true);
            }
        }
        else
        {

        }

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        foreach (GameObject npc in extraNpc)
        { 
            npc.SetActive(false);
        }
        GenExtra();

        playeryarn = FindObjectOfType<PlayerYarn>();
    }

    private void GenExtra()
    {
        if(TimeManager.instance != null)
        {
            GameTimestamp currentTime = TimeManager.instance.GetGameTimestamp();
            for(int i = 0; i < extraNpc.Length; i++) 
            {

                bool shouldBeActive = currentTime.hour >= startTime[i] && currentTime.hour < endTime[i];
                if (extraNpc[i].activeSelf != shouldBeActive)
                {
                    extraNpc[i].SetActive(shouldBeActive);
                }

            }

            //����ŷ ���� ����� �ڵ�
            if(currentTime.hour <= 10)
            {
                int rand = UnityEngine.Random.Range(0, 2);
                extraNpc[2].GetComponent<AudioSource>().clip = buskingAudio[rand];
                extraNpc[2].GetComponent<AudioSource>().Play();
            }
            else if(currentTime.hour <= 15)
            {
                int rand = UnityEngine.Random.Range(0, 2) + 2;
                extraNpc[2].GetComponent<AudioSource>().clip = buskingAudio[rand];
                extraNpc[2].GetComponent<AudioSource>().Play();
            }
            else if(currentTime.hour <= 20)
            {
                int rand = UnityEngine.Random.Range(0, 2) + 4;
                extraNpc[2].GetComponent<AudioSource>().clip = buskingAudio[rand];
                extraNpc[2].GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            Debug.Log("Time�Ŵ��� ��ã��");
        }
    }
    public void ClockUpdate(GameTimestamp timestamp) //�ð� �������� ȣ����Լ�
    {
        
        if (player == null) return;

        for (int i = 0; i < extraNpc.Length; i++)
        {
            if (extraNpc[i] == null) continue;

            float distance = Vector3.Distance(player.position, extraNpc[i].transform.position);

            // �÷��̾ �ָ� ���� ���� Ȱ��ȭ/��Ȱ��ȭ ó��
            if (distance >= checkDistance)
            {
                bool shouldBeActive = timestamp.hour >= startTime[i] && timestamp.hour < endTime[i];
                if (extraNpc[i].activeSelf != shouldBeActive)
                {
                    extraNpc[i].SetActive(shouldBeActive);
                }
            }
        }
    }
    public void GenPolice() //���� ����
    {
        police.transform.position = policeGenPoint;
        police.transform.rotation = policeGenRotation;
        police.SetActive(true);
        playeryarn.CheckForNearbyNPC("CriminalStart", police.transform);
    }
    public void GenSean() //�þ� ����
    {
        sean.SetActive(true);
        playeryarn.CheckForNearbyNPC("SeanLook", sean.transform);
    }

    public void DegenPolice() //���� �ٽ� ���ֱ�
    {
        //police2.SetActive(false);
        //police2 = null;
    }

    public void MovePlayer(int currentIndex) //ű���� Ŭ���� �ش� ��ġ�� �̵�
    {
        index = currentIndex;
        UIManager.instance.StartFadeoutScooter(MoveStart);

    }
    private void MoveStart()
    {

        Transform player = FindObjectOfType<Kupa.Player>().transform;
        //ĳ���� ��Ʈ�ѷ� ��Ȱ��
        CharacterController playerCharacter = player.GetComponent<CharacterController>();
        playerCharacter.enabled = false;

        player.position = kickboardTransform[index].position;
        player.rotation = kickboardTransform[index].rotation;
        //pivotCameraPoint.rotation = startPoint.rotation;
        player.GetComponent<Kupa.Player>().ResetRotation(kickboardTransform[index]);

        //�ٽ� Ȱ��ȭ
        playerCharacter.enabled = true;

        UIManager.instance.StopFadeoutScooter(); //��ġ �̵� �� ���̵���
    }
}
