using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour
{
    GameObject level;

    // Start is called before the first frame update
    void Start()
    {
        level = GameObject.Find("Level");

        Bounds bounds = GetComponent<Collider>().bounds;
        bounds.size *= 0.2f;

        GetComponent<MeshCollider>().convex = true;
        GetComponent<MeshCollider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // when player enter slow field, it slows the player down for some time
        // get play from level
        if (other.gameObject.name == "BigVegas(Clone)") {
            GetComponent<AudioSource>().Play();
            other.gameObject.GetComponent<BigVegas>().top_speed = 0.9f;
            StartCoroutine(SlowDown(other));
        }

    }

    IEnumerator SlowDown(Collider other) {
        
        yield return new WaitForSeconds(3.0f);
        other.gameObject.GetComponent<BigVegas>().top_speed = 1.5f;
        Debug.Log(other.gameObject.GetComponent<BigVegas>().top_speed);
    }
}
