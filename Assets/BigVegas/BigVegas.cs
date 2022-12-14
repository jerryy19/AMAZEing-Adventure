using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BigVegas : MonoBehaviour
{
    public Animator animation_controller;
    private CharacterController character_controller;
    public Vector3 movement_direction;
    public float walking_velocity;
    public Text health;
    public bool frompause = true;
    public bool losePuzzle = false;
    public float velocity;
    public float interval = 2.0f;
    public float top_speed;
    public int healthpoint;
    public GameObject health_bar;
    public GameObject guide;
    public GameObject settings;
    public GameObject pause;
    public GameObject menu;

    public Level level;

    private AudioSource audioSource;
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        animation_controller.enabled = false;
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        velocity = 0.0f;
        health_bar = GameObject.Find("healthbar");
        health = health_bar.transform.GetChild(3).gameObject.GetComponent<Text>();
        healthpoint = 100;
        // change top speed to change speed
        top_speed = 1.5f;
        menu = GameObject.Find("Menu");
        guide = GameObject.Find("Guide");
        settings = GameObject.Find("Settings");
        pause = GameObject.Find("PauseMenu");
        level = GameObject.Find("Level").GetComponent<Main>().level;
        health_bar.SetActive(false);
        settings.SetActive(false);
        pause.SetActive(false);
        guide.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {   

        if (healthpoint <= 0) {
            SceneManager.LoadScene("NewGame");
        } 
        if (Input.GetKey("3") && health_bar.activeSelf == true) {
            animation_controller.enabled = false;
            health_bar.SetActive(false);
            pause.SetActive(true);
        }
        // adjest health
        health.text =  healthpoint  + " / 100";
        Slider slider = health_bar.GetComponent("Slider") as Slider;
        slider.value = healthpoint;

        //input key
        if (health_bar.activeSelf) {
            bool isWalkingForwardPressed = Input.GetKey("up") || Input.GetKey("w");
            bool isWalkingBackwardPressed = Input.GetKey("down") || Input.GetKey("s");
            bool isLeftTurn = Input.GetKey("left");
            bool isRightTurn = Input.GetKey("right");
            bool isDance = Input.GetKey("1");
            bool isSillyDance = Input.GetKey("2");
            bool isIdlePressed = !isWalkingForwardPressed && !isWalkingBackwardPressed;

            //animations
            animation_controller.SetBool("IsLeftTurn", isLeftTurn);
            animation_controller.SetBool("IsRightTurn", isRightTurn);
            animation_controller.SetBool("IsWalkingForward", isWalkingForwardPressed && velocity < 2.0f);
            animation_controller.SetBool("IsRunningForward", isWalkingForwardPressed && velocity > 2.0f);
            animation_controller.SetBool("IsWalkingBackward", isWalkingBackwardPressed);
            animation_controller.SetBool("IsIdle", isIdlePressed);      
            animation_controller.SetBool("IsDance", isDance);      
            animation_controller.SetBool("IsSillyDance", isSillyDance);

            //speed based on animation
            if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("WalkingForward")) {
                if (velocity <= top_speed) {
                    velocity += (top_speed) / interval;
                } else {
                    velocity = top_speed;
                }
            }  else if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("WalkingBackward")) {
                if (velocity * -1.0f >= top_speed) {
                    velocity = -top_speed;
                } else {
                    velocity -= (top_speed) / interval;
                }         
            } else if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("RunningForward")) {
                velocity = top_speed;
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
                lower_character.y = -100.0f; // hack to force her down
                character_controller.Move(lower_character);
            }
            else 
            {                   
                character_controller.Move(movement_direction * velocity * Time.deltaTime);
            }
            
        } else {
            animation_controller.SetBool("IsIdle", true);      
            velocity = 0;
            animation_controller.SetBool("IsLeftTurn", false);
            animation_controller.SetBool("IsRightTurn", false);
            animation_controller.SetBool("IsWalkingForward", false);
            animation_controller.SetBool("IsRunningForward", false);
            animation_controller.SetBool("IsWalkingBackward", false);
            animation_controller.SetBool("IsDance", false);      
            animation_controller.SetBool("IsSillyDance", false);
        }
        
    }

    public void PlayWalkSound()
    {
        audioSource.Play();
    }

}
