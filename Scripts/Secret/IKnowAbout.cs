using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKnowAbout : MonoBehaviour
{
    const string SECRET_PREFIX = "Secret_";

    public static List<bool> secrets = new List<bool>(new bool[10]);

    public static void UnlockSecret(int index) //비밀 추가
    {
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();

        secrets[index] = true;

        blackboard.SetValue(SECRET_PREFIX + index, true);

    }

    public static bool ExistSecret(int index) //시크릿 알고있는지 검사
    {
        return secrets[index]; //트루 폴스 반환
    }

    public static List<bool> GetSecret() //저장할때 관계 불러올용도
    {
        return secrets;
    }

    //로드할때 관계 로드
    public static void LoadSecret(List<bool> secretToLoad)
    {
        if (secretToLoad == null)
        {
            secretToLoad = new List<bool>(new bool[10]);
            return;
        }
        secrets = secretToLoad;
    }
}
