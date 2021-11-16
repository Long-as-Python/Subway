using System.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Battlehub.MeshDeformer2;
using TMPro;
//using Facebook.Unity;


[System.Serializable]
public class CollideEvent : UnityEvent<Vector3, float> { }
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI LevelNum;
    MapGenerator generator;
    public static GameManager Instance { private set; get; }
    private bool goalReached;
    [SerializeField] private int length;
    [SerializeField] private int height;
    public CollideEvent CollideMesh;
    public UnityEvent OnWin;
    public UnityEvent OnStart;
    public UnityEvent StartTrain;
    public RailsPlacer railsPlacer;
    public GameObject drill;
    public GameObject train;
    public MeshDeformer meshDeformer;
    private int levelnum = 1;

    private void InitCallback()
    {
        // if (FB.IsInitialized)
        // {
        //     // Signal an app activation App Event
        //     FB.ActivateApp();
        //     // Continue with Facebook SDK
        //     // ...
        // }
        // else
        // {
        //     Debug.Log("Failed to Initialize the Facebook SDK");
        // }
    }
    
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;

        }
    }

    void Awake()
    {
        // if (!FB.IsInitialized)
        // {
        //     // Initialize the Facebook SDK
        //     FB.Init(InitCallback, OnHideUnity);
        // }
        // else
        // {
        //     // Already initialized, signal an app activation App Event
        //     FB.ActivateApp();
        // }
        if (CollideMesh == null)
            CollideMesh = new CollideEvent();
        if (OnStart == null)
            OnStart = new UnityEvent();
        if (Instance == null)
            Instance = this;
        generator = GetComponent<MapGenerator>();
        railsPlacer = FindObjectOfType<RailsPlacer>();
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        StartNewGame();
        StartNewGame();
        OnWin.AddListener(Win);
        meshDeformer = FindObjectOfType<MeshDeformer>();
        //meshDeformer.Append();
    }

    public void StartNewGame()
    {
        StartNewMaze();
    }

    public void StartNewGame(int i)
    {
        StartNewMaze(i);
    }

    private void StartNewMaze()
    {
        OnStart.Invoke();
        generator.GenerateNewMaze(length, height, OnStartTrigger, OnGoalTrigger);
        Instantiate(drill, new Vector3(-0.4f, 1, 3), Quaternion.identity);
        Instantiate(train, new Vector3(-1.4f, 1, 3), Quaternion.identity);
        goalReached = false;
    }

    private void StartNewMaze(int i)
    {
        generator.GenerateNewMaze(length, height);
        Instantiate(drill, new Vector3(-0.4f, 1, 3), Quaternion.identity);
        Instantiate(train, new Vector3(-1.4f, 1, 3), Quaternion.identity);
        goalReached = false;
    }

    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("Goal!");
        goalReached = true;
        Destroy(trigger);
    }

    private void OnStartTrigger(GameObject trigger, GameObject other)
    {
        if (goalReached)
        {
            Debug.Log("Finish!");
            Invoke("StartNewMaze", 4);
        }
    }

    public void Win()
    {
        Debug.Log("Win");
        levelnum++;
        LevelNum.text = "Level " + levelnum;
        StartCoroutine(Timer(2));
        Debug.ClearDeveloperConsole();
    }

    public void Win(int time)
    {
        Debug.Log("Win");
        StartCoroutine(Timer(time));
        Debug.ClearDeveloperConsole();
    }

    public void Restart()
    {
        Debug.Log("Win");
        StartCoroutine(Timer());
        Debug.ClearDeveloperConsole();
    }

    IEnumerator Timer(int time)
    {
        yield return new WaitForSeconds(time);
        CollideMesh.RemoveAllListeners();
        GameObject drill = GameObject.FindGameObjectWithTag("Drill");
        Destroy(drill);
        railsPlacer.placedRails.Clear();
        StartNewGame();
        GameObject temp = GameObject.FindGameObjectWithTag("Start Pos");
        RailsPlacer.PlacedRail start = new RailsPlacer.PlacedRail();
        start.height = Mathf.RoundToInt(temp.transform.position.x);
        start.width = Mathf.RoundToInt(temp.transform.position.z);
        start.ClickX = Mathf.RoundToInt(temp.transform.position.x);
        start.ClickZ = Mathf.RoundToInt(temp.transform.position.z);
        railsPlacer.placedRails.Add(start);
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(0);
        CollideMesh.RemoveAllListeners();
        GameObject drill = GameObject.FindGameObjectWithTag("Drill");
        Destroy(drill);
        railsPlacer.placedRails.Clear();
        StartNewGame(1);
        GameObject temp = GameObject.FindGameObjectWithTag("Start Pos");
        RailsPlacer.PlacedRail start = new RailsPlacer.PlacedRail();
        start.height = Mathf.RoundToInt(temp.transform.position.x);
        start.width = Mathf.RoundToInt(temp.transform.position.z);
        start.ClickX = Mathf.RoundToInt(temp.transform.position.x);
        start.ClickZ = Mathf.RoundToInt(temp.transform.position.z);
        railsPlacer.placedRails.Add(start);
    }
}
