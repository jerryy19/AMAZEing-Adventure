using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Config : MonoBehaviour
{
    // Start is called before the first frame update
    public bool fromnewgame = false;
    public GameObject player;
    public float volume = 0.0f;

    private Timer timer;
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
        timer = GetComponent<Timer>();

        //update sound configuration if change
        GameObject player = GameObject.Find("BigVegas(Clone)");
        GameObject settings = GameObject.Find("VolumeScrollbar");
        if (player != null && fromnewgame) {
            BigVegas bv = player.GetComponent<BigVegas>();
            bv.settings.GetComponentInChildren<Scrollbar>().value = volume;
            AudioListener.volume = volume;
            ExampleCoroutine(bv);
            bv.guide.SetActive(true);
        }
        if (GameObject.Find("NewGame") != null) {
            fromnewgame = true;
        } else {
            fromnewgame = false;
        }
        if (settings != null) {
            volume = settings.GetComponentInChildren<Scrollbar>().value;
            AudioListener.volume = volume;
        }


    }
    IEnumerator ExampleCoroutine(BigVegas bv)
    {

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);
        bv.menu.SetActive(false);

    }
}
