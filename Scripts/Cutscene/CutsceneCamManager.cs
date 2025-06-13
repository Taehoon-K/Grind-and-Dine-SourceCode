using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCamManager : MonoBehaviour
{
    public static CutsceneCamManager Instance { get; private set; }

    private Coroutine orbitCoroutine; // ȸ�� �ڷ�ƾ �����
    private int originalCullingMask; // ������ cullingMask ���� ����
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
    private int aaLayerMask; // AA ���̾� ����ũ

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


    //Ư�� ������Ʈ�� �ٶ󺸸� ������ ���� ���
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

        // ���� cullingMask ���� �� Default Layer�� Ȱ��ȭ
        originalCullingMask = Camera.main.cullingMask;
        Camera.main.cullingMask = (1 << LayerMask.NameToLayer("Default")) |
    (1 << LayerMask.NameToLayer("SimulObject")) |
    (1 << LayerMask.NameToLayer("LargeMesh"));

        // �ʱ� ��ġ���� �������� ���� ��� (���� �Ÿ�)
        Vector3 offset = initialPosition - target;
        float height = initialPosition.y; // �ʱ� ī�޶� ���� ����

        // xz ������ �Ÿ� ��� (y �� ����)
        float horizontalDistance = new Vector2(offset.x, offset.z).magnitude;
        float startAngle = Mathf.Atan2(offset.z, offset.x); // �ʱ� ���� ���

        float angle = startAngle; // ���� ����

        MirrorScript mirror = FindObjectOfType<MirrorScript>();
        if(mirror != null)
        {
            mirror.ReflectLayers &= ~aaLayerMask; //�ｺ�� �ſ� 3��Ī ���ֱ�
        }

        while (true)
        {
            if (target == null) yield break;

            angle += speed * Time.deltaTime; // �ݽð� ���� ȸ��
            Vector3 newOffset = new Vector3(Mathf.Cos(angle) * horizontalDistance, 0, Mathf.Sin(angle) * horizontalDistance);

            // ���ο� ī�޶� ��ġ ���� (���̴� ����)
            Camera.main.transform.position = new Vector3(target.x + newOffset.x, height, target.z + newOffset.z);
            Camera.main.transform.LookAt(target); // ��� �ٶ󺸱�

            yield return null;
        }
    }
    //ȸ�� ���߱�
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
            mirror.ReflectLayers |= aaLayerMask; //�ｺ�� �ſ� 3��Ī �ٽ� ����
        }
    }

}
