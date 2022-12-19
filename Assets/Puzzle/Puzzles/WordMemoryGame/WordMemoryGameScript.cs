using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // file system

public class WordMemoryGameScript : MonoBehaviour
{
   
   public GameObject letterPrefab;

    List<string> words = new List<string>();                    // our word bank
    List<GameObject>[] wordInRow = new List<GameObject>[5];     // letter outline ui
    List<string> toRemember = new List<string>();               // words to remember for player
    List<string> toRememberTemp = new List<string>();               // words to remember for player
    int numWords;      // choose this many words to memorize

    private Timer timer;
    float time = 10.0f;
    bool started = false;
    bool guessPhase = false;
    float elapsedTime;
    int currentRow = 0;
    int currentCol = 0;
    int correct = 0;

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

        using (StreamReader sr = File.OpenText("./Assets/Puzzle/Puzzles/wordBank.txt")) {
            string s = "";
            while ((s = sr.ReadLine()) != null) {
                words.Add(s.ToUpper());
            }
        }

        numWords = Random.Range(3, 5);      // 3 to 4 words

        // initialize the list for letter outline on UI 
        for (int i = 0; i < numWords; i++) {
            wordInRow[i] = new List<GameObject>();
        }

    }


    void createGame() {

        int temp = numWords;
        while (temp != 0) {
            string word = words[Random.Range(0, words.Count - 1)];
            if (!toRemember.Contains(word)) {
                toRemember.Add(word);
                temp--;
            }
        }
        toRememberTemp.AddRange(toRemember);

        // GameObject o = Instantiate(letterPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
        GameObject toRememberWordsUI = new GameObject();
        toRememberWordsUI.name = "WORDS";
        toRememberWordsUI.transform.SetParent(gamePanel.transform, false);
        toRememberWordsUI.AddComponent<RectTransform>();
        toRememberWordsUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0.0f);
        toRememberWordsUI.GetComponent<RectTransform>().sizeDelta = new Vector2(200.0f, 150.0f);
        toRememberWordsUI.AddComponent<Text>();
        toRememberWordsUI.GetComponent<Text>().text = string.Join("\n", toRemember.ToArray());
        toRememberWordsUI.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        toRememberWordsUI.GetComponent<Text>().fontSize = 30;
        toRememberWordsUI.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;


        // player will have 10 seconds to remember the words, then have 30 seconds to type the words
        // display typing word UI after 10 seconds
        timer.set(time, () => {
            guessPhase = true;
            elapsedTime = 25.0f;
            time = 25.0f;
            toRememberWordsUI.SetActive(false);

            float s = 40.0f;          // change the size of prefab to this

            float vert = 52.0f;
            for (int row = 0; row < numWords; row++) {
                float hor = -100.0f;      // positioning in GUI
                for (int i = 0; i < 4; i++) {
                    GameObject o = Instantiate(letterPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
                    // o.transform.localScale
                    o.name = $"{(row, i)}";
                    o.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
                    
                    RectTransform pos = o.GetComponent<RectTransform>();
                    pos.sizeDelta = new Vector2(s, s);      // change size of prefab
                    pos.anchoredPosition = new Vector2(hor + s, vert);
                    hor += s + 3;

                    wordInRow[row].Add(o);
                }
                vert -= (s + 3);
            }
        });

    }


    // Update is called once per frame
    void Update()
    {
        if (!done && (correct == numWords || (guessPhase && elapsedTime <= 0))) {
            done = true;
            // disable game and display result (success or failure)
            timePanel.SetActive(false);
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
            
            string successText = null;
            if (correct == numWords) {
                success = true;
                successText = "SUCCESS";
                text.color = new Color(0.0f, 1.0f, 0.0f);
            } else {
                successText = "FAIL";
                text.color = new Color(1.0f, 0.0f, 0.0f);

                GameObject actualWord = new GameObject();
                actualWord.name = "actualWordsText";
                actualWord.AddComponent<RectTransform>();
                actualWord.GetComponent<RectTransform>().sizeDelta = new Vector2(400.0f, 100.0f);
                actualWord.transform.SetParent(resultsPanel.transform, false);

                // text itself
                Text text2 = actualWord.AddComponent<Text>();
                text2.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                text2.fontSize = 20;
                text2.alignment = TextAnchor.LowerCenter;
                text2.GetComponent<Text>().text = $"Words: {string.Join(", ", toRemember.ToArray())}";
            }
            text.GetComponent<Text>().text = successText;

            // disable after some time
            timer.set(3.0f, () => {
                gameObject.SetActive(false);
            });

        }

        // Timer animation
        if (started && elapsedTime > 0) {
            Image i = gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
            i.fillAmount = elapsedTime / time;
            elapsedTime -= Time.deltaTime;
        }
    }

    void OnGUI()
    {
        if (!guessPhase) return;
        Event e = Event.current;

        if (gamePanel.gameObject.activeSelf) {
            if (currentRow >= numWords) return;

            if (e.type == EventType.KeyDown && e.keyCode.ToString() != "None") {
                if (e.keyCode >= KeyCode.A && e.keyCode <= KeyCode.Z) {
                    if (currentCol >= 4) return;
                    GameObject o = wordInRow[currentRow][currentCol];
                    o.name = $"{e.keyCode}";
                    o.transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{e.keyCode}";
                    currentCol++;
                    
                }

                if (e.keyCode == KeyCode.Backspace) {
                    if (currentCol == 0) return;
                    GameObject o = wordInRow[currentRow][--currentCol];
                    o.name = $"{(currentRow, currentCol)}";
                    o.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
                }

                if (e.keyCode == KeyCode.Return) {
                    
                    // check if the whole row is filled
                    foreach (GameObject o in wordInRow[currentRow]) {
                        if (o.transform.GetChild(0).gameObject.GetComponent<Text>().text == "") {
                            return;
                        }
                    }

                    string playerWord = "";
                    foreach (GameObject o in wordInRow[currentRow]) {
                        playerWord += o.transform.GetChild(0).gameObject.GetComponent<Text>().text;
                    }
                    
                    // check if player word/guess is correct
                    if (toRememberTemp.Contains(playerWord)) {
                        StartCoroutine(Flash(wordInRow[currentRow], true));
                        toRememberTemp.Remove(playerWord);
                        currentRow++;
                        currentCol = 0;
                        correct++;
                    } else {
                        StartCoroutine(Flash(wordInRow[currentRow], false));
                    }
                    
                }

            }
        }
    }

    IEnumerator Flash(List<GameObject> l, bool correct) {
        
        // color to change to
        foreach (GameObject o in l) {
            if (correct) {
                o.GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f);
            } else {
                o.GetComponent<Image>().color = new Color(1.0f, 0.0f, 0.0f);
            }
        }

        yield return new WaitForSeconds(0.05f);

        // change color back
        foreach (GameObject o in l) {
            if (!correct) {
                Color c = new Color(0.0f, 0.0f, 0.0f);
                o.GetComponent<Image>().color = c;
            }
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
            createGame();
            Debug.Log(string.Join("\n", toRemember.ToArray()));
        }

        if (i == 0) {
            gameObject.SetActive(false);
            GameObject.Find("BigVegas(Clone)").GetComponent<BigVegas>().health_bar.SetActive(true);
        }
    }

}
