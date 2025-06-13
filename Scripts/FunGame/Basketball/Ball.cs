using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody ballRigidbody;
    private AudioSource audioSource;  // 오디오 소스 참조

    private int bounceCount = 0; // 공의 바운스 횟수
    private bool isHitRim; //림 맞았는지 여부
    private bool isOutCourt; //코트 나갔으면 트루

    private void Start()
    {
        ballRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tray"))
        {
            // 충돌 시 상대 속도 계산
            float collisionForce = collision.relativeVelocity.magnitude;  // 속도 크기
            
            // 충돌력 범위 설정
            float minForce = 3f;  // 최소 힘
            float maxForce = 10f; // 최대 힘

            if(collisionForce > maxForce)
            {
                collisionForce = maxForce;
            }

            // 충돌력이 범위 내에 있는지 확인
            if (collisionForce >= minForce && collisionForce <= maxForce)
            {
                // 힘을 0에서 1 사이로 비율화
                float normalizedForce = Mathf.InverseLerp(minForce, maxForce, collisionForce);

                // 오디오 소스 볼륨 설정 (최대 볼륨은 1, 최소 볼륨은 0)
                audioSource.volume = normalizedForce;

                // 오디오 소스가 재생 중이지 않으면 재생 시작
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            // 코트에 튕긴 경우 바운스 횟수 증가
            bounceCount++;
        }
        else if (collision.gameObject.CompareTag("TrashCan"))
        {
            isHitRim = true;
            SoundManager.instance.PlaySound3D("Rim", transform);
        }
        else if(collision.gameObject.CompareTag("Watch")) //백보드처럼 바운드 횟수에 포함 안되는 것
        {
            SoundManager.instance.PlaySound3D("Backboard", transform);
        }
        else if (collision.gameObject.CompareTag("Bottle")) //그물망
        {
            SoundManager.instance.PlaySound3D("BasketNet", transform);
        }
        else  //코트 밖에 것들
        {
            // Debug.Log(collision.gameObject);
            // 충돌 시 상대 속도 계산
            float collisionForce = collision.relativeVelocity.magnitude;  // 속도 크기
            // 충돌력이 범위 내에 있는지 확인
            if (collisionForce >= 3f)
            {
                SoundManager.instance.PlaySound3D("BasketLand", transform);
            }
            isOutCourt = true;
            bounceCount++;
        }

        if(bounceCount == 2)
        {
            if(isHitRim && !isOutCourt)  //만약 림 맞고 코트 밖으로 안나갔다면
            {
                BasketBallManager.instance.WhereCourt(gameObject.transform.GetChild(0).transform);
                //Debug.Log("자유튜 라인에서 다ㅣ시");
                //BasketBallManager.instance.ResetCameraToFree(false, gameObject.transform.GetChild(0).transform);
                BasketBallManager.instance.ReceiceResult(false);
            }
            else
            {
                //BasketBallManager.instance.ResetCameraToFree(true, gameObject.transform.GetChild(0).transform);
                //Debug.Log("나가리리리리리리리");
                BasketBallManager.instance.ReceiceResult(true);
            }
        }
    }

    public void ResetVariable()  //변수들 초기화
    {
        bounceCount = 0;
        isHitRim = false;
        isOutCourt=false;
    }
}
