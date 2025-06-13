using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraHello : MonoBehaviour
{
    Animator animator;
    [SerializeField] private float second;

    void Start()
    {
        animator = GetComponent<Animator>();
        Invoke(nameof(Hello),second);
    }

    private void Hello()
    {
        animator.SetTrigger("Idle");
    }
}
