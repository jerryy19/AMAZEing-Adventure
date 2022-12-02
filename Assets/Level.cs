using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    WALL = 0,
    FLOOR = 1,
    PUZZLE = 2,
    ENEMY = 3,
    SPEED = 4,
    SLOW = 5,
    MYSTERY = 6

}

public class Level
{
    public int width;                   // size of level (default 16 x 16 blocks)
    public int length;

    private int function_calls = 0;     // number of function calls during backtracking for solving the CSP

    public List<TileType>[,] grid;      // initialize 2D grid
    List<int[]> unassigned;             // useful to keep variables that are unassigned so far
    int num_enemies;
    int num_mysteries;

    public Level(int width, int length) {
        this.width = width;
        this.length = length;
        grid = new List<TileType>[width, length];
        unassigned = new List<int[]>();
    }


    public void createLevel() {
        // Random.InitState(1); // seed, if we want to keep this level to replay
        num_enemies = width * length / 25 + 1;              // at least one enemy will be added
        num_mysteries = (int)(width * length * 0.05);       // 5 percent of map will contain mystery blocks

        // create the wall perimeter of the level, and let the interior as unassigned
        // then try to assign variables to satisfy all constraints
        // *rarely* it might be impossible to satisfy all constraints due to initialization
        // in this case of no success, we'll restart the random initialization and try to re-solve the CSP
        bool success = false;
        while (!success) {
            for (int i = 0; i < num_enemies; i++) {
                // try until enemy placement is successful (unlikely that there will no places)
                while (true) {
                    // try a random location in the grid
                    int w = Random.Range(1, width - 1);
                    int l = Random.Range(1, length - 1);

                    // if grid location is empty/free, place it there
                    if (grid[w, l] == null) {
                        grid[w, l] = new List<TileType> { TileType.ENEMY };
                        break;
                    }
                }
            }

            for (int i = 0; i < num_mysteries; i++) {
                // try until mystery placement is successful (unlikely that there will no places)
                while (true) {
                    // try a random location in the grid
                    int w = Random.Range(1, width - 1);
                    int l = Random.Range(1, length - 1);

                    // if grid location is empty/free, place it there
                    if (grid[w, l] == null) {
                        grid[w, l] = new List<TileType> { TileType.ENEMY };
                        break;
                    }
                }
            }

            for (int w = 0; w < width; w++)
                for (int l = 0; l < length; l++)
                    if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                        grid[w, l] = new List<TileType> { TileType.WALL };
                    else
                    {
                        if (grid[w, l] == null) // does not have enemy already or some other assignment from previous run
                        {
                            // CSP will involve assigning variables to one of the following four values (ENEMY is predefined for some tiles)
                            List<TileType> candidate_assignments = new List<TileType> { TileType.WALL, TileType.FLOOR, TileType.PUZZLE, TileType.SPEED, TileType.SLOW };
                            Shuffle<TileType>(ref candidate_assignments);

                            grid[w, l] = candidate_assignments;
                            unassigned.Add(new int[] { w, l });
                        }
                    }

            success = BackTrackingSearch(grid, unassigned);
            if (!success)
            {
                Debug.Log("Could not find valid solution - will try again");
                unassigned.Clear();
                grid = new List<TileType>[width, length];
                function_calls = 0; 
            }
        }

    }


    bool BackTrackingSearch(List<TileType>[,] grid, List<int[]> unassigned) {
        // if there are too many recursive function evaluations, then backtracking has become too slow (or constraints cannot be satisfied)
        // to provide a reasonable amount of time to start the level, we put a limit on the total number of recursive calls
        // if the number of calls exceed the limit, then it's better to try a different initialization
        if (function_calls++ > 100000)       
            return false;

        // we are done!
        if (unassigned.Count == 0)
            return true;
        
        // select unassigned variable
        int[] uvar = unassigned[Random.Range(0, unassigned.Count)];
        unassigned.Remove(uvar);

        List<TileType> domain = new List<TileType>();
        domain.AddRange(grid[uvar[0], uvar[1]]);
        
        foreach (TileType t in domain) {
            if (CheckConsistency(grid, uvar, t)) {
                
                // add to assignment
                grid[uvar[0], uvar[1]] = new List<TileType> { t };
            
                // recurse
                bool result = BackTrackingSearch(grid, unassigned);  

                // if result consistent to csp
                if (result) {
                    return true;
                }

                // remove to assignment
                grid[uvar[0], uvar[1]] = new List<TileType>();
                grid[uvar[0], uvar[1]].AddRange(domain);
            }
        }

        return false;
    }


    // check if attempted assignment is consistent with the constraints or not
    bool CheckConsistency(List<TileType>[,] grid, int[] cell_pos, TileType t) {
        int w = cell_pos[0];
        int l = cell_pos[1];

        List<TileType> old_assignment = new List<TileType>();
        old_assignment.AddRange(grid[w, l]);
        grid[w, l] = new List<TileType> { t };

		// note that we negate the functions here i.e., check if we are consistent with the constraints we want
        bool areWeConsistent = !DoWeHaveTooFewInteriorJunk(grid) && !DoWeHaveTooManyInteriorJunk(grid) 
                            && !NoWallsCloseToEnemy(grid);

        grid[w, l] = new List<TileType>();
        grid[w, l].AddRange(old_assignment);
        return areWeConsistent;
    }


    // one type of constraint already implemented for you
    bool DoWeHaveTooManyInteriorJunk(List<TileType>[,] grid)
    {
        int[] number_of_assigned_elements = new int[] { 0, 0, 0, 0, 0 };
        for (int w = 0; w < width; w++)
            for (int l = 0; l < length; l++)
            {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                    continue;
                if (grid[w, l].Count == 1)
                    number_of_assigned_elements[(int)grid[w, l][0]]++;
            }

        if ((number_of_assigned_elements[(int)TileType.WALL] > num_enemies * 10) ||
             (number_of_assigned_elements[(int)TileType.PUZZLE] > (width + length) / 4) ||
             (number_of_assigned_elements[(int)TileType.SPEED] > (width + length) / 4) ||
             (number_of_assigned_elements[(int)TileType.SLOW] >= num_enemies / 2))
            return true;
        else
            return false;
    }

    // another type of constraint already implemented for you
    bool DoWeHaveTooFewInteriorJunk(List<TileType>[,] grid)
    {
        int[] number_of_potential_assignments = new int[] { 0, 0, 0, 0, 0 };
        for (int w = 0; w < width; w++)
            for (int l = 0; l < length; l++)
            {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                    continue;
                for (int i = 0; i < grid[w, l].Count; i++)
                    number_of_potential_assignments[(int)grid[w, l][i]]++;
            }

        if ((number_of_potential_assignments[(int)TileType.WALL] < (width * length) / 4) ||
             (number_of_potential_assignments[(int)TileType.PUZZLE] < num_enemies / 4) ||
             (number_of_potential_assignments[(int)TileType.SPEED] < num_enemies / 4) ||
             (number_of_potential_assignments[(int)TileType.SLOW] < num_enemies / 4))
            return true;
        else
            return false;
    }


    // must return true if there is no WALL adjacent to a enemy 
    // adjacency means left, right, top, bottom, and *diagonal* blocks
    bool NoWallsCloseToEnemy(List<TileType>[,] grid)
    {
        int[,] adj = new int[8, 2] {
            {-1, -1}, {-1, 0}, {-1, 1},
            {0, -1}, {0, 1},
            {1, -1}, {1, 0}, {1, 1}
        };

        for (int w = 0; w < width; w++) {

            for (int l = 0; l < length; l++) {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1) {
                    continue;
                }

                if (grid[w, l].Count == 1 && grid[w, l][0] == TileType.ENEMY) {
                    for (int i = 0; i < 8; i++) {
                        int adj_w = w + adj[i, 0];
                        int adj_l = l + adj[i, 1];
                        if (grid[adj_w, adj_l].Count == 1 && grid[adj_w, adj_l][0] == TileType.WALL) {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    // a helper function that randomly shuffles the elements of a list (useful to randomize the solution to the CSP)
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

}
