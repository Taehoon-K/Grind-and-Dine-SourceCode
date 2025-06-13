using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationEntryPoint : MonoBehaviour
{
    //[SerializeField]
    public SceneTransitionManager.Location locationToSwitch;
    private bool sceneSwitched = false;
    private void OnTriggerEnter(Collider other) //�ݶ��̴� ���� �� ȣ��
    {
        if (other.tag == "Npc")
        {
            Destroy(other.gameObject); //npc ���Խ� �ı�
        }
    }

    public void SwitchScene() //��ȭ���̳� Ŭ�� ������ ���� �� ���
    {
        if (!sceneSwitched)
        {
            SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);
            sceneSwitched = true; // �� ��ȯ�� �Ϸ�Ǿ����� ǥ��
        }
    }
}
