using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHalfEssential : MonoBehaviour
{
    public static BlockHalfEssential instance = null;

    private void Awake()
    {
        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
            DontDestroyOnLoad(gameObject); //OnLoad(씬이 로드 되었을때) 자신을 파괴하지 않고 유지
        }
        else
        {
            // 기존에 존재하는 instance를 파괴
            Destroy(instance.gameObject);
            // 현재 instance로 설정
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

}
