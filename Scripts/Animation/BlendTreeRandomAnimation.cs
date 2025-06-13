using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendTreeRandomAnimation : StateMachineBehaviour
{
    /// <summary>
    /// 블렌드에서 사용하는 파라미터의 이름
    /// </summary>
    [Header("Parameter name used in blend tree")] [SerializeField] private string mStateParameterName;

    /// <summary>
    /// 블렌드 하는 시간
    /// </summary>
    [Header("Takes to switch to another clip")] [SerializeField] private float mBlendDuration = 0.5f;

    /// <summary>
    /// 각 클립들의 시간
    /// </summary>
    [Space(50)] [Header("Times in the order you put the clips in")] [SerializeField] float[] mClipLengths;

    /// <summary>
    /// 애니메이터 블렌더
    /// </summary>
    private AnimatorBlender mAnimBlender;

    /// <summary>
    /// 최초 1회 실행하기위해 구분
    /// </summary>
    bool mIsAlreadyExecuted = false;

    /// <summary>
    /// 현재 딜레이 시간
    /// </summary>
    private float mCurrentDelay;

    /// <summary>
    /// 현재 재생중인 클립의 인덱스 번호
    /// </summary>
    private int mCurrentClipIndex;

    private void RefreshClip()
    {
        mCurrentClipIndex = Random.Range(0, mClipLengths.Length);
        mCurrentDelay = mClipLengths[mCurrentClipIndex];
    }

    private void PlayUpdatedClip(Animator animator)
    {
        RefreshClip();

        mAnimBlender.BlendLerp(animator, mStateParameterName, mCurrentClipIndex, mBlendDuration);
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //최초 한번만 실행하도록 한다
        if (mIsAlreadyExecuted) { return; }

        //애니메이터블렌더 찾기
        mAnimBlender = animator.GetComponent<AnimatorBlender>();

        //클립 재조정
        RefreshClip();

        //실행 완료
        mIsAlreadyExecuted = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mCurrentDelay -= Time.deltaTime;
        if (mCurrentDelay < 0f) { PlayUpdatedClip(animator); }
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}