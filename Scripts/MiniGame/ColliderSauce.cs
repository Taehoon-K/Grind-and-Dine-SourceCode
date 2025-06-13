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

    public bool canPlaced = true;  //이미 무언가 올라가 있는지 확인여부, 빌드매니저에서 사용중

    private int index_;

    public bool CheckActive() //양념통 사용 가능한지 여부
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
        StartCoroutine(DelayCoroution()); //감튀 내려놓자마자 바로 주워지는거 방지용
    }
    
    
    public void SauceInter(int id) //소스 상호작용시 호출되는 이벤트
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
