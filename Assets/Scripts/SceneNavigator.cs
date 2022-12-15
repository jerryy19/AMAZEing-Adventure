using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void PauseGame() {

    }

    public void Setting() {
        
    }

    public void QuitGame() {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
