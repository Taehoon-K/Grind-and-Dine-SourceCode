using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKnowAbout : MonoBehaviour
{
    const string SECRET_PREFIX = "Secret_";

    public static List<bool> secrets = new List<bool>(new bool[10]);

    public static void UnlockSecret(int index) //��� �߰�
    {
        GameBlackboard blackboard = GameTimeStateManager.instance.GetBlackboard();

        secrets[index] = true;

        blackboard.SetValue(SECRET_PREFIX + index, true);

    }

    public static bool ExistSecret(int index) //��ũ�� �˰��ִ��� �˻�
    {
        return secrets[index]; //Ʈ�� ���� ��ȯ
    }

    public static List<bool> GetSecret() //�����Ҷ� ���� �ҷ��ÿ뵵
    {
        return secrets;
    }

    //�ε��Ҷ� ���� �ε�
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
