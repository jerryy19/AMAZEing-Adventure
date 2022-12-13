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

    public int num_puzzlesTypes = 5;
    public PuzzleType puzzleType;
    GameObject p = null;

    public bool done;           // did user finish puzzle (success or fail)
    public bool success;        // did user succeed in solving puzzle
    public bool interactable;   // if succeed in solving puzzle, then this puzzle is no longer interactable

    // Start is called before the first frame update
    void Start()
    {
        done = false;
        success = false;
        interactable = true;
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
        
        if (done && success) {
            interactable = false;

            // TODO: maybe animations to show success
        }
    }


    public void createPuzzle() {
        puzzleType = (PuzzleType) Random.Range(0, num_puzzlesTypes);
        createPuzzle(puzzleType);
    }
    
    public void createPuzzle(PuzzleType puzzleType) {
        done = false;
        success = false;
        switch (puzzleType) {
            case PuzzleType.TYPING: 
                p = Instantiate(typingPrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                break;
            case PuzzleType.WORD_MEMORY:
                p = Instantiate(wordPrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                break;
            case PuzzleType.FOURDLE:
                p = Instantiate(fourdlePrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                break;
            case PuzzleType.FILL:
                p = Instantiate(fillPrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                break;
            case PuzzleType.SCRAMBLE:
                p = Instantiate(scramblePrefab, new Vector3(0, 0, 0), Quaternion.identity); 
                break;
        }

        p.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        p.SetActive(false);

    }

    // play puzzle on screen
    public void play() {
        p.SetActive(true);
    }
}
