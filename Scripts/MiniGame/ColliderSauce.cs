using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderSauce : MonoBehaviour
{
    
    public int[] requireId;
    public GameObject[] previewItem;
    //[SerializeField]
    public GameObject[] realItem;
    [SerializeField]
    private GameObject[] hotItem;

    [SerializeField]
    private GameObject[] sauceItem;

    public bool canPlaced = true;  //�̹� ���� �ö� �ִ��� Ȯ�ο���, ����Ŵ������� �����

    private int index_;

    public bool CheckActive() //����� ��� �������� ����
    {
        foreach (GameObject obj in realItem)
        {
            if (obj.activeSelf)
            {           
                return true;
            }
        }

        return false;
    }  
    IEnumerator DelayCoroution()
    {
        yield return new WaitForSeconds(0.1f);
        realItem[index_].SetActive(true);
    }
    public void StartCo(int index)
    {
        index_ = index;
        StartCoroutine(DelayCoroution()); //��Ƣ �������ڸ��� �ٷ� �ֿ����°� ������
    }
    
    
    public void SauceInter(int id) //�ҽ� ��ȣ�ۿ�� ȣ��Ǵ� �̺�Ʈ
    {
        if (CheckActive())
        {
            realItem[index_].SetActive(false);
            if (id == 0)
            {
                hotItem[index_].SetActive(true);
            }

        }
    }
}
