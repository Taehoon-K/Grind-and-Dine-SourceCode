using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHalfEssential : MonoBehaviour
{
    public static BlockHalfEssential instance = null;

    private void Awake()
    {
        if (instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            instance = this; //���ڽ��� instance�� �־��ݴϴ�.
            DontDestroyOnLoad(gameObject); //OnLoad(���� �ε� �Ǿ�����) �ڽ��� �ı����� �ʰ� ����
        }
        else
        {
            // ������ �����ϴ� instance�� �ı�
            Destroy(instance.gameObject);
            // ���� instance�� ����
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

}
