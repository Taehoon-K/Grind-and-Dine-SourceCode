using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketNets : MonoBehaviour
{
    private AudioSource audioSource;  // ����� �ҽ� ����
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
            audioSource.volume = 1;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        
    }
}
