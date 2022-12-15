using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // file system

public class ScrambleGameScript : MonoBehaviour
{

    public GameObject letterPrefab;

    List<GameObject> userPressedLetters = new List<GameObject>();   // ui for the letter boxes
    List<string> words = new List<string>();                        // our word bank

    private Timer timer;
    string theWord;
    int tries = 3;
    int currentIndex = 0;
    string scrambledWord;
    
    public bool done = false;
    public bool success = false;

    GameObject triesObj;
    
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

        using (StreamReader sr = File.OpenText("./Assets/Puzzle/Puzzles/wordBank.txt")) {
            string s = "";
            while ((s = sr.ReadLine()) != null) {
                words.Add(s.ToUpper());
            }
        }

        triesObj = Instantiate(letterPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
        triesObj.name = $"{tries}";
        triesObj.transform.GetChild(0).gameObject.GetComponent<Text>().text = triesObj.name;
        RectTransform pos = triesObj.GetComponent<RectTransform>();
        pos.anchoredPosition = new Vector2(-40, -40);
        pos.anchorMin = new Vector2(1, 1);
        pos.anchorMax = new Vector2(1, 1);
        pos.pivot = new Vector2(0.5f, 0.5f);

    }

    void createGame() {
        theWord = words[Random.Range(0, words.Count)];
        string temp = theWord;
        Debug.Log(theWord);

        List<char> theWordSplit = new List<char>();
        theWordSplit.AddRange(theWord.ToCharArray());
        Shuffle<char>(ref theWordSplit);

        scrambledWord = string.Join("", theWordSplit);

        float start = -200.0f;      // positioning in GUI
        for (int i = 0; i < 4; i++) {
            GameObject o = Instantiate(letterPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
            o.name = $"{scrambledWord[i]}";
            o.transform.GetChild(0).gameObject.GetComponent<Text>().text = o.name;

            RectTransform pos = o.GetComponent<RectTransform>();
            pos.anchoredPosition = new Vector2(start + 75, 25);
            start += 75 + 10;

        }

        start = -200.0f;
        for (int i = 0; i < 4; i++) {
            GameObject o = Instantiate(letterPrefab, new Vector3(0, 0, 0), Quaternion.identity, gamePanel.transform);
            o.name = $"{i}";
            o.transform.GetChild(0).gameObject.GetComponent<Text>().text = " ";

            RectTransform pos = o.GetComponent<RectTransform>();
            pos.anchoredPosition = new Vector2(start + 75, -25);
            start += 75 + 10;

            userPressedLetters.Add(o);
        }
    }

    private void Shuffle<T>(ref List<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!done && (success || tries == 0)) {
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

            if (success) {
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
    }

    void OnGUI()
    {
        if (tries == 0) return;

        Event e = Event.current;

        if (gamePanel.gameObject.activeSelf) {

            if (e.type == EventType.KeyDown && e.keyCode.ToString() != "None") {
                if (e.keyCode >= KeyCode.A && e.keyCode <= KeyCode.Z) {
                    if (currentIndex >= 4) return;
                    GameObject o = userPressedLetters[currentIndex];
                    o.name = $"{e.keyCode}";
                    o.transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{e.keyCode}";
                    currentIndex++;
                    
                }

                if (e.keyCode == KeyCode.Backspace) {
                    if (currentIndex == 0) return;
                    GameObject o = userPressedLetters[--currentIndex];
                    o.name = $"{currentIndex}";
                    o.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
                }

                if (e.keyCode == KeyCode.Return) {

                    // check if the whole row is filled, if not then dont do anything
                    foreach (GameObject o in userPressedLetters) {
                        if (o.transform.GetChild(0).gameObject.GetComponent<Text>().text == "") {
                            return;
                        }
                    }

                    // check if player typed word matches the unscrambled word
                    string playerWord = "";
                    foreach (GameObject o in userPressedLetters) {
                        playerWord += o.transform.GetChild(0).gameObject.GetComponent<Text>().text;
                    }

                    if (playerWord == theWord) {
                        success = true;
                    } else {
                        StartCoroutine(Flash());
                        tries--;
                        triesObj.transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{tries}";
                    }
                }

            }
        }
    }

    IEnumerator Flash() {
        // flash red
        foreach (GameObject o in userPressedLetters) {
                o.GetComponent<Image>().color = new Color(1.0f, 0.0f, 0.0f);
        }

        yield return new WaitForSeconds(0.05f);

        // change color back to transparent
        foreach (GameObject o in userPressedLetters) {
            Color c = new Color(0.0f, 0.0f, 0.0f);
            c.a = 0.0f;
            o.GetComponent<Image>().color = c;
        }
    }


    // 0 = close instructions panel and display Typing Game
    // 1 = close instructions panel and exit puzzle 
    public void executeGame(int i) {
        if (i == 1) {
            instructionsPanel.gameObject.SetActive(false);
            gamePanel.gameObject.SetActive(true);
            createGame();
        }

        if (i == 0) gameObject.SetActive(false);
    }
}
