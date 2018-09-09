using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BeatAnimator : MonoBehaviour
{
    Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;
    }
    
    void Update()
    {
        animator.speed = 1;
        animator.Update(BeatTime.deltaBeat);
        animator.speed = 0;
    }

}
