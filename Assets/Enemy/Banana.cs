using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody.AddForce(transform.up * 100f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
