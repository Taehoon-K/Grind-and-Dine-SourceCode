using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusMoneyPanel : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DisableAfterDelay()); // �ڷ�ƾ ����
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 1�� ���
        gameObject.SetActive(false); // ������Ʈ ��Ȱ��ȭ
    }
}
