using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBlender : MonoBehaviour
{
    /// <summary>
    /// ���� Ŭ������ �ٸ� Ŭ������ ����
    /// </summary>
    /// <param name="animator">�ִϸ��̼� ��Ʈ�ѷ�</param>
    /// <param name="parameterName">���� Ʈ������ ����ϴ� �Ķ���� �̸�</param>
    /// <param name="toAnimState">������ �ִϸ��̼� ������Ʈ</param>
    /// <param name="duration">���ʵ��� ���带 ���� ����, -1�ΰ�쿡�� ���������ʰ� ��� toAnimState�� ����</param>
    public void BlendLerp(Animator animator, string parameterName, float toAnimState, float duration)
    {
        if (duration == -1)
        {
            animator.SetFloat(parameterName, toAnimState);
            return;
        }

        StartCoroutine(SetState(animator, parameterName, toAnimState, duration));
    }

    /// <summary>
    /// �ڷ�ƾ�� ����Ͽ� ����
    /// </summary>
    private IEnumerator SetState(Animator animator, string parameterName, float toAnimState, float duration)
    {
        float process = 0;
        float currentState = animator.GetFloat(parameterName);

        while (true)
        {
            animator.SetFloat(parameterName, Mathf.Lerp(currentState, toAnimState, process));

            process += Time.deltaTime / duration;

            if (process > 1.0f)
            {
                animator.SetFloat(parameterName, toAnimState);
                yield break;
            }
            yield return null;
        }
    }
}
