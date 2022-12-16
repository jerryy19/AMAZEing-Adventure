using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BananaMan : MonoBehaviour
{
    public GameObject banana;
    public GameObject level;
    public Transform hand;
    public Animator animator;

    private GameObject player;
    private Vector3 direction_to_player;
    
    // Start is called before the first frame update
    void Start()
    {
        direction_to_player = new Vector3(0, 0, 0);
        player = GameObject.Find("BigVegas(Clone)");
        level = GameObject.Find("Level");
        animator = GetComponent<Animator>();
        // Adding throw event to throw animation
        AddAnimationEvent("Throw", 0.816f, "ThrowBanana", 1);
    }
    

    // Update is called once per frame
    void Update()
    {
        Vector3 player_centroid = player.GetComponent<CapsuleCollider>().bounds.center;
        Vector3 bananaman_centroid = GetComponent<CapsuleCollider>().bounds.center;
        direction_to_player = (player_centroid - bananaman_centroid);
        direction_to_player.Normalize();
        RaycastHit hit;
        if (Physics.Raycast(bananaman_centroid, direction_to_player, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == player)
            {
                float angle_to_rotate = Mathf.Rad2Deg * Mathf.Atan2(direction_to_player.x, direction_to_player.z);
                transform.eulerAngles = new Vector3(0.0f, angle_to_rotate, 0.0f);
            }
        }
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
