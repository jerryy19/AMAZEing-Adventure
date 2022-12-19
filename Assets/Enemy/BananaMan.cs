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

    public float bananaSpeed;

    private GameObject player;
    private Vector3 direction_enemy_to_player;

    private bool can_see_player;
    private Vector3 originalPosition;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bananaSpeed = 350f;
        can_see_player = false;
        direction_enemy_to_player = new Vector3(0, 0, 0);
        player = GameObject.Find("BigVegas(Clone)");
        level = GameObject.Find("Level");
        animator = GetComponent<Animator>();
        // Adding throw event to throw animation
        if (!CheckHasAnimationEvent("Throw"))
            AddAnimationEvent("Throw", 0.816f, "ThrowBanana", 1);

        originalPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        StartCoroutine("ThrowingSequence");
    }
    

    // Update is called once per frame
    void Update()
    {
        Vector3 player_centroid = player.GetComponent<CapsuleCollider>().bounds.center;
        Vector3 bananaman_centroid = GetComponent<CapsuleCollider>().bounds.center;
        direction_enemy_to_player = (player_centroid - bananaman_centroid);
        direction_enemy_to_player.Normalize();
        RaycastHit hit;
        if (Physics.Raycast(bananaman_centroid, direction_enemy_to_player, out hit, 6f))
        {
            if (hit.collider.gameObject == player)
            {
                float angle_to_rotate = Mathf.Rad2Deg * Mathf.Atan2(direction_enemy_to_player.x, direction_enemy_to_player.z);
                transform.eulerAngles = new Vector3(0.0f, angle_to_rotate, 0.0f);
                can_see_player = true;
            }
            else
            {
                can_see_player = false;
                animator.SetBool("isIdle", true);
                animator.SetBool("isThrowing", false);
            }
        }
    }

    private void ThrowBanana()
    {
        Vector3 bananaPos = hand.position;
        Quaternion bananaRot = hand.rotation;
        GameObject thrownBanana = (GameObject)GameObject.Instantiate(banana, bananaPos, transform.rotation);
        Vector3 direction_banana_to_player = player.GetComponent<CapsuleCollider>().bounds.center - thrownBanana.GetComponent<CapsuleCollider>().bounds.center;
        direction_banana_to_player.Normalize();
        Vector3 direction_to_shoot = PredictShootingDirection(direction_banana_to_player,
            thrownBanana.GetComponent<CapsuleCollider>().bounds.center);
        thrownBanana.GetComponent<Rigidbody>().AddForce(
            new Vector3(direction_to_shoot.x * bananaSpeed, Math.Abs(direction_to_shoot.y), direction_to_shoot.z * bananaSpeed)
            );
        audioSource.Play();
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

    private bool CheckHasAnimationEvent(string clipName)
    {
        AnimationClip animationClip = null;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName)
                animationClip = clip;
        return animationClip.events.Length > 0;
    }

    private Vector3 PredictShootingDirection(Vector3 current, Vector3 root)
    {
        Vector3 shooting_direction = current;
        float delta_pos = Mathf.Infinity;
        float epsilon = 0.1f;
        
        Vector3 future_target_pos = player.GetComponent<CapsuleCollider>().bounds.center;
        Vector3 last_target_pos;
        while (delta_pos > epsilon)
        {
            float distance = Vector3.Distance(player.GetComponent<CapsuleCollider>().bounds.center, root);
            float look_ahead_time = distance * 2f / bananaSpeed;

            float playerSpeed = player.GetComponent<BigVegas>().velocity;
            Vector3 playerDirection = player.GetComponent<BigVegas>().movement_direction;
            Vector3 velocityVector = playerDirection * playerSpeed;

            last_target_pos = new Vector3(future_target_pos.x, future_target_pos.y, future_target_pos.z);
            future_target_pos = player.GetComponent<CapsuleCollider>().bounds.center + look_ahead_time * velocityVector;

            delta_pos = Vector3.Distance(future_target_pos, last_target_pos);

            shooting_direction = future_target_pos - root;
            shooting_direction.Normalize();
        }

        return shooting_direction;
    }
    
    IEnumerator ThrowingSequence()
    {
        while (true) {
            if (can_see_player && GameObject.Find("healthbar") != null)
            {
                animator.SetBool("isIdle", false);
                animator.SetBool("isThrowing", true);
            }

            // transform.position = originalPosition;
            yield return new WaitForSeconds(2.1833f);
        }
    } 
}
