using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCamManager : MonoBehaviour
{
    public static CutsceneCamManager Instance { get; private set; }

    private Coroutine orbitCoroutine; // 회전 코루틴 저장용
    private int originalCullingMask; // 원래의 cullingMask 저장 변수
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private int aaLayerMask; // AA 레이어 마스크

    void Start()
    {
        aaLayerMask = 1 << LayerMask.NameToLayer("Player3rd");
    }
    public void MoveCamera(Vector3 targetPosition, Quaternion targetRotation, float duration, Action onComplete)
    {
        StartCoroutine(MoveCameraCoroutine(targetPosition, targetRotation, duration, onComplete));
    }

    private IEnumerator MoveCameraCoroutine(Vector3 targetPosition, Quaternion targetRotation, float duration, Action onComplete)
    {
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            Camera.main.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = targetRotation;

        onComplete?.Invoke();
    }

    public void LotationCamera(Quaternion targetRotation, float duration, Action onComplete)
    {
        StartCoroutine(LotationCameraCoroutine(targetRotation, duration, onComplete));
    }

    private IEnumerator LotationCameraCoroutine(Quaternion targetRotation, float duration, Action onComplete)
    {
        Quaternion startRotation = Camera.main.transform.localRotation;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            
            Camera.main.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localRotation = targetRotation;

        onComplete?.Invoke();
    }


    //특정 오브젝트를 바라보며 주위를 도는 기능
    public void OrbitAroundObject(Vector3 initialPosition, Vector3 target, float speed)
    {
        if (orbitCoroutine != null)
        {
            StopCoroutine(orbitCoroutine);
        }
        orbitCoroutine = StartCoroutine(OrbitCoroutine(initialPosition, target, speed));
    }
    private IEnumerator OrbitCoroutine(Vector3 initialPosition, Vector3 target, float speed)
    {
        if (target == null) yield break;

        // 기존 cullingMask 저장 후 Default Layer만 활성화
        originalCullingMask = Camera.main.cullingMask;
        Camera.main.cullingMask = (1 << LayerMask.NameToLayer("Default")) |
    (1 << LayerMask.NameToLayer("SimulObject")) |
    (1 << LayerMask.NameToLayer("LargeMesh"));

        // 초기 위치에서 대상까지의 벡터 계산 (수평 거리)
        Vector3 offset = initialPosition - target;
        float height = initialPosition.y; // 초기 카메라 높이 유지

        // xz 평면상의 거리 계산 (y 값 제외)
        float horizontalDistance = new Vector2(offset.x, offset.z).magnitude;
        float startAngle = Mathf.Atan2(offset.z, offset.x); // 초기 각도 계산

        float angle = startAngle; // 시작 각도

        MirrorScript mirror = FindObjectOfType<MirrorScript>();
        if(mirror != null)
        {
            mirror.ReflectLayers &= ~aaLayerMask; //헬스장 거울 3인칭 없애기
        }

        while (true)
        {
            if (target == null) yield break;

            angle += speed * Time.deltaTime; // 반시계 방향 회전
            Vector3 newOffset = new Vector3(Mathf.Cos(angle) * horizontalDistance, 0, Mathf.Sin(angle) * horizontalDistance);

            // 새로운 카메라 위치 설정 (높이는 유지)
            Camera.main.transform.position = new Vector3(target.x + newOffset.x, height, target.z + newOffset.z);
            Camera.main.transform.LookAt(target); // 대상 바라보기

            yield return null;
        }
    }
    //회전 멈추기
    public void StopOrbit()
    {
        if (orbitCoroutine != null)
        {
            StopCoroutine(orbitCoroutine);
            orbitCoroutine = null;
        }
        Camera.main.cullingMask = originalCullingMask;

        MirrorScript mirror = FindObjectOfType<MirrorScript>();
        if (mirror != null)
        {
            mirror.ReflectLayers |= aaLayerMask; //헬스장 거울 3인칭 다시 생성
        }
    }

}
