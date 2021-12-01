using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.MeshDeformer2;

public class DrillController : MonoBehaviour
{
    public float speed = 2;
    private Vector3 pos;
    public DrillController Instance { set; get; }
    public List<GameObject> RailPoints;
    public GameObject ForwardRail;
    public GameObject TurnRail;
    //public MeshDeformer meshDeformer;

    private void Start()
    {
        //meshDeformer = FindObjectOfType<MeshDeformer>();
        if (Instance == null)
            Instance = this;
        pos = transform.position;
        count = 0;
        point = transform.position;
        PoisitionBeforeTurn = transform.position;
        temp = new Vector3(transform.position.x, 1, transform.position.z);
        Vector3 look = new Vector3(GameManager.Instance.railsPlacer.placedRails[count].ClickX, 1, GameManager.Instance.railsPlacer.placedRails[count].ClickZ);
        transform.LookAt(look);
    }

    public void Move(Vector3 dest)
    {
        dest.y = 1;
        //pos = dest;
        Debug.Log("Moving Tovards dest");
    }

    int count;
    float turnCounter = 0.0f;
    bool isTurn = false;
    Vector3 point;
    bool stop = false;
    private Coroutine LookCoroutine;
    Vector3 temp;
    bool railPlacedCheck = false;
    Vector3 PoisitionBeforeTurn;
    bool fingerUp = false;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                fingerUp = true;
            }
            else
                fingerUp = false;
        }
        if (fingerUp || Input.GetKey(KeyCode.P))
        {
            if (Vector3.Distance(transform.position, pos) < 0.001f && !stop)
            {
                if (GameManager.Instance.railsPlacer.placedRails[count].PointMarker != null)
                    Destroy(GameManager.Instance.railsPlacer.placedRails[count].PointMarker);
                if (GameManager.Instance.railsPlacer.placedRails[count].ClickX == GameManager.Instance.railsPlacer.endRail.height - 1 && GameManager.Instance.railsPlacer.placedRails[count].ClickZ == GameManager.Instance.railsPlacer.endRail.width)
                {
                    StartCoroutine(ToFinalPos());
                    PoisitionBeforeTurn = transform.position;
                    temp = new Vector3(GameManager.Instance.railsPlacer.placedRails[count].ClickX, transform.position.y, GameManager.Instance.railsPlacer.placedRails[count].ClickZ);
                    Vector3 goingTo = new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1, GameManager.Instance.railsPlacer.endRail.ClickZ);
                    Debug.Log(goingTo);
                    pos = goingTo;
                    point = peekingPoint(goingTo, new Vector3(GameManager.Instance.railsPlacer.endRail.height, 1, GameManager.Instance.railsPlacer.endRail.width));
                    turnCounter = 0.0f;
                    GameManager.Instance.StartTrain.Invoke();
                    stop = true;
                }
                else if (count < GameManager.Instance.railsPlacer.placedRails.Count - 1)
                {
                    railPlacedCheck = false;
                    PoisitionBeforeTurn = transform.position;
                    temp = new Vector3(GameManager.Instance.railsPlacer.placedRails[count].ClickX, transform.position.y, GameManager.Instance.railsPlacer.placedRails[count].ClickZ);
                    count++;
                    Vector3 goingTo = new Vector3(GameManager.Instance.railsPlacer.placedRails[count].height, 1, GameManager.Instance.railsPlacer.placedRails[count].width);
                    pos = goingTo;
                    point = peekingPoint(goingTo);
                    turnCounter = 0.0f;
                }
            }

            if (turnCounter < 1.0f && isTurn)
            {
                PlaceRail(pos, temp + (pos - temp) * 0.6f, PoisitionBeforeTurn + (temp - PoisitionBeforeTurn) * 0.4f);
                turnCounter += 2.0f * Time.deltaTime;
                transform.position = GetPointByLerp(PoisitionBeforeTurn, PoisitionBeforeTurn + (temp - PoisitionBeforeTurn) * 0.4f, temp + (pos - temp) * 0.6f, pos, turnCounter);
                transform.rotation = Quaternion.LookRotation(GetFirstDerivate(PoisitionBeforeTurn, PoisitionBeforeTurn + (temp - PoisitionBeforeTurn) * 0.4f, temp + (pos - temp) * 0.6f, pos, turnCounter));

            }
            else if (!isTurn)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, pos, step);
            }
        }
    }

    IEnumerator check(Transform trans)
    {
        yield return new WaitForSeconds(1);
        Debug.Log("PlacedRailCurveCheck" + trans.localPosition + trans.gameObject.name);

    }

    IEnumerator ToFinalPos()
    {
        yield return new WaitForSeconds(1);
        isTurn = false;
        Vector3 goingTo = new Vector3(transform.position.x + 3, 1, GameManager.Instance.railsPlacer.endRail.ClickZ);
        pos = goingTo;
        transform.LookAt(goingTo);
    }

    public static Vector3 GetPointByLerp(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 p01 = Vector3.Lerp(p0, p1, t);
        Vector3 p12 = Vector3.Lerp(p1, p2, t);
        Vector3 p23 = Vector3.Lerp(p2, p3, t);

        Vector3 p012 = Vector3.Lerp(p01, p12, t);
        Vector3 p112 = Vector3.Lerp(p1, p23, t);

        Vector3 p0123 = Vector3.Lerp(p012, p112, t);

        return p0123;
    }

    public void PlaceRail(Vector3 p3, Vector3 p2, Vector3 p1)
    {
        if (!railPlacedCheck)
        {
            //meshDeformer.Append();
            RailPlacament rp = new RailPlacament();
            //List<ControlPoint> temp = meshDeformer.GetComponentsInChildren<ControlPoint>(true).ToList();
            //GameObject checkTrans = rp.RaillOffset(p1, p2, p3, temp);
            //StartCoroutine(check(checkTrans.transform));
            railPlacedCheck = true;
        }
    }

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            Mathf.Pow(oneMinusT, 3) * p0 +
            3f * Mathf.Pow(oneMinusT, 2) * t * p1 +
            3f * oneMinusT * Mathf.Pow(t, 2) * p2 +
            Mathf.Pow(t, 3) * p3;
    }

    public static Vector3 GetFirstDerivate(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }


    Vector3 peekingPoint(Vector3 goingTo)
    {
        Vector3 temppos = transform.position;
        if (GameManager.Instance.railsPlacer.placedRails[count - 1].height == goingTo.x || GameManager.Instance.railsPlacer.placedRails[count - 1].width == goingTo.z)
        {
            transform.LookAt(goingTo);
            isTurn = false;
            temppos = pos;
            GameObject tempRail = Instantiate(ForwardRail, new Vector3(temp.x, 0.525f, temp.z), Quaternion.identity);
            tempRail.tag = "Rail";
            if (count - 2 > 0)
                tempRail.transform.LookAt(new Vector3(GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX, 0.525f, GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ));
            else
                tempRail.transform.LookAt(new Vector3(-2, 0.525f, 3));
            //meshDeformer.Append();
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z && transform.position.z > GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
        {
            isTurn = true;
            SpawnTurn(180 + 45);//+
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.left * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z && transform.position.z < GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
        {
            isTurn = true;
            SpawnTurn(45);
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.right * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z && transform.position.z > GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
        {
            isTurn = true;
            SpawnTurn(315);
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.right * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z && transform.position.z < GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
        {
            SpawnTurn(90 + 45);
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.left * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z && transform.position.x > GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
        {
            SpawnTurn(45);//
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.back * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z && transform.position.x < GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
        {
            SpawnTurn(225);
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.forward * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z && transform.position.x < GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
        {
            SpawnTurn(135);
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.back * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z && transform.position.x > GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
        {
            SpawnTurn(315);
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.forward * 0.2f;
        }

        return temppos;
    }

    Vector3 peekingPoint(Vector3 goingTo, Vector3 EndRail)
    {
        Debug.Log("End Rail placed on");
        Vector3 temppos = transform.position;
        if (GameManager.Instance.railsPlacer.placedRails[count - 1].ClickX == goingTo.x || GameManager.Instance.railsPlacer.placedRails[count - 1].ClickZ == goingTo.z)
        {
            transform.LookAt(goingTo);
            isTurn = false;
            temppos = pos;
            GameObject tempRail = Instantiate(ForwardRail, new Vector3(temp.x, 0.525f, temp.z), Quaternion.identity);
            tempRail.tag = "Rail";
            if (count - 2 > 0)
                tempRail.transform.LookAt(new Vector3(GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX, 0.525f, GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ));
            else
                tempRail.transform.LookAt(new Vector3(-2, 0.525f, 3));
            //meshDeformer.Append();
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z && transform.position.z > GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
        {
            isTurn = true;
            SpawnTurn(180 + 45, new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1, GameManager.Instance.railsPlacer.endRail.ClickZ));//+
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.left * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z && transform.position.z < GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
        {
            isTurn = true;
            SpawnTurn(45, new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1, GameManager.Instance.railsPlacer.endRail.ClickZ));
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.right * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z && transform.position.z > GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
        {
            isTurn = true;
            SpawnTurn(315, new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1, GameManager.Instance.railsPlacer.endRail.ClickZ));
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.right * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z && transform.position.z < GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
        {
            SpawnTurn(90 + 45, new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1, GameManager.Instance.railsPlacer.endRail.ClickZ));
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.left * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z && transform.position.x > GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
        {
            SpawnTurn(45, new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1, GameManager.Instance.railsPlacer.endRail.ClickZ));//
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.back * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z && transform.position.x < GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
        {
            SpawnTurn(225, new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1, GameManager.Instance.railsPlacer.endRail.ClickZ));
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.forward * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z && transform.position.x < GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
        {
            SpawnTurn(135, new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1, GameManager.Instance.railsPlacer.endRail.ClickZ));
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.back * 0.2f;
        }
        else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x && GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z && transform.position.x > GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
        {
            SpawnTurn(315, new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1, GameManager.Instance.railsPlacer.endRail.ClickZ));
            isTurn = true;
            temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.forward * 0.2f;
        }

        return temppos;
    }

    void SpawnTurn(float angle)
    {
        GameObject tempRail = Instantiate(TurnRail, new Vector3(temp.x, 0.536f, temp.z), Quaternion.AngleAxis(angle, Vector3.up));
        tempRail.tag = "Rail";
    }

    void SpawnTurn(float angle, Vector3 positionEnd)
    {
        GameObject tempRail = Instantiate(TurnRail, new Vector3(positionEnd.x - 1, 0.536f, positionEnd.z), Quaternion.AngleAxis(angle, Vector3.up));
        tempRail.tag = "Rail";
    }

    public void RestPos()
    {
        pos = new Vector3(0, 1, 3);
    }
}
