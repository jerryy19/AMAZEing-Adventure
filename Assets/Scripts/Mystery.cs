using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mystery : MonoBehaviour
{
    GameObject level;
    Collider c;

    // Start is called before the first frame update
    void Start()
    {
        level = GameObject.Find("Level");
        c = gameObject.AddComponent<Collider>();
        c.isTrigger = true;

        Bounds bounds = GetComponent<Collider>().bounds;
        bounds.size *= 0.2f;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // when player enter mystery field, it shows where enemies are
        // get player from the level
    }
}
