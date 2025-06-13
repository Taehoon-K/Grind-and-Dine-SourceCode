using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody ballRigidbody;
    private AudioSource audioSource;  // ����� �ҽ� ����

    private int bounceCount = 0; // ���� �ٿ Ƚ��
    private bool isHitRim; //�� �¾Ҵ��� ����
    private bool isOutCourt; //��Ʈ �������� Ʈ��

    private void Start()
    {
        ballRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tray"))
        {
            // �浹 �� ��� �ӵ� ���
            float collisionForce = collision.relativeVelocity.magnitude;  // �ӵ� ũ��
            
            // �浹�� ���� ����
            float minForce = 3f;  // �ּ� ��
            float maxForce = 10f; // �ִ� ��

            if(collisionForce > maxForce)
            {
                collisionForce = maxForce;
            }

            // �浹���� ���� ���� �ִ��� Ȯ��
            if (collisionForce >= minForce && collisionForce <= maxForce)
            {
                // ���� 0���� 1 ���̷� ����ȭ
                float normalizedForce = Mathf.InverseLerp(minForce, maxForce, collisionForce);

                // ����� �ҽ� ���� ���� (�ִ� ������ 1, �ּ� ������ 0)
                audioSource.volume = normalizedForce;

                // ����� �ҽ��� ��� ������ ������ ��� ����
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            // ��Ʈ�� ƨ�� ��� �ٿ Ƚ�� ����
            bounceCount++;
        }
        else if (collision.gameObject.CompareTag("TrashCan"))
        {
            isHitRim = true;
            SoundManager.instance.PlaySound3D("Rim", transform);
        }
        else if(collision.gameObject.CompareTag("Watch")) //�麸��ó�� �ٿ�� Ƚ���� ���� �ȵǴ� ��
        {
            SoundManager.instance.PlaySound3D("Backboard", transform);
        }
        else if (collision.gameObject.CompareTag("Bottle")) //�׹���
        {
            SoundManager.instance.PlaySound3D("BasketNet", transform);
        }
        else  //��Ʈ �ۿ� �͵�
        {
            // Debug.Log(collision.gameObject);
            // �浹 �� ��� �ӵ� ���
            float collisionForce = collision.relativeVelocity.magnitude;  // �ӵ� ũ��
            // �浹���� ���� ���� �ִ��� Ȯ��
            if (collisionForce >= 3f)
            {
                SoundManager.instance.PlaySound3D("BasketLand", transform);
            }
            isOutCourt = true;
            bounceCount++;
        }

        if(bounceCount == 2)
        {
            if(isHitRim && !isOutCourt)  //���� �� �°� ��Ʈ ������ �ȳ����ٸ�
            {
                BasketBallManager.instance.WhereCourt(gameObject.transform.GetChild(0).transform);
                //Debug.Log("����Ʃ ���ο��� �٤ӽ�");
                //BasketBallManager.instance.ResetCameraToFree(false, gameObject.transform.GetChild(0).transform);
                BasketBallManager.instance.ReceiceResult(false);
            }
            else
            {
                //BasketBallManager.instance.ResetCameraToFree(true, gameObject.transform.GetChild(0).transform);
                //Debug.Log("������������������");
                BasketBallManager.instance.ReceiceResult(true);
            }
        }
    }

    public void ResetVariable()  //������ �ʱ�ȭ
    {
        bounceCount = 0;
        isHitRim = false;
        isOutCourt=false;
    }
}
