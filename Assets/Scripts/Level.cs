using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    WALL = 0,
    FLOOR = 1,
    PUZZLE = 2,
    ENEMY = 3,
    SPEED = 4,
    SLOW = 5,
    MYSTERY = 6

}

public class Level {
    public int width;                   // size of level (default 16 x 16 blocks)
    public int length;

    private int function_calls = 0;     // number of function calls during backtracking for solving the CSP

    public List<TileType>[,] grid;      // initialize 2D grid
    List<int[]> unassigned;             // useful to keep variables that are unassigned so far
    public int num_enemies;
    public int num_mysteries;
    public bool success = false;

    // Convert each block on the grid to a singular number
    Graph<int> g;                       // graph with no wall tiles
    Graph<int> gWall;                   // graph with wall tiles


    public Level(int width, int length) {
        this.width = width;
        this.length = length;
        grid = new List<TileType>[width, length];
        unassigned = new List<int[]>();

        num_enemies = width * length / 50 + 1;              // at least one enemy will be added
        num_mysteries = (int)(width * length * 0.02);       // 5 percent of map will contain mystery blocks
        
    }


    public void createLevel() {
        // Random.InitState(1); // seed, if we want to keep this level to replay

        // create the wall perimeter of the level, and let the interior as unassigned
        // then try to assign variables to satisfy all constraints
        // *rarely* it might be impossible to satisfy all constraints due to initialization
        // in this case of no success, we'll restart the random initialization and try to re-solve the CSP
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
                        grid[w, l] = new List<TileType> { TileType.MYSTERY };
                        break;
                    }
                }
            }

            for (int w = 0; w < width; w++) {
                for (int l = 0; l < length; l++) {
                    if (w == 0 || l == 0 || w == width - 1 || l == length - 1) {
                        grid[w, l] = new List<TileType> { TileType.WALL };
                    } else {
                        if (grid[w, l] == null) { // does not have enemy already or some other assignment from previous run
                            // CSP will involve assigning variables to one of the following four values (ENEMY is predefined for some tiles)
                            List<TileType> candidate_assignments = new List<TileType> { TileType.WALL, TileType.FLOOR, TileType.PUZZLE, TileType.SPEED, TileType.SLOW };
                            Shuffle<TileType>(ref candidate_assignments);

                            grid[w, l] = candidate_assignments;
                            unassigned.Add(new int[] { w, l });
                        }
                    }
                }
            }

            success = BackTrackingSearch(grid, unassigned);
            if (!success) {
                Debug.Log("Could not find valid solution - will try again");
                unassigned.Clear();
                grid = new List<TileType>[width, length];
                function_calls = 0; 
            }
        }

        // Post Processing after CSP
        postProcessing();

    }


    // make graphs, connect walkable tiles to walkable tiles and make a start and end goal for player
    void postProcessing() {
        // edges can be made in 4 directions, no diagonal edges
        int[,] adj = new int[4, 2] {
            {-1, 0}, {1, 0},
            {0, -1}, {0, 1}
        };

        // Convert each block on the grid to a singular number
        g = new Graph<int>();      // graph with no wall
        gWall = new Graph<int>();  // graph with wall
        
        for (int w = 0; w < width; w++) {
            for (int l = 0; l < length; l++) {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1) {
                    continue;
                }
                int v = w * width + l;       // vertex

                if (grid[w, l][0] == TileType.WALL) {
                    gWall.addVertex(v);
                } else {
                    gWall.addVertex(v);
                    g.addVertex(v);
                }

                // for each neighbor
                for (int index = 0; index < 4; index++) {
                    int adj_w = w + adj[index, 0];
                    int adj_l = l + adj[index, 1];
                    int neighbor = adj_w * length + adj_l;     // neighbor

                    // if neighbor is an exterior wall
                    if (adj_w == 0 || adj_l == 0 || adj_w == width - 1 || adj_l == length - 1) {
                        continue;
                    }

                    // set edge weights
                    switch (grid[adj_w, adj_l][0]) {
                        case TileType.WALL:
                            gWall.addEdge(v, neighbor, 1000);
                            break;
                        case TileType.FLOOR:
                            g.addEdge(v, neighbor);
                            gWall.addEdge(v, neighbor, 1);
                            break;
                        case TileType.PUZZLE:
                            g.addEdge(v, neighbor);
                            gWall.addEdge(v, neighbor, 500);
                            break;
                        case TileType.ENEMY:
                            g.addEdge(v, neighbor);
                            gWall.addEdge(v, neighbor, 500);
                            break;
                        case TileType.SPEED:
                            g.addEdge(v, neighbor);
                            gWall.addEdge(v, neighbor, 300);
                            break;
                        case TileType.SLOW:
                            g.addEdge(v, neighbor);
                            gWall.addEdge(v, neighbor, 200);
                            break;
                        case TileType.MYSTERY:
                            g.addEdge(v, neighbor);
                            gWall.addEdge(v, neighbor, 50);
                            break;
                    }
                }
            }
        }

        // PLAYER START AND PLAYER END
        int wstart = Random.Range(1, width - 2);
        int lstart = 0;
        int start = wstart * width + lstart;

        int wend = Random.Range(1, width - 2);
        int lend = length - 1;
        int end = wend * width + lend;
        
        g.addVertex(start);
        gWall.addVertex(start);
        g.addVertex(end);
        gWall.addVertex(end);

        g.addEdge(start, start + 1);
        gWall.addEdge(start, start + 1);
        g.addEdge(start + 1, start);
        gWall.addEdge(start + 1, start);

        g.addEdge(end, end - 1);
        gWall.addEdge(end, end - 1);
        g.addEdge(end - 1, end);
        gWall.addEdge(end - 1, end);

        grid[wstart, lstart] = new List<TileType>() { TileType.FLOOR };
        grid[wend, lend] = new List<TileType>() { TileType.FLOOR };


        connectPaths();
    }

    // the pcg content may not have a path from tile to tile (connected graph)
    // so we will get nodes in each component and try to connect it to other components
    // to make the graph connected
    void connectPaths() {
        List<Vertex<int>> visited = new List<Vertex<int>>();
        visited.AddRange(g.vertices);
        
        // nodes in the graph that are in each tree
        List<Vertex<int>> components = new List<Vertex<int>>();
        
        while (visited.Count > 0) {
            Vertex<int> v = visited[0];
            components.Add(v);
            visited.RemoveAt(0);
            dfs(v, visited);
        }

        for (int c = 0; c < components.Count - 1; c++) {
            List<Vertex<int>> path = dijkstra(gWall, components[c].data, components[c + 1].data);
            for (int i = 0; i < path.Count; i++) {
                // turn numbers back to positions (w, l)
                int pw = path[i].data / length;
                int pl = path[i].data % length;
                if (grid[pw, pl][0] == TileType.WALL) {
                    grid[pw, pl][0] = TileType.FLOOR;
                }
            }
        }
    }

    void dfs(Vertex<int> v, List<Vertex<int>> visited) {
        List<Vertex<int>> neighbors = g.edges[v];

        foreach (Vertex<int> n in neighbors) {
            if (visited.IndexOf(n) == -1) continue;
            visited.Remove(n);
            dfs(n, visited);
        }

    }

    List<Vertex<int>> dijkstra(Graph<int> graph, int source, int dest) {
        Vertex<int> sourceNode = new Vertex<int>(source);
        Vertex<int> destNode = new Vertex<int>(dest);
        List<Vertex<int>> path = new List<Vertex<int>>();

        Dictionary<Vertex<int>, int> dist = new Dictionary<Vertex<int>, int>();
        Dictionary<Vertex<int>, bool> explored = new Dictionary<Vertex<int>, bool>();
        Dictionary<Vertex<int>, Vertex<int>> parent = new Dictionary<Vertex<int>, Vertex<int>>();
        PriorityQueue<Vertex<int>> Q = new PriorityQueue<Vertex<int>>();

        // initialize distances
        for (int i = 0; i < graph.Count; i++) {
            Vertex<int> v = graph.vertices[i];
            if (v.Equals(sourceNode)) {
                dist.Add(v, 0);
                parent.Add(v, v);
            } else {
                dist.Add(v, int.MaxValue);
                parent.Add(v, null);
            }

            Q.push(v, dist[v]);
            explored.Add(v, false);
        }


        while (Q.Count != 0) {
            // Entry<vertex, cost/priority/dist>
            Entry<Vertex<int>, int> e = Q.pop();       // get vertex with lowest distance and remove vertex from Q
            Vertex<int> u = e.key;
            int dist_u = e.value;
            explored[u] = true;

            List<Vertex<int>> neighbors = graph.edges[u];
            string s = $"({u.data/length},{u.data%length})";
            // for each neighbor of this vertex
            for (int i = 0; i < neighbors.Count; i++) {
                Vertex<int> v = neighbors[i];
                if (explored[v]) {
                    continue;
                }
                
                int d = dist_u + graph.getCost(u, v);
                if (d < dist[v]) {
                    Q.remove(v);
                    dist[v] = d;
                    Q.push(v, dist[v]);
                    parent[v] = u;
                }
            }
        }

        path.Add(destNode);
        while (!destNode.Equals(sourceNode)) { 
            destNode = parent[destNode];
            path.Insert(0, destNode);
        }

        return path;
    }



    bool BackTrackingSearch(List<TileType>[,] grid, List<int[]> unassigned) {
        // if there are too many recursive function evaluations, then backtracking has become too slow (or constraints cannot be satisfied)
        // to provide a reasonable amount of time to start the level, we put a limit on the total number of recursive calls
        // if the number of calls exceed the limit, then it's better to try a different initialization
        if (function_calls++ > 100000) {
            return false;
        }

        // we are done!
        if (unassigned.Count == 0) {
            return true;
        }
        
        // select unassigned variable
        int[] uvar = unassigned[Random.Range(0, unassigned.Count)];
        
        // remove assignment because be are assigning it 
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

        // variable needs to be assigned since it was not assigtned
        unassigned.Add(uvar);

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
                            && !TooLongWall(grid)
                            && !NoWallsCloseToEnemy(grid) && NoPuzzleEnemyMysteryWithinRadius(grid) 
                            && SpeedSlowWithinRadius2(grid) && NoLargeWallChunks(grid);

        grid[w, l] = new List<TileType>();
        grid[w, l].AddRange(old_assignment);
        return areWeConsistent;
    }

    bool NoLargeWallChunks(List<TileType>[,] grid) {
        int[,] adj = new int[8, 2] {
            {-1, -1}, {-1, 0}, {-1, 1},
            {0, -1}, {0, 1},
            {1, -1}, {1, 0}, {1, 1}
        };

        for (int w = 0; w < width; w++) {
            for (int l = 0; l < length; l++) {
                int count = 0;

                if (w == 0 || l == 0 || w == width - 1 || l == length - 1) {
                    continue;
                }

                if (grid[w, l].Count == 1 && grid[w, l][0] == TileType.WALL) {
                    for (int i = 0; i < 8; i++) {
                        int adj_w = w + adj[i, 0];
                        int adj_l = l + adj[i, 1];
                        if (grid[adj_w, adj_l].Count == 1 && grid[adj_w, adj_l][0] == TileType.WALL) {
                            count++;
                        }
                    }

                    if (count > 6) {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    // There must not be three or more consecutive -interior- wall blocks horizontally or vertically.
    bool TooLongWall(List<TileType>[,] grid) {
        int len = 4;

        // horizontal 
        for (int w = 0; w < width; w++) {
            int horizontal = 0;

            for (int l = 0; l < length; l++) {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1) {
                    continue;
                }

                if (grid[w, l].Count == 1 && grid[w, l][0] == TileType.WALL) {
                    if (++horizontal == len) {
                        return true;
                    }
                } else {
                    horizontal = 0;
                }
            }
        }

        // vertical 
        for (int l = 0; l < length; l++) {
            int vertical = 0;

            // horizontal 
            for (int w = 0; w < width; w++) {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1) {
                    continue;
                }

                if (grid[w, l].Count == 1 && grid[w, l][0] == TileType.WALL) {
                    if (++vertical == len) {
                        return true;
                    }
                } else {
                    vertical = 0;
                }
            }
        }
        
        return false;
    }

    bool SpeedSlowWithinRadius2(List<TileType>[,] grid) {
        if (!radiusCheckNoTileType(grid, 2, TileType.SPEED, TileType.SLOW)) {
            return false;
        }

        if (!radiusCheckNoTileType(grid, 2, TileType.SLOW, TileType.SPEED)) {
            return false;
        }

        return true;
    }
    

    // Puzzles, Enemies and Mystery tiles can not spawn with a 2 block radius of the same tile
    bool NoPuzzleEnemyMysteryWithinRadius(List<TileType>[,] grid) {
        if (!radiusCheckNoTileType(grid, 2, TileType.PUZZLE, TileType.PUZZLE)) {
            return false;
        }

        if (!radiusCheckNoTileType(grid, 3, TileType.ENEMY, TileType.ENEMY)) {
            return false;
        }

        if (!radiusCheckNoTileType(grid, 2, TileType.MYSTERY, TileType.MYSTERY)) {
            return false;
        }

        return true;
    }


    bool radiusCheckNoTileType(List<TileType>[,] grid, int radius, TileType tile, TileType adjacentTile) {
        for (int w = 0; w < width; w++) {
            for (int l = 0; l < length; l++) {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1) {
                    continue;
                }

                if (grid[w, l].Count == 1 && grid[w, l][0] == tile) {
                    for (int wr = -radius; wr < radius; wr++) { 
                        for (int lr = -radius; lr < radius; lr++) {
                            // out of bound check
                            if (w + wr < 0 || w + wr >= width || l + lr < 0 || l + lr >= length) continue;
                            
                            // current block check
                            if (w + wr == w && l + lr == l) continue;

                            // adjacent block check
                            if (grid[w + wr, l + lr].Count == 1 && grid[w + wr, l + lr][0] == adjacentTile) {
                                return false;
                            }
                        }
                    }
                }

            }
        }

        return true;
    }


    // one type of constraint already implemented for you
    bool DoWeHaveTooManyInteriorJunk(List<TileType>[,] grid) {
        int[] number_of_assigned_elements = new int[] { 0, 0, 0, 0, 0, 0, 0 };
        for (int w = 0; w < width; w++) {
            for (int l = 0; l < length; l++) {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1) { 
                    continue;
                }

                if (grid[w, l].Count == 1) {
                    number_of_assigned_elements[(int)grid[w, l][0]]++;
                }
            }
        }

        if ((number_of_assigned_elements[(int)TileType.WALL] > num_enemies * 20) ||
             (number_of_assigned_elements[(int)TileType.PUZZLE] > (width + length) / 4) ||
             (number_of_assigned_elements[(int)TileType.SPEED] > (width + length) / 4) ||
             (number_of_assigned_elements[(int)TileType.SLOW] >= num_enemies)) {

            return true;
        }

        return false;
    }


    // another type of constraint already implemented for you
    bool DoWeHaveTooFewInteriorJunk(List<TileType>[,] grid) {
        int[] number_of_potential_assignments = new int[] { 0, 0, 0, 0, 0, 0, 0 };
        for (int w = 0; w < width; w++) {
            for (int l = 0; l < length; l++) {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1) {
                    continue;
                }
                for (int i = 0; i < grid[w, l].Count; i++) {
                    number_of_potential_assignments[(int)grid[w, l][i]]++;
                }
            }
        }

        if ((number_of_potential_assignments[(int)TileType.WALL] < (width * length) / 4) ||
             (number_of_potential_assignments[(int)TileType.PUZZLE] < num_enemies / 4) ||
             (number_of_potential_assignments[(int)TileType.SPEED] < num_enemies / 4) ||
             (number_of_potential_assignments[(int)TileType.SLOW] < num_enemies / 2)) {
                return true;
        }

        return false;
    }


    // must return true if there is no WALL adjacent to a enemy 
    // adjacency means left, right, top, bottom, and *diagonal* blocks
    bool NoWallsCloseToEnemy(List<TileType>[,] grid) {
        if (!radiusCheckNoTileType(grid, 2, TileType.ENEMY, TileType.WALL)) {
            return false;
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
