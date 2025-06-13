using System.Collections;
using System.Collections.Generic;
using PP.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

public class CollectionPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject[] panel; //������ �г�
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
    }  //���� �����ϸ� �ִϸ����� ���� �ڵ����� ����ǵ���

    private bool[] itemCode;
    private bool[] actOpen; //Ȱ�� ���ȴ��� ����
    private bool[] notInvenItemCode; //�κ� �Ȱ�ġ�� �ٷ� �Դ� ������ �迭
    private bool[] jobOpen; //�۾� ���ȴ��� ����

    [SerializeField] private Image[] itemImage;      // ������ �̹��� �迭
    [SerializeField] private Image[] notInvenItemImage;      // ������ �̹��� �迭

    [Header("JobLogPanel")]
    [SerializeField] private Image[] jobImage;

    [Header("ActLogPanel")]
    [SerializeField] private Image[] actImage;

    private void Start()
    {
       /*- itemCode = StatusManager.instance.GetStatus().itemOpen; //������ ���� ���� �޾ƿ���
        notInvenItemCode = StatusManager.instance.GetStatus().notInvenItemOpen; //������ ���� ���� �޾ƿ���
        jobOpen = StatusManager.instance.GetStatus().jobOpen; //�۾� �����͵� �޾ƿ���
        actOpen = StatusManager.instance.GetStatus().actOpen; //Ȱ�� �����͵� �޾ƿ���*/
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
            Debug.Log("StatusManager �Ǵ� Status�� null�Դϴ�!");
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
    void UpdateJobImages() //�г� ų�� �۾� ����
    {
        for (int i = 0; i < jobImage.Length; i++)
        {
            if (jobImage[i] != null && jobOpen[i])
            {
                jobImage[i].color = Color.white;
            }
        }
    }
    private void UpdateActImages() //�г� ų�� Ȱ�� ����
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
