using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public GameObject bananaMan;        // enemy prefab
    public GameObject giraffePrefab;    // puzzle prefab
    
    
    [SerializeField]
    private List<GameObject> plist = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        plist.AddRange(Resources.LoadAll<GameObject>("CityVoxelPack/Assets/buildings/medium/Prefabs"));

        // create default levels
        // levels is a grid that tells us what goes where
        Level level = new Level(16, 16);
        level.createLevel();
        List<TileType>[,] grid = level.grid;

        // instantiations game
        instantiateGame(grid, level);


    }

    void instantiateGame(List<TileType>[,] grid, Level level) {
        Color floor = new Color(1f, 1f, 1f);        // white
        Color wall = new Color(0f, 0f, 0f);         // black
        Color mystery = new Color(0f, 0f, 1f);      // blue
        Color speed = new Color(0f, 1f, 0f);        // green
        Color slow = new Color(1f, 0f, 0f);         // red

        Color enemy = new Color(1f, 1f, 0f);        // yellow
        Color puzzle = new Color(1f, 0f, 1f);       // purp

        GameObject gridObj = new GameObject();
        gridObj.name = "Grid";
        gridObj.transform.SetParent(transform, false);

        for (int w = 0; w < level.width; w++) {
            for (int l = 0; l < level.length; l++) {
                GameObject o = GameObject.CreatePrimitive(PrimitiveType.Plane);
                o.name = $"({w}, {l})";
                o.transform.SetParent(gridObj.transform, false);
                o.transform.localScale = new Vector3(0.2f, 1, 0.2f);
                o.transform.position = new Vector3(w * 2, 0, l * 2);

                if (level.playerStart == new Vector2(w, l) || level.playerGoal == new Vector2(w, l)) continue;

                // exterior wall
                if (w == 0 || w == level.width - 1 || l == 0 || l == level.length - 1) {
                    initExteriorWall(o);
                }

                switch (grid[w, l][0]) {
                    case TileType.FLOOR:
                        o.GetComponent<Renderer>().material.color = floor;
                        break;
                    
                    case TileType.WALL:
                        o.GetComponent<Renderer>().material.color = wall;
                        break;
                    
                    case TileType.PUZZLE:
                        o.GetComponent<Renderer>().material.color = puzzle;
                        initPuzzle(o.transform.position);
                        break;
                    
                    case TileType.ENEMY:
                        o.GetComponent<Renderer>().material.color = enemy;
                        initEnemy(o.transform.position);
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
            }
        }
        // GameObject x = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // x.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
        // x.transform.position = new Vector3(Random.Range(1, level.width - 1) * 10, 0, 0);
        // x.transform.localScale = new Vector3(7, 7, 7);
    }

    void initExteriorWall(GameObject platformObj) {
        Bounds bounds = platformObj.GetComponent<Collider>().bounds;

        // https://answers.unity.com/questions/1372385/object-size-changing-issue-w-box-collider.html
        // collider bounds would automatically scale according to transform.localScale but when 
        // changing transform.localScale when scripting collider bounds does not automatically scale
        bounds.size *= 0.2f;            //  manually scale it

        int index = Random.Range(0, plist.Count);
        // exclude some assets
        while (index >= 4 && index <= 9) {
            index = Random.Range(0, plist.Count);
        }

        GameObject buildingObj = Instantiate(plist[index], new Vector3(0, 0, 0), Quaternion.identity);
        buildingObj.transform.position = platformObj.transform.position;
        buildingObj.transform.SetParent(transform, false);
        // building face directions
        if (buildingObj.transform.position.x == 0) {
            buildingObj.transform.Rotate(0.0f, 90.0f, 0.0f);
        }
        if (buildingObj.transform.position.x == 30) {
            buildingObj.transform.Rotate(0.0f, 270.0f, 0.0f);
        }
        if (buildingObj.transform.position.z == 30) {
            buildingObj.transform.Rotate(0.0f, 180.0f, 0.0f);
        }


        GameObject buildingObjChild = buildingObj.transform.GetChild(0).gameObject;
        // add mesh collider to child because imported asset does not have
        buildingObjChild.AddComponent<MeshCollider>();
        buildingObjChild.GetComponent<MeshCollider>().convex = true;
        Bounds buildingBounds = buildingObjChild.GetComponent<Collider>().bounds;

        float ratio = Mathf.Min(bounds.size.x / buildingBounds.size.x, bounds.size.z / buildingBounds.size.z);

        buildingObj.transform.localScale *= ratio;
    }

    void initPuzzle(Vector3 pos) {
        GameObject p = Instantiate(giraffePrefab, pos, Quaternion.identity);
        p.name = "Puzzle";
        p.transform.SetParent(transform, false);

    }

    void initEnemy(Vector3 pos) {
        GameObject e = Instantiate(bananaMan, pos, Quaternion.identity);
        e.name = "Enemy";
        e.transform.SetParent(transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        // player movement in another script
        // enemy movement in another script
        // menu/tutorial scene stuff?
    }
}
