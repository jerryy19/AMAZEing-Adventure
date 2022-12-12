using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public new Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody.AddForce(transform.up * 100f);
        StartCoroutine("Countdown");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1.2f);
        Destroy(transform.gameObject);
    }
}
