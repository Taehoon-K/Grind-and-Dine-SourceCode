using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendTreeRandomAnimation : StateMachineBehaviour
{
    /// <summary>
    /// ���忡�� ����ϴ� �Ķ������ �̸�
    /// </summary>
    [Header("Parameter name used in blend tree")] [SerializeField] private string mStateParameterName;

    /// <summary>
    /// ���� �ϴ� �ð�
    /// </summary>
    [Header("Takes to switch to another clip")] [SerializeField] private float mBlendDuration = 0.5f;

    /// <summary>
    /// �� Ŭ������ �ð�
    /// </summary>
    [Space(50)] [Header("Times in the order you put the clips in")] [SerializeField] float[] mClipLengths;

    /// <summary>
    /// �ִϸ����� ����
    /// </summary>
    private AnimatorBlender mAnimBlender;

    /// <summary>
    /// ���� 1ȸ �����ϱ����� ����
    /// </summary>
    bool mIsAlreadyExecuted = false;

    /// <summary>
    /// ���� ������ �ð�
    /// </summary>
    private float mCurrentDelay;

    /// <summary>
    /// ���� ������� Ŭ���� �ε��� ��ȣ
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
        //���� �ѹ��� �����ϵ��� �Ѵ�
        if (mIsAlreadyExecuted) { return; }

        //�ִϸ����ͺ��� ã��
        mAnimBlender = animator.GetComponent<AnimatorBlender>();

        //Ŭ�� ������
        RefreshClip();

        //���� �Ϸ�
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