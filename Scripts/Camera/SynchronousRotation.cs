using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronousRotation : MonoBehaviour
{
    [Header("동기화 할 대상의 트랜스폼")]
    [SerializeField] private Transform mSynchronizeTarget;

    [Header("동기화시 각도의 오프셋")]
    [SerializeField] private Vector3 mEulerAnglesOffset;

    [Header("로컬 회전값으로 적용하는가?")]
    [SerializeField] private bool mIsLocalRotation = false;

    [Header("동기화 할 축")]
    [SerializeField] bool mSyncX;
    [SerializeField] bool mSyncY;
    [SerializeField] bool mSyncZ;

    private void Start()
    {
        transform.localPosition = new Vector3(0, 82, 0);
        if(mSynchronizeTarget == null)
        {
            GameObject camreas =  GameObject.FindWithTag("FirstPCamera");
            if(camreas != null)
            {
                mSynchronizeTarget = camreas.transform;
            }
        }
    }

    private Vector3 GetRotation()
    {
        return new Vector3(
            mSyncX ? mSynchronizeTarget.eulerAngles.x : 0f + mEulerAnglesOffset.x,
            mSyncY ? mSynchronizeTarget.eulerAngles.y : 0f + mEulerAnglesOffset.y,
            mSyncZ ? mSynchronizeTarget.eulerAngles.z : 0f + mEulerAnglesOffset.z);
    }

    private void Update()
    {
        if (mIsLocalRotation)
            transform.localEulerAngles = GetRotation();
        else
            transform.eulerAngles = GetRotation();
    }
}
