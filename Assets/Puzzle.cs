using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuzzleType
{
    ARROW_KEY = 0,          // click the arrow keys on keyboard that you see on screen
    ADDITION = 1,           // 2 digits
    MULTIPLICATION = 2,     // 1 digit
    NUMBER_MEMORY = 3,      // enter the 3 highest numbers you saw
    AIM_TRAINER = 4,        // click as many circles as you can
    TYPING = 5

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
