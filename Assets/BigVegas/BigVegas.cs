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
    public Text health;
    public GameObject health_bar;
    public float velocity;
    public float interval = 2.0f;
    public float top_speed;
    public int healthpoint;
    // Start is called before the first frame update
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        velocity = 0.0f;
        health = health_bar.transform.GetChild(3).gameObject.GetComponent<Text>();
        healthpoint = 100;
        // change top speed to change speed
        top_speed = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {   
        if (healthpoint < 0) {
            Debug.Log("game ended");
        }
        Slider slider = health_bar.GetComponent("Slider") as Slider;
        slider.value = healthpoint;
        health.text = "100 / " + healthpoint;
        bool isWalkingForwardPressed = Input.GetKey("up");
        bool isWalkingBackwardPressed = Input.GetKey("down");
        bool isLeftTurn = Input.GetKey("left");
        bool isRightTurn = Input.GetKey("right");
        //bool isRunForwardPressed = (Input.GetKey("left shift") || Input.GetKey("right shift")) && isWalkingForwardPressed;
        bool isDance = Input.GetKey("r");
        bool isSillyDance = Input.GetKey("t");
        bool isIdlePressed = !isWalkingForwardPressed && !isWalkingBackwardPressed;
        animation_controller.SetBool("IsLeftTurn", isLeftTurn);
        animation_controller.SetBool("IsRightTurn", isRightTurn);
        animation_controller.SetBool("IsWalkingForward", isWalkingForwardPressed && velocity < 3.0f);
        animation_controller.SetBool("IsRunningForward", isWalkingForwardPressed && velocity > 3.0f);
        animation_controller.SetBool("IsWalkingBackward", isWalkingBackwardPressed);
        //animation_controller.SetBool("IsRunForward", isRunForwardPressed && !isJumpPressed);
        animation_controller.SetBool("IsIdle", isIdlePressed);      
        animation_controller.SetBool("IsDance", isDance);      
        animation_controller.SetBool("IsSillyDance", isSillyDance);

        if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("WalkingForward")) {
            if (velocity >= top_speed) {
                velocity = top_speed;
            } else {
                velocity += (top_speed) / interval;
            }
        }  else if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("WalkingBackward")) {
            if (velocity * -1.0f >= top_speed) {
                velocity = -top_speed;
            } else {
                velocity -= (top_speed) / interval;
            }         
        } else if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("RunningForward")) {
            
        } else {
            velocity = 0.0f;
        }
            float turn = Input.GetAxis("Horizontal");
            transform.Rotate(0, turn * 100f * Time.deltaTime, 0);
        
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
