using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CoffeeBowl : Bowl
{
    // TableState ��ü�� �����ϴ� ����Ʈ �ʱ�ȭ
    private List<TableState> coffeeMenu = new List<TableState>
    {                              //����� ������ ����   ����  ����   �ʽ�   ī��    ũ��  ����
        new TableState(new bool[8] { true, false, false, true, false, false, false, false}, false), //�Ƹ޸�ī��
        new TableState(new bool[8] { false, false, true, true, true, false, false, true}, true),
        new TableState(new bool[8] { false, false, true, true, false, true, false, true}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, false, true}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, true, true}, true), //ī��� ����
        new TableState(new bool[8] { false, false, true, true, true, false, false, false}, false),
        new TableState(new bool[8] { false, false, true, true, false, true, false, false}, false),
        new TableState(new bool[8] { false, true, true, true, false, false, false, false}, false),
        new TableState(new bool[8] { false, true, true, true, false, false, true, false}, false),  //ī��� ����
                                
        new TableState(new bool[8] { false, false, true, true, true, false, false, false}, true), //���� ũ�� ���ܰ��
        new TableState(new bool[8] { false, false, true, true, false, true, false, false}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, false, false}, true),
        new TableState(new bool[8] { false, true, true, true, false, false, true, false}, true), 
    };

    [SerializeField]
    private bool mix = false; // ���� ���θ� ��Ÿ��

    [SerializeField]
    private GameObject[] mixResult; //�ϼ��� �޴� ����� ������
    [SerializeField]
    private GameObject mixBad; //�������� ��������
    [SerializeField]
    private GameObject straw; //�ϼ� �� �ŶѲ��̶� ���� ��

    private bool isCombinationActive = false; // ���� ������ Ȱ�� �������� Ȯ��

    protected override void Update()
    {
        if (!isCombinationActive && !mix) //���� �ϼ����� ���°� �ƴϰ� mix�� �ƴ϶��
        {
            // �迭 ũ�Ⱑ ���ٰ� ����
            for (int i = 0; i < ingredient.Length; i++)
            {
                if (ingredient[i])
                {
                    ingredients[i].SetActive(true);
                }
                else
                {
                    ingredients[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < ingredient.Length; i++)
            {
                ingredients[i].SetActive(false);
            }
        }
        
        // ���� ���°� �޴��� ��ġ�ϴ��� Ȯ���ϰ� ��� Ȱ��ȭ
        CheckAndActivateMixResult();
    }

    private void CheckAndActivateMixResult()
    {
        for (int i = 0; i < coffeeMenu.Count; i++)
        {
            var menu = coffeeMenu[i];
            if (IsMatch(menu))
            {
                ActivateMixResult(i);
                return;
            }
        }

        if (mix) //���� ���� ���¶��
        {
            if (mixBad.activeSelf == false)
            {
                mixBad.SetActive(true);
            }
        }
        else
        {
            // ���ǿ� �´� ������ ������ mixResult ��Ȱ��ȭ
            DeactivateMixResult();
            if(mixBad.activeSelf == true)
            {
                mixBad.SetActive(false);
            }
        }
        
    }
    private bool IsMatch(TableState menu)
    {
        if (mix != menu.isCook) return false; // �ͽ� ���°� �ٸ��� ��ġ���� ����

        for (int i = 0; i < ingredient.Length; i++)
        {
            if (ingredient[i] != menu.ingre[i])
                return false; // �ϳ��� ��ġ���� ������ false
        }

        return true; // ��� ��ġ�ϸ� true
    }

    private void ActivateMixResult(int index)
    {
        isCombinationActive = true;
        for (int i = 0; i < mixResult.Length; i++)
        {
            mixResult[i].SetActive(i == index); // �ش� �޴��� �´� ����� Ȱ��ȭ
        }
    }

    private void DeactivateMixResult()
    {
        isCombinationActive = false;
        foreach (var result in mixResult)
        {
            result.SetActive(false);
        }
    }

    public void SetMix(bool value)
    {
        mix = value;
    }
    public bool GetMix()
    {
        return mix;
    }
    public override void Pickup() //������ ������ �Լ�
    {
        Destroy(gameObject);
    }
    public override void Pickup(int id) //��� ������ ȣ���
    {
        if(id == 2 && ingredient[0]) //���� ���� Ŭ����, �׸��� �� ������ ���������� ����
        {
            ingredient[0] = false;
            ingredient[1] = true;
        }
        if(id == 0 && ingredient[2]) //���� ������ �� �߰���
        {
            ingredient[1] = true;
            return;  //������ �߰��� ����
        }
        if (id < 0)
        {
            return;
        }
        else if (id <= 6 && mix) //�������°� ũ�� ����� �߰� �ȵ�
        {
            //���߿� ui ����
            return;
        }
        else if (id == 4 && (ingredient[5] || ingredient[6]))
        {
            return;
        }
        else if (id == 5 && (ingredient[4] || ingredient[6]))
        {
            return;
        }
        else if (id == 6 && (ingredient[5] || ingredient[4]))
        {
            return;
        }
        else if (id == 7 && !mix )
        {
            return;
        }
        else if (id == 9)
        {
            SetMix(true);
            SoundManager.instance.PlaySound2D("blender");
            return;
        }
        //�ش� ��� ���� �ڵ�
        ingredient[id] = true;
    }
}
