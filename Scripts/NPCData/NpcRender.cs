using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FLook;

[RequireComponent(typeof(CharacterMovement), typeof(Animator),typeof(FLookAnimator))]
public class NpcRender : MonoBehaviour
{
    CharacterMovement movement;
    Animator animator;
    FLookAnimator lookAni;
    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<CharacterMovement>();
        lookAni = GetComponent<FLookAnimator>();
        lookAni.ObjectToFollow = GameObject.FindGameObjectWithTag("Eye").transform;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Walk", movement.IsMoving());
    }
}
