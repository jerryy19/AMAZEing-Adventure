using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BananaMan : MonoBehaviour
{
    public GameObject banana;
    public Transform hand;
    public Animator animator;
    


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        // Adding throw event to throw animation
        AddAnimationEvent("Throw", 0.816f, "ThrowBanana", 1);

    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isThrowing", true);
        }    
    }

    private void ThrowBanana()
    {
        Vector3 bananaPos = hand.position;
        Quaternion bananaRot = hand.rotation;
        Instantiate(banana, bananaPos, bananaRot);
    }
    private void AddAnimationEvent(string clipName, float time, string function, float param)
    {
        AnimationClip animationClip = null;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName)
                animationClip = clip;
        AnimationEvent animationEvent = new AnimationEvent();
        animationEvent.functionName = function;
        animationEvent.floatParameter = param;
        animationEvent.time = time;
        
        animationClip.AddEvent(animationEvent);
    }
    
}
