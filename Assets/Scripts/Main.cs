using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public GameObject bananaMan;
    // Used for instantiating the different objects; each location is the center of a square
    public Vector3[,] squareLocations;
    // Start is called before the first frame update
    void Start()
    {

        // create default levels
        // levels is a grid that tells us what goes where
        Level level = new Level(16, 16);
        level.createLevel();
        List<TileType>[,] grid = level.grid;
        squareLocations = new Vector3[16, 16];
        visualizeLevel(grid, level);

        // instantiations
        // add puzzle to level

        // add enemy to level


    }

    // TODO: WALL INSTANTIATIONS 
    // void wallInstantiations() {
    //         List<GameObject> plist = new List<GameObject>();

    //     plist.AddRange(Resources.LoadAll<GameObject>("CityVoxelPack/Assets/buildings/medium/Prefabs"));

    //     Bounds bounds = GetComponent<Collider>().bounds; 

    //     GameObject exteriorWalls = Instantiate(plist[Random.Range(0, plist.Count-1)], new Vector3(0, 0, 0), Quaternion.identity);
    //     GameObject o = exteriorWalls.transform.GetChild(0).gameObject;
    //     o.AddComponent<MeshCollider>();
    //     o.GetComponent<MeshCollider>().convex = true;
    //     Bounds currentSize = o.GetComponent<Collider>().bounds;
    //     // exteriorWalls.name = "exteriorWalls";
    //     // exteriorWalls.transform.localScale = new Vector3(0.5f * bounds.size[0], 1.0f, 0.5f * bounds.size[2]);
        
    //     float newSizeRatioX = bounds.size.x / currentSize.size.x;
    //     float newSizeRatioZ = bounds.size.z / currentSize.size.z;

    //     float minimumNewSizeRatio = Mathf.Min(newSizeRatioX, newSizeRatioZ);

    //     Vector3 newScale = new Vector3(exteriorWalls.transform.localScale.x * minimumNewSizeRatio, exteriorWalls.transform.localScale.y, exteriorWalls.transform.localScale.z * minimumNewSizeRatio);
    //     exteriorWalls.transform.localScale = newScale;
    // }

    // TEMPORARY FUNCTION TO SEE THE LEVEL DESIGN
    void visualizeLevel(List<TileType>[,] grid, Level level) {
        Color floor = new Color(1f, 1f, 1f);        // white
        Color wall = new Color(0f, 0f, 0f);         // black
        Color mystery = new Color(0f, 0f, 1f);      // blue
        Color speed = new Color(0f, 1f, 0f);        // green
        Color slow = new Color(1f, 0f, 0f);         // red

        Color enemy = new Color(1f, 1f, 0f);        // yellow
        Color puzzle = new Color(1f, 0f, 1f);       // purp


        for (int w = 0; w < level.width; w++) {
            for (int l = 0; l < level.length; l++) {
                GameObject o = GameObject.CreatePrimitive(PrimitiveType.Plane);
                // if (grid[w,l].Count == 5) {
                //     Debug.Log("TRUE");
                // }
                // Debug.Log($"{w},{l} {c}, {grid[w,l][0]}");
                switch (grid[w, l][0]) {
                    case TileType.FLOOR:
                        o.GetComponent<Renderer>().material.color = floor;
                        break;
                    
                    case TileType.WALL:
                        o.GetComponent<Renderer>().material.color = wall;
                        break;
                    
                    case TileType.PUZZLE:
                        o.GetComponent<Renderer>().material.color = puzzle;
                        break;
                    
                    case TileType.ENEMY:
                        o.GetComponent<Renderer>().material.color = enemy;
                        break;
                    
                    case TileType.SPEED:
                        o.GetComponent<Renderer>().material.color = speed;
                        break;
                    
                    case TileType.SLOW:
                        o.GetComponent<Renderer>().material.color = slow;
                        break;
                    
                    case TileType.MYSTERY:
                        o.GetComponent<Renderer>().material.color = mystery;
                        break;
                    

                }
                
                o.transform.localScale -= new Vector3(0.8f, 0, 0.8f);
                o.transform.position = new Vector3(w * 2, 0, l * 2);
                squareLocations[w, l] = new Vector3(w * 2, 0, l * 2 + 1);
                // TODO: Move this instantiation to Start or another method; just here for testing.
                if (grid[w,l][0] == TileType.ENEMY)
                    Instantiate(bananaMan, squareLocations[w, l], Quaternion.identity);
                
                o.name = $"({w}, {l})";
                o.transform.SetParent(transform, false);
            }
        }

        // // START
        // GameObject x = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // x.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
        // x.transform.position = new Vector3(Random.Range(1, level.width - 1) * 10, 0, 0);
        // x.transform.localScale = new Vector3(7, 7, 7);

        // // GOAL
        // GameObject y = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // y.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0.5f);
        // y.transform.position = new Vector3(Random.Range(1, level.width - 1) * 10, 0, (level.length - 1) * 10);
        // y.transform.localScale = new Vector3(7, 7, 7);
    }

    // Update is called once per frame
    void Update()
    {
        // player movement in another script
        // enemy movement in another script
        // menu/tutorial scene stuff?
    }
}
