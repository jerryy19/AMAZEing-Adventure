using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public new Rigidbody rigidbody;

    private int hitCounter; 

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        hitCounter = 0;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine("Countdown");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(2.1f);
        Destroy(transform.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit");
        if (collision.gameObject.name == "BigVegas(Clone)" && hitCounter == 0)
        {
            hitCounter++;
            audioSource.Play();
            BigVegas player = GameObject.Find("BigVegas(Clone)").GetComponent<BigVegas>();
            player.healthpoint -= 5;
        }
    }
}
