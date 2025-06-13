using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidControllerBbq : HumanoidController
{
    public override void Hello() //�����̺�� Ʈ������ ȣ��
    {
        animator.SetTrigger("isHello");
        ChiTableManager.instance.SetOrderUp(index, true); //�� ���̺� ���� ���� �� ����
        
    }
    public void Order()
    {
        chatBubble_main.SetActive(true); //��ǳ�� Ű��
        chatBubble_main.transform.GetChild(mMenu).gameObject.SetActive(true); //���θ޴� ��ȣ���� �ڽ� ������Ʈ Ű��
        if (subOn)
        {
            chatBubble_sub.SetActive(true);
            chatBubble_sub.transform.GetChild(sMenu).gameObject.SetActive(true); //����޴� ��ȣ���� �ڽ� ������Ʈ Ű��
        }
        if (drinkOn)
        {
            chatBubble_drink.SetActive(true);
            chatBubble_drink.transform.GetChild(dMenu).gameObject.SetActive(true); //�帵ũ�޴� ��ȣ���� �ڽ� ������Ʈ Ű��
        }
        SoundManager.instance.PlaySound3D("blabla", transform);
        //GetComponent<AudioSource>().enabled = true; //������� ����� Ű��
    }
}
