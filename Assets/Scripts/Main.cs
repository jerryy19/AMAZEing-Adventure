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
    public GameObject playerPrefab;     // player
    
    GameObject playerObj;
    public int solvedPuzzles = 0;
    public Level level;
    
    [SerializeField]
    private List<GameObject> wallList = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        wallList.AddRange(Resources.LoadAll<GameObject>("CityVoxelPack/Assets/buildings/medium/Prefabs"));

        // create default levels
        // levels is a grid that tells us what goes where
        level = new Level(16, 16);
        level.createLevel();

        // instantiations game
        instantiateGame(level);
    }

    // instantiate all game objects
    public void instantiateGame(Level level) {
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

                switch (level.grid[w, l][0]) {
                    case TileType.FLOOR:
                        o.GetComponent<Renderer>().material.color = floor;
                        break;
                    
                    case TileType.WALL:
                        o.GetComponent<Renderer>().material.color = wall;
                        // exterior wall
                        if (w == 0 || w == level.width - 1 || l == 0 || l == level.length - 1) {
                            initExteriorWall(o);
                        } else {
                            initInteriorWall(o);
                        }

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
                        initSpeed(o.transform.position);
                        break;
                    
                    case TileType.SLOW:
                        o.GetComponent<Renderer>().material.color = slow;
                        initSlow(o.transform.position);
                        break;
                    
                    case TileType.MYSTERY:
                        o.GetComponent<Renderer>().material.color = mystery;
                        initMystery(o.transform.position);
                        break;

                }
            }
        }


        playerObj = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        playerObj.transform.position = new Vector3(level.playerStart.x * 2, 0, level.playerStart.y * 2);


    }

    // instantiate exterior walls
    void initExteriorWall(GameObject platformObj) {
        Bounds bounds = platformObj.GetComponent<Collider>().bounds;

        // https://answers.unity.com/questions/1372385/object-size-changing-issue-w-box-collider.html
        // collider bounds would automatically scale according to transform.localScale but when 
        // changing transform.localScale when scripting collider bounds does not automatically scale
        bounds.size *= 0.2f;            //  manually scale it

        int index = Random.Range(0, wallList.Count);
        // exclude some assets
        while (index >= 4 && index <= 9) {
            index = Random.Range(0, wallList.Count);
        }

        GameObject buildingObj = Instantiate(wallList[index], new Vector3(0, 0, 0), Quaternion.identity);
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
        // buildingObjChild.GetComponent<MeshCollider>().convex = true;
        Bounds buildingBounds = buildingObjChild.GetComponent<Collider>().bounds;

        float ratio = Mathf.Min(bounds.size.x / buildingBounds.size.x, bounds.size.z / buildingBounds.size.z);

        buildingObj.transform.localScale *= ratio;
    }

    // instantiate interior walls
    void initInteriorWall(GameObject platformObj) {
        Bounds bounds = platformObj.GetComponent<Collider>().bounds;
        bounds.size *= 0.2f;            //  manually scale it

        int index = Random.Range(0, wallList.Count);

        GameObject buildingObj = Instantiate(wallList[index], new Vector3(0, 0, 0), Quaternion.identity);
        buildingObj.transform.position = platformObj.transform.position;
        buildingObj.transform.SetParent(transform, false);


        GameObject buildingObjChild = buildingObj.transform.GetChild(0).gameObject;
        // add mesh collider to child because imported asset does not have
        buildingObjChild.AddComponent<MeshCollider>();
        // buildingObjChild.GetComponent<MeshCollider>().convex = true;
        Bounds buildingBounds = buildingObjChild.GetComponent<Collider>().bounds;

        float ratio = Mathf.Min(bounds.size.x / buildingBounds.size.x, bounds.size.z / buildingBounds.size.z);

        buildingObj.transform.localScale *= ratio;
    }

    // instantiate puzzles
    void initPuzzle(Vector3 pos) {
        GameObject p = Instantiate(giraffePrefab, pos, Quaternion.identity);
        p.name = "Puzzle";
        p.transform.SetParent(transform, false);

    }

    // instantiate enemies
    void initEnemy(Vector3 pos) {
        GameObject e = Instantiate(bananaMan, pos, Quaternion.identity);
        e.name = "Enemy";
        e.transform.SetParent(transform, false);
    }

    // instantiate speed zone
    void initSpeed(Vector3 pos) {
        GameObject sp = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sp.transform.localScale = new Vector3(0.2f, 1.0f, 0.2f);
        sp.transform.position = pos + new Vector3(0.0f, -0.05f, 0.0f);
        sp.name = "Speed";
        sp.transform.SetParent(transform, false);
        sp.AddComponent<Speed>();
    }


    // instantiate slow zone
    void initSlow(Vector3 pos) {
        GameObject sl = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sl.transform.localScale = new Vector3(0.2f, 1.0f, 0.2f);
        sl.transform.position = pos + new Vector3(0.0f, -0.05f, 0.0f);
        sl.name = "Slow";
        sl.transform.SetParent(transform, false);
        sl.AddComponent<Slow>();
    }

    // instantiate mystery zone (shows enemies when standing on it)
    void initMystery(Vector3 pos) {
        GameObject m = GameObject.CreatePrimitive(PrimitiveType.Plane);
        m.transform.localScale = new Vector3(0.2f, 1.0f, 0.2f);
        m.transform.position = pos + new Vector3(0.0f, -0.05f, 0.0f);
        m.name = "Mystery";
        m.transform.SetParent(transform, false);
        // m.AddComponent<Mystery>();
    }

    // Update is called once per frame
    void Update()
    {

        // INTERACT WITH PUZZLE
        if (Input.GetMouseButtonDown(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10.0f)) {
                if (hit.collider.name.IndexOf("Plane") != -1) {
                    GameObject puzzleGameObject = hit.collider.transform.parent.gameObject;
                    Puzzle p = puzzleGameObject.GetComponent<Puzzle>();
                    if (p.interactable) {
                        if (p.done && !p.success) {
                            p.createPuzzle(p.puzzleType);
                        }
                        p.startPuzzle();
                    }
                }
            }
        }


        if (solvedPuzzles == level.num_puzzles && checkWin()) {
            Debug.Log("hi");
        }


    }

    bool checkWin() {
        int x = (int)Mathf.Floor(playerObj.transform.position.x / 2);
        int z = (int)Mathf.Floor(playerObj.transform.position.z / 2);

        return level.playerGoal.x == x && level.playerGoal.y == z;
    }

}
