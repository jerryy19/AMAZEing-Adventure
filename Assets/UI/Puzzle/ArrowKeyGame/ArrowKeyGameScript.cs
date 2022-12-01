using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowKeyGameScript : MonoBehaviour
{
    public GameObject upArrowPrefab;
    public GameObject downArrowPrefab;
    public GameObject leftArrowPrefab;
    public GameObject rightArrowPrefab;

    List<GameObject> arrowsToPress = new List<GameObject>();
    int index = 0;
    int score = 0;

    private Timer timer;
    float time = 10.0f;
    bool started = false;
    float elapsedTime;

    GameObject timePanel;
    GameObject instructionsPanel;
    GameObject gamePanel;
    GameObject resultsPanel;

    // Start is called before the first frame update
    void Start()
    {
        timePanel = gameObject.transform.GetChild(0).gameObject;            
        instructionsPanel = gameObject.transform.GetChild(1).gameObject;
        gamePanel = gameObject.transform.GetChild(2).gameObject;
        resultsPanel = gameObject.transform.GetChild(3).gameObject;

        elapsedTime = time;
        timer = GetComponent<Timer>();

        newGame();
        foreach (GameObject o in arrowsToPress) {
            o.SetActive(false);
        }

    }

    void newGame() {
        foreach (GameObject o in arrowsToPress) {
            Destroy(o);
        }
        arrowsToPress.Clear();
        index = 0;

        float start = -200.0f;

        // 0 = up, 1 = down, 2 = left, 3 = right
        for (int i = 0; i < 4; i++) {
            int arrowDir = Random.Range(0, 3);
            GameObject o = null;

            switch (arrowDir) {
                case 0:
                    o = Instantiate(upArrowPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
                    o.name = "up";
                    break;
                case 1:
                    o = Instantiate(downArrowPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
                    o.name = "down";
                    break;
                case 2:
                    o = Instantiate(leftArrowPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
                    o.name = "left";
                    break;
                case 3:
                    o = Instantiate(rightArrowPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
                    o.name = "right";
                    break;
            }

            RectTransform pos = o.GetComponent<RectTransform>();
            pos.anchoredPosition = new Vector2(start + 75, 0);
            start += 75 + 10;

            arrowsToPress.Add(o);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool up = Input.GetKeyDown(KeyCode.UpArrow);
        bool down = Input.GetKeyDown(KeyCode.DownArrow);
        bool left = Input.GetKeyDown(KeyCode.LeftArrow);
        bool right = Input.GetKeyDown(KeyCode.RightArrow);

        // if user clicked button
        if (up || down || left || right) {
            // if correct key, add to score, otherwise continue;
            if (up && arrowsToPress[index].name == "up" || down && arrowsToPress[index].name == "down" || 
                left && arrowsToPress[index].name == "left" || right && arrowsToPress[index].name == "right") {
                score++;
                StartCoroutine(Flash(arrowsToPress[index], true));
            } else {
                StartCoroutine(Flash(arrowsToPress[index], false));
            }

            index++;
        }

        if (index == 4) {
            StartCoroutine(Delay());
        }
        
        if (started) {
            Image i = gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
            i.fillAmount = elapsedTime / time;
            elapsedTime -= Time.deltaTime;
        }
    }

    // produces a bug where after you click the arrow key 4 times,
    // it shows a randomization animation at the end. 
    // we will call this a "feature"
    IEnumerator Delay() {
        yield return new WaitForSeconds(0.1f);
        newGame();
    }

    IEnumerator Flash(GameObject o, bool correct) {
        if (o == null) yield break;

        // save the InputField.textComponent color
        if (correct) {
            o.GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f);
        } else {
            o.GetComponent<Image>().color = new Color(1.0f, 0.0f, 0.0f);
        }
        yield return new WaitForSeconds(0.1f);
        o.SetActive(false);
    }

    // 0 = close instructions panel and display Arrow Key Game
    // 1 = close instructions panel and exit puzzle 
    public void executeGame(int i) {
        if (i == 1) {
            started = true;
            timePanel.SetActive(true);
            instructionsPanel.gameObject.SetActive(false);
            gamePanel.gameObject.SetActive(true);

            foreach (GameObject o in arrowsToPress) {
                o.SetActive(true);
            }

            timer.set(time, () => {
                gamePanel.SetActive(false);
                resultsPanel.SetActive(true);
                
                GameObject o = new GameObject();
                o.name = "resultText";
                o.AddComponent<Text>().text = $"Score: {score}";
                o.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                o.transform.SetParent(resultsPanel.transform, false);
            });

        }
    }


}
