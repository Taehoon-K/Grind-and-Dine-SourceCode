using System.Collections;
using System.Collections.Generic;
using PP.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

public class CollectionPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject[] panel; //소제목 패널
    private int currentPanel;
    public int CurrentPanel
    {
        get { return currentPanel; }
        set
        {
            currentPanel = value;
            for (int i = 0; i < panel.Length; i++)
            {
                panel[i].SetActive(i == currentPanel);
            }
        }
    }  //값을 변경하면 애니메이터 값도 자동으로 변경되도록

    private bool[] itemCode;
    private bool[] actOpen; //활동 열렸는지 여부
    private bool[] notInvenItemCode; //인벤 안거치고 바로 먹는 아이템 배열
    private bool[] jobOpen; //작업 열렸는지 여부

    [SerializeField] private Image[] itemImage;      // 아이템 이미지 배열
    [SerializeField] private Image[] notInvenItemImage;      // 아이템 이미지 배열

    [Header("JobLogPanel")]
    [SerializeField] private Image[] jobImage;

    [Header("ActLogPanel")]
    [SerializeField] private Image[] actImage;

    private void Start()
    {
       /*- itemCode = StatusManager.instance.GetStatus().itemOpen; //아이템 오픈 여부 받아오기
        notInvenItemCode = StatusManager.instance.GetStatus().notInvenItemOpen; //아이템 오픈 여부 받아오기
        jobOpen = StatusManager.instance.GetStatus().jobOpen; //작업 열린것들 받아오기
        actOpen = StatusManager.instance.GetStatus().actOpen; //활동 열린것들 받아오기*/
    }
    private void OnEnable()
    {
        Render();
    }

    public void Render()
    {
        var status = StatusManager.instance?.GetStatus();

        if (status == null)
        {
            Debug.Log("StatusManager 또는 Status가 null입니다!");
            return;
        }

        if (itemCode == null)
            itemCode = status.itemOpen;

        if (notInvenItemCode == null)
            notInvenItemCode = status.notInvenItemOpen;

        if (jobOpen == null)
            jobOpen = status.jobOpen;

        if (actOpen == null)
            actOpen = status.actOpen;

        UpdateItemImages();
        UpdateJobImages();
        UpdateActImages();
    }

    void UpdateItemImages()
    {
        for (int i = 0; i < itemImage.Length; i++)
        {
            if (itemImage[i] != null && itemCode[i])
            {
                itemImage[i].color = Color.white;
            }
        }
        for (int i = 0; i < notInvenItemImage.Length; i++)
        {
            if (notInvenItemImage[i] != null && notInvenItemCode[i])
            {
                notInvenItemImage[i].color = Color.white;
            }
        }
    }
    void UpdateJobImages() //패널 킬때 작업 열기
    {
        for (int i = 0; i < jobImage.Length; i++)
        {
            if (jobImage[i] != null && jobOpen[i])
            {
                jobImage[i].color = Color.white;
            }
        }
    }
    private void UpdateActImages() //패널 킬때 활동 열기
    {
        for (int i = 0; i < actImage.Length; i++)
        {
            if (actImage[i] != null && actOpen[i])
            {
                actImage[i].color = Color.white;
            }
        }
    }
}
