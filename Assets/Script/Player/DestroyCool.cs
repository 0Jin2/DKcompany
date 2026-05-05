using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCool : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("CoolAttacking") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
