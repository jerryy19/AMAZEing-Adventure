using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // file system

public class FourdleGameScript : MonoBehaviour
{
    public GameObject letterPrefab;

    List<string> words = new List<string>();                    // our word bank
    List<GameObject>[] wordInRow = new List<GameObject>[5];     // letter outline ui

    private Timer timer;
    string theWord;
    int currentRow = 0;
    int currentCol = 0;
    
    public bool done = false;
    public bool success = false;

    GameObject instructionsPanel;
    GameObject gamePanel;
    GameObject resultsPanel;

    // Start is called before the first frame update
    void Start()
    {
        instructionsPanel = gameObject.transform.GetChild(1).gameObject;
        gamePanel = gameObject.transform.GetChild(2).gameObject;
        resultsPanel = gameObject.transform.GetChild(3).gameObject;

        timer = GetComponent<Timer>();

        using (StreamReader sr = File.OpenText("./Assets/UI/Puzzles/wordBank.txt")) {
            string s = "";
            while ((s = sr.ReadLine()) != null) {
                words.Add(s.ToUpper());
            }
        }

        // initialize the list for letter outline on UI 
        for (int i = 0; i < 5; i++) {
            wordInRow[i] = new List<GameObject>();
        }

        createGame();
        Debug.Log(theWord);
    }

    void createGame() {

        GameObject title = new GameObject();
        title.name = "title";
        title.transform.SetParent(gamePanel.transform, false);
        title.AddComponent<RectTransform>();
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 115.0f);
        title.GetComponent<RectTransform>().sizeDelta = new Vector2(200.0f, 100.0f);
        title.AddComponent<Text>();
        title.GetComponent<Text>().text = "Fourdle";
        title.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        title.GetComponent<Text>().fontSize = 30;
        title.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        // choose a random 4 letter word
        theWord = words[Random.Range(0, words.Count)];

        float s = 40.0f;          // change the size of prefab to this

        float vert = 72.0f;
        for (int row = 0; row < 5; row++) {
            float hor = -100.0f;      // positioning in GUI
            for (int i = 0; i < 4; i++) {
                GameObject o = Instantiate(letterPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        Event e = Event.current;

        if (gamePanel.gameObject.activeSelf) {
            if (currentRow >= 5) return;

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
                    
                    if (!words.Contains(playerWord)) return;

                    // game logic check row against word(theWord)
                    string temp = theWord;      // helper
                    string space = "";          // helper
                    for (int i = 0; i < 4; i++) {
                        GameObject o = wordInRow[currentRow][i];
                        string t = o.transform.GetChild(0).gameObject.GetComponent<Text>().text;
                        // if correct letter, change color to yellow
                        if (temp.Contains(t) && temp[i] != t[0]) {
                            o.GetComponent<Image>().color = new Color(1.0f, 1.0f, 0.0f);
                        }

                        // if correct pos, change color to green
                        if (temp[i] == t[0]) {
                            o.GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f);
                        }
                        space += " ";
                        temp = space + temp.Substring(i + 1);

                    }

                    // if guess is correct
                    if (playerWord == theWord || currentRow == 4) {
                        done = true;
                        gamePanel.gameObject.SetActive(false);
                        resultsPanel.gameObject.SetActive(true);
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

                        if (playerWord == theWord) {
                            success = true;
                            successText = "SUCCESS";
                            text.color = new Color(0.0f, 1.0f, 0.0f);
                        } else {
                            successText = "FAIL";
                            text.color = new Color(1.0f, 0.0f, 0.0f);

                            GameObject actualWord = new GameObject();
                            actualWord.name = "actualWordText";
                            actualWord.AddComponent<RectTransform>();
                            actualWord.GetComponent<RectTransform>().sizeDelta = new Vector2(400.0f, 100.0f);
                            actualWord.transform.SetParent(resultsPanel.transform, false);

                            // text itself
                            Text text2 = actualWord.AddComponent<Text>();
                            text2.GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                            text2.fontSize = 20;
                            text2.alignment = TextAnchor.LowerCenter;
                            text2.GetComponent<Text>().text = $"Word: {theWord}";
                        }

                        text.GetComponent<Text>().text = successText;

                        // disable after some time
                        timer.set(3.0f, () => {
                            gameObject.SetActive(false);
                        });
                    }

                    currentRow++;
                    currentCol = 0;
                }

            }
        }
    }



    // 0 = close instructions panel and display Typing Game
    // 1 = close instructions panel and exit puzzle 
    public void executeGame(int i) {
        if (i == 1) {
            instructionsPanel.gameObject.SetActive(false);
            gamePanel.gameObject.SetActive(true);
        }

        if (i == 0) gameObject.SetActive(false);
    }


}
