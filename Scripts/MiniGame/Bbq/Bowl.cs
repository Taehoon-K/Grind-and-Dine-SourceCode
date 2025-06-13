using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : InteractableItem
{
    public bool[] ingredient;
    public GameObject[] ingredients; //���� ����

    public Vector3 savedPosition;
    public Quaternion savedRotation;
    public Vector3 savedScale;

    protected virtual void Update()
    {
        // �迭 ũ�Ⱑ ���ٰ� ����
        for (int i = 0; i < ingredient.Length; i++)
        {
            if (ingredient[i])
            {
                // �ش� �ε����� ���� ������Ʈ�� Ȱ��ȭ
                ingredients[i].SetActive(true);
            }
            else
            {
                // �ش� �ε����� ���� ������Ʈ�� ��Ȱ��ȭ
                ingredients[i].SetActive(false);
            }
        }
    }
    public override void Pickup() //������ ������ �Լ�
    {
        Destroy(gameObject);
        /*
        // ��� ingredient�� false���� Ȯ���ϴ� ����
        bool allFalse = true;

        // ingredient �迭�� ��ȸ�ϸ鼭 ��� ���� false���� Ȯ��
        for (int i = 0; i < ingredient.Length; i++)
        {
            if (ingredient[i])
            {
                allFalse = false;
                break; // �ϳ��� true�� ������ �� �̻� Ȯ���� �ʿ䰡 ����
            }
        }

        if (allFalse)
        {
            // ��� ���� false��� ���� ������Ʈ�� �ı�
            Destroy(gameObject);
        }
        else
        {
            // �׷��� ������ ��� ���� false�� ����
            for (int i = 0; i < ingredient.Length; i++)
            {
                ingredient[i] = false;
            }
        }*/
    }
    public virtual void Pickup(int id) //��� ������ ȣ���
    {
        if (id < 0)
        {
            return;
        }
        else if(id == 2 && (ingredient[3] || ingredient[4]))
        {
            return;
        }
        else if (id == 3 && (ingredient[2] || ingredient[4]))
        {
            return;
        }
        else if (id == 4 && (ingredient[3] || ingredient[2]))
        {
            return;
        }
        else if (ingredient[0] && id == 1)
        {
            return;
        }
        else if (ingredient[1] && id == 0)
        {
            return;
        }
        //�ش� ��� ���� �ڵ�
        ingredient[id] = true;
    }

    public void CleanIngre() //��� �� ���ִ� �Լ�
    {
        for (int i = 0; i < ingredient.Length; i++)
        {
            ingredient[i] = false;
        }
    }
}
