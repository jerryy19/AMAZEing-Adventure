using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BigVegas : MonoBehaviour
{
    private Animator animation_controller;
    private CharacterController character_controller;
    public Vector3 movement_direction;
    public float walking_velocity;
    // public Text text;    
    // public Text text2;

    public float velocity;
    public int num_lives;
    public bool has_won;
    public float deathtime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        walking_velocity = 1.5f;
        velocity = 0.0f;
        num_lives = 5;
        has_won = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalkingForwardPressed = Input.GetKey("up");
        bool isWalkingBackwardPressed = Input.GetKey("down");
        bool isRunForwardPressed = (Input.GetKey("left shift") || Input.GetKey("right shift")) && isWalkingForwardPressed;
        bool isJumpPressed = Input.GetKey("space");
        bool isIdlePressed = !isWalkingForwardPressed && !isWalkingBackwardPressed && !isRunForwardPressed && !isJumpPressed;
        bool noWalkForwardInterference = !(isRunForwardPressed);
        float interval = 5.0f;

        if (has_won) {
            animation_controller.Play("Idle", -1, 0);
             
        } else {
            animation_controller.SetBool("IsStartWalkingForward", isWalkingForwardPressed && velocity < walking_velocity);
            Debug.Log(isWalkingForwardPressed + " " + (velocity));
            animation_controller.SetBool("IsWalkingForward", isWalkingForwardPressed && velocity == walking_velocity);
            //animation_controller.SetBool("IsWalkingBackward", isWalkingBackwardPressed);
            //animation_controller.SetBool("IsRunForward", isRunForwardPressed && !isJumpPressed);
            animation_controller.SetBool("IsIdle", isIdlePressed);            
            //animation_controller.SetBool("IsJump", isJumpPressed && !animation_controller.GetCurrentAnimatorStateInfo(0).IsName("Jump"));
        }
        

        if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("RunningForward")) {
            if (velocity < walking_velocity * 2.0f) {
                velocity += (walking_velocity * 2.0f) / interval;
            } else {
                velocity = walking_velocity * 2.0f;
            }
        } else if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("StartWalkingForward")) {
            Debug.Log("yes");
            velocity += walking_velocity / interval;
        } else if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("WalkingForward")) { 
            velocity = walking_velocity;
        } else if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("WalkingBackward")) {
            
            if (velocity > -walking_velocity/1.5f) {
                velocity -= (walking_velocity/1.5f) / interval;
            } else {
                velocity = -walking_velocity/1.5f;
            }
        } 
        // else if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("Jump")) {
        //     if (velocity < walking_velocity * 3.0f) {
        //         velocity += (walking_velocity * 3.0f) / interval;
        //     } else {
        //         velocity = walking_velocity * 3.0f;
        //     }
        // } 
        else {
            velocity = 0.0f;
        }
        if (has_won || num_lives == 0) {
            velocity = 0.0f;
        }
        float xdirection = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        float zdirection = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        movement_direction = new Vector3(xdirection, 0.0f, zdirection);

        if (transform.position.y > 0.0f) // if the character starts "climbing" the terrain, drop her down
        {
            Vector3 lower_character = movement_direction * velocity * Time.deltaTime;
            lower_character.y = -100f; // hack to force her down
            character_controller.Move(lower_character);
        }
        else
        {
            character_controller.Move(movement_direction * velocity * Time.deltaTime);
        }
    }
}
