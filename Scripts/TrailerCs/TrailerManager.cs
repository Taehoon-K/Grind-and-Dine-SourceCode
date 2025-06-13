using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerManager : MonoBehaviour
{
    [SerializeField] private AudioSource ambience;
    [SerializeField] private float audioFadeSec = 2f;

    public Transform cameraTransform; // 움직일 카메라
    public Transform backCharacter; // 이동하고 싶은 첫 번째 Transform
    public Transform sky; // 이동하고 싶은 두 번째 Transform
    public float moveDuration = 3f;   // 이동 및 회전 시간

    [SerializeField] Animator animator;

    [Header("WaitSec")]
    [SerializeField] private float AmbiMute;
    [SerializeField] private float backCha;
    [SerializeField] private float skyss;
    [SerializeField] private float stopCha;


    public Transform target; // 바라볼 오브젝트
    public float duration = 5f;
    public float heightOffset = 20f;
    public float distanceOffset = 20f;


    private void Start()
    {
        //StartCoroutine(FadeOutAudio(ambience, audioFadeSec)); //오디오 정지
        StartCoroutine(MoveCamera(cameraTransform, backCharacter, moveDuration, backCha)); //캐릭터 뒤로 줌아웃
       // StartCoroutine(MoveCamera(cameraTransform, sky, moveDuration, skyss)); //하늘 위로 틸트업
        StartCoroutine(CharcaterStop()); //캐ㅣㄱ터 정지
    }

    // 페이드 아웃 실행 함수


    // 실제 볼륨 줄이기 코루틴
    IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        yield return new WaitForSeconds(AmbiMute);

        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0f;  // 최종 볼륨을 확실히 0으로 설정
        audioSource.Stop();       // 오디오 정지 (필요 시)
    }




    IEnumerator MoveCamera(Transform cam, Transform target, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 startPos = cam.localPosition;
        Quaternion startRot = cam.localRotation;

        Vector3 endPos = target.localPosition;
        Quaternion endRot = target.localRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cam.localPosition = Vector3.Lerp(startPos, endPos, t);
            cam.localRotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = endPos; // 최종 위치 보정
        cam.localRotation = endRot; // 최종 로테이션 보정
    }

    IEnumerator CharcaterStop()
    {
        yield return new WaitForSeconds(stopCha);
        //animator.SetTrigger("Idle");
        cameraTransform.SetParent(null);
        StartCoroutine(MoveCamera());
    }

    IEnumerator MoveCamera()
    {
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        // 목표 위치: 대상에서 멀어지고 위로 올라간 위치
        Vector3 endPos = target.position
                         + Vector3.up * heightOffset
                         - target.forward * distanceOffset;

        // 목표 회전: 하늘을 보는 회전
        Quaternion endRot = target.rotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // 위치 점점 멀어지고 올라감
            cameraTransform.position = Vector3.Lerp(startPos, endPos, t);

            // 처음엔 타겟을 바라보다가, 점점 하늘 방향으로 회전
            Quaternion lookAtTarget = Quaternion.LookRotation(target.position - cameraTransform.position);
            cameraTransform.rotation = Quaternion.Slerp(lookAtTarget, endRot, t);
            //cameraTransform.rotation = lookAtTarget;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막 위치와 회전 고정
        cameraTransform.position = endPos;
       cameraTransform.rotation = endRot;
    }
}
