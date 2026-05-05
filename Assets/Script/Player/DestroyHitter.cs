using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyHitter : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("HitterAttacking") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
