using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuzzleType
{
    TYPING = 0,             // type the words you see on screen as fast as possible (timer)
    WORD_MEMORY = 1,        // display 3 to 4 words on screen and have player type out all the words (timer)
    FOURDLE = 2,            // wordle but with 4 letter words
    FILL = 3,               // fill in the letters of the word
    SCRAMBLE = 4            // unscramble the word

}

public class Puzzle : MonoBehaviour
{
    public GameObject typingPrefab;
    public GameObject wordPrefab;
    public GameObject fourdlePrefab;
    public GameObject fillPrefab;
    public GameObject scramblePrefab;

    private Animator animation_controller;
    public int num_puzzlesTypes = 5;
    public PuzzleType puzzleType;
    GameObject p = null;
    private Timer timer;

    bool called;                // check if update method already called this method
    public bool done;           // did user finish puzzle (success or fail)
    public bool success;        // did user succeed in solving puzzle
    public bool interactable;   // if succeed in solving puzzle, then this puzzle is no longer interactable

    // Start is called before the first frame update
    void Start()
    {
        timer = GetComponent<Timer>();
        called = false;
        done = false;
        success = false;
        interactable = true;
        animation_controller = GetComponent<Animator>();
        createPuzzle();
    }

    // Update is called once per frame
    void Update()
    {
        switch (puzzleType) {
            case PuzzleType.TYPING: 
                done = p.GetComponent<TypingGameScript>().done;
                success = p.GetComponent<TypingGameScript>().success;
                break;
            case PuzzleType.WORD_MEMORY:
                done = p.GetComponent<WordMemoryGameScript>().done;
                success = p.GetComponent<WordMemoryGameScript>().success;
                break;
            case PuzzleType.FOURDLE:
                done = p.GetComponent<FourdleGameScript>().done;
                success = p.GetComponent<FourdleGameScript>().success;
                break;
            case PuzzleType.FILL:
                done = p.GetComponent<FillGameScript>().done;
                success = p.GetComponent<FillGameScript>().success;
                break;
            case PuzzleType.SCRAMBLE:
                done = p.GetComponent<ScrambleGameScript>().done;
                success = p.GetComponent<ScrambleGameScript>().success;
                break;
        }
        
        if (!called && done && success) {
            called = true;
            interactable = false;
            timer.set(3.0f, () => {
                animation_controller.SetBool("solved", true);
            });
            GameObject.Find("Level").GetComponent<Main>().solvedPuzzles++;
            Debug.Log(GameObject.Find("Level").GetComponent<Main>().solvedPuzzles);
        }
        if (!called && done && !success) {
            called = true;
            timer.set(3.0f, () => {
                animation_controller.SetBool("solved", true);
            });
            GameObject.Find("BigVegas(Clone)").GetComponent<BigVegas>().healthpoint -= 20;
            
        }

    }

    public void startPuzzle() {
        p.SetActive(true);
    }

    public void createPuzzle() {
        puzzleType = (PuzzleType) Random.Range(0, num_puzzlesTypes);
        createPuzzle(puzzleType);
    }
    
    public void createPuzzle(PuzzleType puzzleType) {
        called = false;
        done = false;
        success = false;
        switch (puzzleType) {
            case PuzzleType.TYPING: 
                p = Instantiate(typingPrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                p.name = "TypingGame"; 
                break;
            case PuzzleType.WORD_MEMORY:
                p = Instantiate(wordPrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                p.name = "WordMemoryGame"; 
                break;
            case PuzzleType.FOURDLE:
                p = Instantiate(fourdlePrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                p.name = "FourdleGame"; 
                break;
            case PuzzleType.FILL:
                p = Instantiate(fillPrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                p.name = "FillGame"; 
                break;
            case PuzzleType.SCRAMBLE:
                p = Instantiate(scramblePrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                p.name = "ScrambleGame"; 
                break;
        }

        p.transform.SetParent(GameObject.Find("Canvas").transform, false);
        p.SetActive(false);

    }

}
