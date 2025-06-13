using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerManager : MonoBehaviour
{
    [SerializeField] private AudioSource ambience;
    [SerializeField] private float audioFadeSec = 2f;

    public Transform cameraTransform; // ������ ī�޶�
    public Transform backCharacter; // �̵��ϰ� ���� ù ��° Transform
    public Transform sky; // �̵��ϰ� ���� �� ��° Transform
    public float moveDuration = 3f;   // �̵� �� ȸ�� �ð�

    [SerializeField] Animator animator;

    [Header("WaitSec")]
    [SerializeField] private float AmbiMute;
    [SerializeField] private float backCha;
    [SerializeField] private float skyss;
    [SerializeField] private float stopCha;


    public Transform target; // �ٶ� ������Ʈ
    public float duration = 5f;
    public float heightOffset = 20f;
    public float distanceOffset = 20f;


    private void Start()
    {
        //StartCoroutine(FadeOutAudio(ambience, audioFadeSec)); //����� ����
        StartCoroutine(MoveCamera(cameraTransform, backCharacter, moveDuration, backCha)); //ĳ���� �ڷ� �ܾƿ�
       // StartCoroutine(MoveCamera(cameraTransform, sky, moveDuration, skyss)); //�ϴ� ���� ƿƮ��
        StartCoroutine(CharcaterStop()); //ĳ�Ӥ��� ����
    }

    // ���̵� �ƿ� ���� �Լ�


    // ���� ���� ���̱� �ڷ�ƾ
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

        audioSource.volume = 0f;  // ���� ������ Ȯ���� 0���� ����
        audioSource.Stop();       // ����� ���� (�ʿ� ��)
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

        cam.localPosition = endPos; // ���� ��ġ ����
        cam.localRotation = endRot; // ���� �����̼� ����
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

        // ��ǥ ��ġ: ��󿡼� �־����� ���� �ö� ��ġ
        Vector3 endPos = target.position
                         + Vector3.up * heightOffset
                         - target.forward * distanceOffset;

        // ��ǥ ȸ��: �ϴ��� ���� ȸ��
        Quaternion endRot = target.rotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // ��ġ ���� �־����� �ö�
            cameraTransform.position = Vector3.Lerp(startPos, endPos, t);

            // ó���� Ÿ���� �ٶ󺸴ٰ�, ���� �ϴ� �������� ȸ��
            Quaternion lookAtTarget = Quaternion.LookRotation(target.position - cameraTransform.position);
            cameraTransform.rotation = Quaternion.Slerp(lookAtTarget, endRot, t);
            //cameraTransform.rotation = lookAtTarget;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ������ ��ġ�� ȸ�� ����
        cameraTransform.position = endPos;
       cameraTransform.rotation = endRot;
    }
}
