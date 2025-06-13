using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBlender : MonoBehaviour
{
    /// <summary>
    /// 현재 클립에서 다른 클립으로 블렌드
    /// </summary>
    /// <param name="animator">애니메이션 컨트롤러</param>
    /// <param name="parameterName">블렌드 트리에서 사용하는 파라미터 이름</param>
    /// <param name="toAnimState">설정한 애니메이션 스테이트</param>
    /// <param name="duration">몇초동안 블렌드를 할지 설정, -1인경우에는 블렌드하지않고 즉시 toAnimState로 설정</param>
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
    /// 코루틴을 사용하여 블렌드
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
