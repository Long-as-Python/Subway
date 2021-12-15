using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool showDebug;

    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject block;
    [SerializeField] private GameObject Wall;
    [SerializeField] private GameObject WallGround;
    [SerializeField] private GameObject Rails;
    [SerializeField] private GameObject EndRail;
    [SerializeField] private List<GameObject> Coin;
    public RailsPlacer railsPlacer;
    public List<RailsPlacer.PlacedRail> wallsPlaced;
    public int[,] data
    {
        get; private set;
    }

    public float hallWidth
    {
        get; private set;
    }
    public float hallHeight
    {
        get; private set;
    }

    public int startRow
    {
        get; private set;
    }
    public int startCol
    {
        get; private set;
    }

    public int goalRow
    {
        get; private set;
    }
    public int goalCol
    {
        get; private set;
    }

    private MazeDataGenerator dataGenerator;

    void Awake()
    {
        railsPlacer = FindObjectOfType<RailsPlacer>();
        dataGenerator = new MazeDataGenerator();

        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols, TriggerEventHandler startCallback = null, TriggerEventHandler goalCallback = null)
    {
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogWarning("Odd numbers work better for dungeon size.");
        }
        //Deleting old map
        DisposeOldMaze();

        data = dataGenerator.FromDimensions(sizeRows, sizeCols);
        FindStartPosition();
        FindGoalPosition();
        //store values used to generate this mesh
        DisplayLevel(sizeRows, sizeCols);
        //PlaceStartTrigger(startCallback);
        //PlaceGoalTrigger(goalCallback);
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols)
    {
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogWarning("Odd numbers work better for dungeon size.");
        }
        //Deleting old map
        DisposeOldMaze();
        FindStartPosition();
        DisplayLevel(sizeRows, sizeCols);
        FindGoalPosition();

        //data = dataGenerator.FromDimensions(sizeRows, sizeCols);
        //store values used to generate this mesh
        //PlaceStartTrigger(startCallback);
        //PlaceGoalTrigger(goalCallback);
    }

    private void DisplayLevel(int rows, int cols)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int b = 0; b < cols; b++)
            {
                if (b == 1 || i == 1 || b == cols - 2 || i == rows - 2)
                {
                    data[i, b] = 0;
                }
            }
        }
        bool isCoinPlaced = false;
        int a = Random.Range(1, cols - 2);
        Debug.Log(a);
        int CoinCoordsX = Random.Range(1, rows - 2);
        int CoinCoordsZ = Random.Range(1, rows - 2);
        bool check = false;
        for (int i = 0; i < rows; i++)
        {
            for (int b = 0; b < cols; b++)
            {
                if (i == 0 && b == 3)
                {
                    PlaceGridObj(Rails, "Start Pos", i, b);
                    RailsPlacer.PlacedRail start = new RailsPlacer.PlacedRail();
                    start.height = i;
                    start.width = b;
                    start.ClickX = Mathf.RoundToInt(i);
                    start.ClickZ = Mathf.RoundToInt(b);
                    railsPlacer.placedRails.Add(start);
                }
                else if (!check)
                {
                    if (data[rows - 2, a] == 1)
                    {
                        data[rows - 2, a] = 0;
                    }
                    RailsPlacer.PlacedRail start = new RailsPlacer.PlacedRail();
                    start.height = rows - 1;
                    start.width = a;
                    start.ClickX = rows - 1;
                    start.ClickZ = a;
                    railsPlacer.endRail = start;
                    Debug.Log("FINISH ");
                    check = true;
                    PlaceGridObj(EndRail, "Finish Pos", rows - 1, a);
                }
                else if (i == 0 || b == 0 || i == rows - 1 || b == cols - 1)
                {
                    PlaceGridObj(Wall, "Wall Block", i, b, rows, cols);
                    //
                    //
                   
                }
                else if (data[i, b] == 1)
                {
                    PlaceGridObj(block, "Generated Block", i, b);
                    var temp = new RailsPlacer.PlacedRail();
                    temp.isBlock = true;
                    temp.ClickX = i;
                    temp.ClickZ = b;
                    wallsPlaced.Add(temp);
                }
                else if (data[i, b] == 0)
                {
                    PlaceGridObj(ground, "Generated Ground", i, b);
                }
            }
        }
        var path = dataGenerator.GetPath(data, new Vector2Int(0, 3), new Vector2Int(0, 3), new Vector2Int((int)railsPlacer.endRail.ClickX, (int)railsPlacer.endRail.ClickZ), new List<Vector2Int> { });
        path.Item1.RemoveAt(path.Item1.Count - 1);
        path.Item1.RemoveAt(0);
        path.Item1.ForEach(p =>
        {
            //PlaceGridObj(Coin[Random.Range(0, Coin.Count - 1)], "Bonus", p.x, p.y, 1);
        });
    }

    private void PlaceGridObj(GameObject obj, string name, int i, int b)
    {
        float num = 0f;
        GameObject temp = Instantiate(obj, new Vector3(i, num, b), Quaternion.identity);
        temp.tag = name;
    }
    private void PlaceGridObj(GameObject obj, string name, int i, int b, int y)
    {
        GameObject temp = Instantiate(obj, new Vector3(i, y, b), Quaternion.identity);
        temp.tag = name;
    }

    private void PlaceGridObj(GameObject obj, string name, int i, int b, int rows, int cols)
    {
        GameObject temp;
        GameObject temp2;
        // if (i == rows - 1)
        // {
        //     temp = Instantiate(WallGround, new Vector3(i, 1, b), Quaternion.identity);
        //     temp2 = Instantiate(obj, new Vector3(i, 0, b), Quaternion.identity);
        //     temp2.tag = name;
        // }
        // if
        temp = Instantiate(obj, new Vector3(i, 0, b), Quaternion.identity);
        temp.tag = name;
    }



    public void DisposeOldMaze()
    {
        wallsPlaced.Clear();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated Block");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
        objects = GameObject.FindGameObjectsWithTag("Drill");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
        objects = GameObject.FindGameObjectsWithTag("Bonus");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
        objects = GameObject.FindGameObjectsWithTag("train");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
        objects = GameObject.FindGameObjectsWithTag("Generated Ground");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
        objects = GameObject.FindGameObjectsWithTag("Rail");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
        objects = GameObject.FindGameObjectsWithTag("Finish Pos");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
        objects = GameObject.FindGameObjectsWithTag("Start Pos");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
        objects = GameObject.FindGameObjectsWithTag("Wall Block");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
        objects = GameObject.FindGameObjectsWithTag("PointMarker");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }

    }

    private void FindStartPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    startRow = i;
                    startCol = j;
                    return;
                }
            }
        }
    }

    private void FindGoalPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        // loop top to bottom, right to left
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = cMax; j >= 0; j--)
            {
                if (maze[i, j] == 0)
                {
                    goalRow = i;
                    goalCol = j;
                    return;
                }
            }
        }
    }

    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        go.name = "Start Trigger";
        //go.tag = "Generated";

        //go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }


    // Дебаг для генератора
    void OnGUI()
    {
        if (!showDebug)
        {
            return;
        }

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        // loop top to bottom, left to right
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "....";
                }
                else
                {
                    msg += "==";
                }
            }
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }
}
