using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using UnityEngine;

public class CarCanvas : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField]
    private Material[] materials; //������ ĥ�� ����
    [SerializeField] private GameObject[] door; //�� ���� �޹� ĥ�ϴ� �뵵

    public void Render(int oil, int item)
    {
        items[oil-1].SetActive(true);

        if(item != 0)
        {
            items[item+1].SetActive(true);
        }

        int randomMat = Random.Range(0, materials.Length); //���� ĥ�ϱ�
        GetComponent<MeshRenderer>().material = materials[randomMat];
        if (door != null)
        {
            foreach (GameObject obj in door)
            {
                obj.GetComponent<MeshRenderer>().material = materials[randomMat];
            }
        }
    }

    public void CorrectItem(bool isOil)
    {
        if (isOil) 
        { 
            items[0].transform.GetChild(0).gameObject.SetActive(true);
            items[1].transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            items[2].transform.GetChild(0).gameObject.SetActive(true);
            items[3].transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void StartSound()
    {
        GetComponent<AudioSource>().Play();
    }
}
