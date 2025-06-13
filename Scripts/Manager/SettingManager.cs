using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance = null;

    private int money = 0; //��ü ��
    private int difficulty = 0; //��, ��, ��, �ֻ� ��

    private void Awake()
    {
        if (instance == null) //instance�� null. ��, �ý��ۻ� �����ϰ� ���� ������
        {
            instance = this; //���ڽ��� instance�� �־��ݴϴ�.
            DontDestroyOnLoad(gameObject); //OnLoad(���� �ε� �Ǿ�����) �ڽ��� �ı����� �ʰ� ����
        }
        else
        {
            if (instance != this) //instance�� ���� �ƴ϶�� �̹� instance�� �ϳ� �����ϰ� �ִٴ� �ǹ�
                Destroy(this.gameObject); //�� �̻� �����ϸ� �ȵǴ� ��ü�̴� ��� AWake�� �ڽ��� ����
        }
    }
    public int Money
    {
        get
        {
            return money; // �Ӽ� ���� ��ȯ
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
            return difficulty; // �Ӽ� ���� ��ȯ
        }
        set
        {
            difficulty = value;
        }
    }

}
