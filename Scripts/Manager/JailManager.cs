using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailManager : MonoBehaviour
{
    [SerializeField] private int prisonDay;
    private int currentDay; //현재까지 지낸 날짜
    [SerializeField] Transform police; //간수 위치
    [SerializeField] GameObject lunckBox; //도시락

    public static JailManager instance = null;
    PlayerYarn playeryarn;
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

    private void Start()
    {
        playeryarn = FindObjectOfType<PlayerYarn>();
        currentDay = 0;
    }

    public void SkipDay() //하루 지낼때마다 하루 카운트
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
