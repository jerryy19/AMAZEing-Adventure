using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    Level gameIntro = new Level(7, 7);
    Level puzzleIntro = new Level(7, 7);
    Level enemyIntro = new Level(7, 7);
    Level zoneIntro = new Level(7, 7);

    // Start is called before the first frame update
    void Start()
    {
        // gameIntro.custom = true;
        // puzzleIntro.custom = true;
        // enemyIntro.custom = true;
        // zoneIntro.custom = true;

        for (int w = 0; w < 7; w++) {
            for (int l = 0; l < 7; l++) {
                // exterior wall
                if (w == 0 || l == 0 || w == 6 || l == 6) {
                    gameIntro.grid[w, l] = new List<TileType> { TileType.WALL };
                    puzzleIntro.grid[w, l] = new List<TileType> { TileType.WALL };
                    enemyIntro.grid[w, l] = new List<TileType> { TileType.WALL };
                    zoneIntro.grid[w, l] = new List<TileType> { TileType.WALL };
                }

                // start and goal
                if ((w == 2 && l == 0) || (w == 4 && l == 6)) {
                    gameIntro.grid[w, l] = new List<TileType> { TileType.WALL };
                    puzzleIntro.grid[w, l] = new List<TileType> { TileType.WALL };
                    enemyIntro.grid[w, l] = new List<TileType> { TileType.WALL };
                    zoneIntro.grid[w, l] = new List<TileType> { TileType.WALL };
                }
            }
        }

        gameIntro.grid[4, 3] = new List<TileType> { TileType.PUZZLE };

        puzzleIntro.grid[1, 2] = new List<TileType> { TileType.PUZZLE };
        puzzleIntro.grid[1, 4] = new List<TileType> { TileType.PUZZLE };
        puzzleIntro.grid[3, 5] = new List<TileType> { TileType.PUZZLE };
        puzzleIntro.grid[5, 2] = new List<TileType> { TileType.PUZZLE };
        puzzleIntro.grid[5, 4] = new List<TileType> { TileType.PUZZLE };

        enemyIntro.grid[1, 5] = new List<TileType> { TileType.ENEMY };
        enemyIntro.grid[2, 2] = new List<TileType> { TileType.WALL };
        enemyIntro.grid[3, 3] = new List<TileType> { TileType.WALL };
        enemyIntro.grid[5, 1] = new List<TileType> { TileType.MYSTERY };

        zoneIntro.grid[2, 4] = new List<TileType> { TileType.SPEED };
        zoneIntro.grid[4, 4] = new List<TileType> { TileType.WALL };
        zoneIntro.grid[4, 2] = new List<TileType> { TileType.SLOW };

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
