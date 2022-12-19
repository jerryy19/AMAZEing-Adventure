using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    bool frompause;
    void Start()
    {
        player = GameObject.Find("BigVegas(Clone)");
        frompause = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadTutorial() {
        SceneManager.LoadScene("Tutorial");
    }
    
    public void LoadGame() {
        SceneManager.LoadScene("Scene");
    }

    public void LoadGuideAfterMenu() {
        BigVegas player = GameObject.Find("BigVegas(Clone)").GetComponent<BigVegas>();
        player.menu.SetActive(false);
        player.guide.SetActive(true);
    }

    //exit out of pause
    public void LoadGameAfterPause() {
        BigVegas player = GameObject.Find("BigVegas(Clone)").GetComponent<BigVegas>();
        player.animation_controller.enabled = true;
        player.health_bar.SetActive(true);
        player.pause.SetActive(false);
    }
    
    // exit out of menu
    public void LoadGameAfterGuide() {
        BigVegas player = GameObject.Find("BigVegas(Clone)").GetComponent<BigVegas>();
        player.animation_controller.enabled = true;
        player.health_bar.SetActive(true);
        player.guide.SetActive(false);
    }
    //menu, new game, and pause
    public void LoadAfterSetting() {
        BigVegas player = GameObject.Find("BigVegas(Clone)").GetComponent<BigVegas>();
        if (player.frompause) {
            player.pause.SetActive(true);
        } else {
            player.menu.SetActive(true);
        }
        player.settings.SetActive(false);
    }

    public void LoadSettingAfterPause() {
        BigVegas player = GameObject.Find("BigVegas(Clone)").GetComponent<BigVegas>();
        player.frompause = true;
        player.pause.SetActive(false);
        player.settings.SetActive(true);
    }


    public void LoadSettingAfterMenu() {
        BigVegas player = GameObject.Find("BigVegas(Clone)").GetComponent<BigVegas>();
        player.frompause = false;
        player.menu.SetActive(false);
        player.settings.SetActive(true);
    }

    public void QuitGame() {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
