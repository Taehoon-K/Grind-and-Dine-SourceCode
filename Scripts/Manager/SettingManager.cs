using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance = null;

    private int money = 0; //전체 돈
    private int difficulty = 0; //하, 중, 상, 최상 순

    private void Awake()
    {
        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
            DontDestroyOnLoad(gameObject); //OnLoad(씬이 로드 되었을때) 자신을 파괴하지 않고 유지
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }
    }
    public int Money
    {
        get
        {
            return money; // 속성 값을 반환
        }
        set
        {
            money = value;
        }
    }
    public int Difficulty
    {
        get
        {
            return difficulty; // 속성 값을 반환
        }
        set
        {
            difficulty = value;
        }
    }

}
