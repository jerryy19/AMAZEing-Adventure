using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    // Start is called before the first frame update
    public bool fromnewgame = false;
    // add volume configuration
    void Awake()
    {

        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //update sound configuration if change
        GameObject player = GameObject.Find("BigVegas(Clone)");
        if (player != null && fromnewgame) {
            Debug.Log("from new game");
            BigVegas bv = player.GetComponent<BigVegas>();
            bv.menu.SetActive(false);
            bv.guide.SetActive(true);
        }
        if (GameObject.Find("NewGame") != null) {
            fromnewgame = true;
        } else {
            fromnewgame = false;
        }
    }
}
