using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraManager : MonoBehaviour, ITimeTracker
{
    [SerializeField] private float checkDistance = 70f; // 플레이어와의 거리 기준
    [SerializeField]
    private GameObject[] extraNpc;
    [SerializeField] private int[] startTime;
    [SerializeField] private int[] endTime;
    //[SerializeField] private bool[] isReverse; //만약 시간이 사이가 아니라면, 즉 0부터 a까지, b부터 24까지라면 트루

    [Header("Envrions")]
    [SerializeField] private AudioClip[] buskingAudio; //버스킹 시 랜덤 재생할 오디오

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
        if (instance != null && instance != this) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
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
        if (GameTimeStateManager.instance.isDayChange) //만약 새로운 날이면
        {
            GameTimeStateManager.instance.isDayChange = false;
            //하루 지나 리셋할 것들

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

            //버스킹 랜덤 오디오 코드
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
            Debug.Log("Time매니저 못찾음");
        }
    }
    public void ClockUpdate(GameTimestamp timestamp) //시간 업뎃마다 호출될함수
    {
        
        if (player == null) return;

        for (int i = 0; i < extraNpc.Length; i++)
        {
            if (extraNpc[i] == null) continue;

            float distance = Vector3.Distance(player.position, extraNpc[i].transform.position);

            // 플레이어가 멀리 있을 때만 활성화/비활성화 처리
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
    public void GenPolice() //경찰 생성
    {
        police.transform.position = policeGenPoint;
        police.transform.rotation = policeGenRotation;
        police.SetActive(true);
        playeryarn.CheckForNearbyNPC("CriminalStart", police.transform);
    }
    public void GenSean() //시언 생성
    {
        sean.SetActive(true);
        playeryarn.CheckForNearbyNPC("SeanLook", sean.transform);
    }

    public void DegenPolice() //경찰 다시 없애기
    {
        //police2.SetActive(false);
        //police2 = null;
    }

    public void MovePlayer(int currentIndex) //킥보드 클릭시 해당 위치로 이동
    {
        index = currentIndex;
        UIManager.instance.StartFadeoutScooter(MoveStart);

    }
    private void MoveStart()
    {

        Transform player = FindObjectOfType<Kupa.Player>().transform;
        //캐릭터 컨트롤러 비활성
        CharacterController playerCharacter = player.GetComponent<CharacterController>();
        playerCharacter.enabled = false;

        player.position = kickboardTransform[index].position;
        player.rotation = kickboardTransform[index].rotation;
        //pivotCameraPoint.rotation = startPoint.rotation;
        player.GetComponent<Kupa.Player>().ResetRotation(kickboardTransform[index]);

        //다시 활성화
        playerCharacter.enabled = true;

        UIManager.instance.StopFadeoutScooter(); //위치 이동 후 페이드인
    }
}
