using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TileType
{
    WALL = 0,
    FLOOR = 1,
    PUZZLE = 2,
    ENEMY = 3,
    SPEED = 4,
    SLOW = 5,
    MYSTERY = 6

}

public class Level : MonoBehaviour
{
    public int width;   // size of level (default 16 x 16 blocks)
    public int length;
    public float storey_height;   // height of walls

    private Bounds bounds;                   // size of ground plane in world space coordinates 
    private int function_calls = 0;          // number of function calls during backtracking for solving the CSP

    public Level(int width, int length, float storey_height) {
        this.width = width;
        this.length = length;
        this.storey_height = storey_height;
    }


    void createLevel() {
        // Random.InitState(1); // seed, if we want to keep this level to replay

    }

}
