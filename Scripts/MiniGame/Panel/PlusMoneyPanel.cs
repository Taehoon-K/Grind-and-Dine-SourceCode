using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusMoneyPanel : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DisableAfterDelay()); // 코루틴 시작
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 1초 대기
        gameObject.SetActive(false); // 오브젝트 비활성화
    }
}
