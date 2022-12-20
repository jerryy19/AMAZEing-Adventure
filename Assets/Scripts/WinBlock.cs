using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WinBlock : MonoBehaviour
{
    GameObject level;
    Main m;
    // Start is called before the first frame update
    void Start()
    {
        level = GameObject.Find("Level");
        m = level.GetComponent<Main>();

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
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "BigVegas(Clone)") {

            // win condition
            if (m.solvedPuzzles == m.level.num_puzzles) {
                StartCoroutine(AfterWinning());
            } else {
                StartCoroutine(displayPuzzlesLeft());
            }

        }

    
    }

    IEnumerator displayPuzzlesLeft() {
        GameObject o = new GameObject();
        o.transform.SetParent(GameObject.Find("Canvas").gameObject.transform, false);
        o.AddComponent<RectTransform>();
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(400.0f, 100.0f);
        o.AddComponent<Text>();
        o.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        o.GetComponent<Text>().fontSize = 60;
        o.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        o.GetComponent<Text>().text = $"Puzzles Left: {m.level.num_puzzles - m.solvedPuzzles}";

        yield return new WaitForSeconds(3.0f);
        
        Destroy(o);
    }

    IEnumerator AfterWinning() {

        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("StartGame");
    }
}
