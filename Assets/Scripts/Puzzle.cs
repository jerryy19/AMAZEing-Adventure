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
    
    public GameObject puzzleTemplatePrefab;

    public GameObject createPuzzle() {
        int puzzleType = Random.Range(0, System.Enum.GetNames(typeof(PuzzleType)).Length - 1);
        return createPuzzle(puzzleType);
    }
    
    public GameObject createPuzzle(int puzzleType) {
        GameObject p = Instantiate(puzzleTemplatePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        p.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        // p.transform.localScale = new Vector3(1, 1, 1);
        p.SetActive(false);

        // TODO: create the puzzle types

        return p;
    }

    // displays puzzle on screen
    public void display(GameObject p) {
        p.SetActive(true);
    }
}
