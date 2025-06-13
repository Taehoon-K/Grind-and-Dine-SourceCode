using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailManager : MonoBehaviour
{
    [SerializeField] private int prisonDay;
    private int currentDay; //������� ���� ��¥
    [SerializeField] Transform police; //���� ��ġ
    [SerializeField] GameObject lunckBox; //���ö�

    public static JailManager instance = null;
    PlayerYarn playeryarn;
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

    private void Start()
    {
        playeryarn = FindObjectOfType<PlayerYarn>();
        currentDay = 0;
    }

    public void SkipDay() //�Ϸ� ���������� �Ϸ� ī��Ʈ
    {
        currentDay++;
        if (currentDay >= prisonDay) 
        {
            playeryarn.CheckForNearbyNPC("JailLeft", police);
        }

        if (lunckBox.activeSelf == false)
        {
            lunckBox.SetActive(true);
        }
    }

}
