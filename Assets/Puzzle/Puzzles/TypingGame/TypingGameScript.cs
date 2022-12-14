using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // file system

public class TypingGameScript : MonoBehaviour
{
    public GameObject letterPrefab;

    List<GameObject> lettersToPress = new List<GameObject>();
    int index = 0;
    int correctLetters = 0;
    int num_words = 0;
    List<string> words = new List<string>();        // our word bank

    private Timer timer;
    float time = 10.0f;
    bool started = false;
    float elapsedTime;

    public bool done = false;
    public bool success = false;

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

        using (StreamReader sr = File.OpenText("./Assets/UI/Puzzles/wordBank.txt")) {
            string s = "";
            while ((s = sr.ReadLine()) != null) {
                words.Add(s.ToUpper());
            }
        }

        newGame();
        num_words++;
        foreach (GameObject o in lettersToPress) {
            o.SetActive(false);
        }

    }

    void newGame() {

        foreach (GameObject o in lettersToPress) {
            Destroy(o);
        }

        lettersToPress.Clear();

        float start = -200.0f;      // positioning in GUI

        // choose a random 4 letter word
        string word = words[Random.Range(0, words.Count)];

        for (int i = 0; i < 4; i++) {
            GameObject o = Instantiate(letterPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
            o.name = $"{word[i]}";
            o.transform.GetChild(0).gameObject.GetComponent<Text>().text = o.name;

            RectTransform pos = o.GetComponent<RectTransform>();
            pos.anchoredPosition = new Vector2(start + 75, 0);
            start += 75 + 10;

            lettersToPress.Add(o);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Timer animation
        if (started && elapsedTime > 0) {
            Image i = gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
            i.fillAmount = elapsedTime / time;
            elapsedTime -= Time.deltaTime;
        }
    }

    void OnGUI()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.keyCode.ToString().Length == 1) {
            if (char.IsLetter(e.keyCode.ToString()[0])) {

                if (index >= lettersToPress.Count) return;
                // Debug.Log(e.keyCode.ToString() + " " + lettersToPress[index].name);

                // if correct key, add to score, otherwise continue;
                if (e.keyCode.ToString() == lettersToPress[index].name) {
                    correctLetters++;
                    StartCoroutine(Flash(lettersToPress[index], true));
                    index++;
                } else {
                    StartCoroutine(Flash(lettersToPress[index], false));
                }

            }

            if (index == 4) {
                index = 0;
                num_words++;
                StartCoroutine(newGameAndAnimation());
            }
        }
    }

    // see the randomization animation
    IEnumerator newGameAndAnimation() {
        for (int i = 0; i < 3; i++) {
            yield return new WaitForSeconds(0.05f);
            newGame();
        }
    }

    IEnumerator Flash(GameObject o, bool correct) {
        if (o == null) yield break;

        // color to change it
        if (correct) {
            o.GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f);
        } else {
            o.GetComponent<Image>().color = new Color(1.0f, 0.0f, 0.0f);
        }
        yield return new WaitForSeconds(0.05f);
        if (o == null) yield break;

        // color to change back or disable
        if (correct) {
            o.SetActive(false);
        } else {
            Color c = new Color(1.0f, 1.0f, 1.0f);
            c.a = 0.0f;
            o.GetComponent<Image>().color = c;
        }
    }

    // 0 = close instructions panel and display Typing Game
    // 1 = close instructions panel and exit puzzle 
    public void executeGame(int i) {
        if (i == 1) {
            started = true;
            timePanel.SetActive(true);
            instructionsPanel.gameObject.SetActive(false);
            gamePanel.gameObject.SetActive(true);

            // display initial word
            foreach (GameObject o in lettersToPress) {
                o.SetActive(true);
            }

            timer.set(time, () => {
                done = true;
                // disable game and display result (success or failure)
                gamePanel.SetActive(false);
                resultsPanel.SetActive(true);
                
                GameObject o = new GameObject();
                o.name = "resultText";
                o.AddComponent<RectTransform>();
                o.GetComponent<RectTransform>().sizeDelta = new Vector2(400.0f, 100.0f);
                o.transform.SetParent(resultsPanel.transform, false);

                // text itself
                Text text = o.AddComponent<Text>();
                text.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                text.fontSize = 60;
                text.alignment = TextAnchor.MiddleCenter;
                
                // success conditions
                string successText = null;
                if ((float)correctLetters / (num_words * 4) >= 0.70f) {
                    success = true;
                    successText = "SUCCESS";
                    text.color = new Color(0.0f, 1.0f, 0.0f);
                } else {
                    successText = "FAIL";
                    text.color = new Color(1.0f, 0.0f, 0.0f);
                }
                text.GetComponent<Text>().text = successText;

                // disable after some time
                timer.set(3.0f, () => {
                    gameObject.SetActive(false);
                });

            });
        }

        if (i == 0) gameObject.SetActive(false);
    }


}
