using UnityEngine;
using System.Collections;
using System;

public class WaterController : MonoBehaviour
{
    public int speed;
    private Vector3 pos;
    public GameObject water;
    public GameObject startWater;

    private void Start()
    {
        speed = 0;
        pos = transform.position;
        count = 0;
        point = transform.position;
        PoisitionBeforeTurn = transform.position;
        temp = new Vector3(transform.position.x, 1, transform.position.z);
        GameManager.Instance.StartTrain.AddListener(StartTrain);
        Vector3 look = new Vector3(GameManager.Instance.railsPlacer.placedRails[count].ClickX, 1,
            GameManager.Instance.railsPlacer.placedRails[count].ClickZ);
        transform.LookAt(look);
    }

    public void Move(Vector3 dest)
    {
        dest.y = 1;
        //pos = dest;
        //Debug.Log("Moving Tovards dest");
    }

    void StartTrain()
    {
        speed = 3;
    }

    int count;
    float turnCounter = 0.0f;
    bool isTurn = false;
    Vector3 point;
    bool stop = false;
    private Coroutine LookCoroutine;
    Vector3 temp;
    Vector3 PoisitionBeforeTurn;

    void Update()
    {
        if (Vector3.Distance(transform.position, pos) < 0.001f && !stop)
        {
            if (GameManager.Instance.railsPlacer.placedRails[count].pointMarker != null)
                Destroy(GameManager.Instance.railsPlacer.placedRails[count].pointMarker);
            if (GameManager.Instance.railsPlacer.placedRails[count].ClickX ==
                GameManager.Instance.railsPlacer.endRail.height - 1 &&
                GameManager.Instance.railsPlacer.placedRails[count].ClickZ ==
                GameManager.Instance.railsPlacer.endRail.width)
            {
                PoisitionBeforeTurn = transform.position;
                temp = new Vector3(GameManager.Instance.railsPlacer.placedRails[count].ClickX, transform.position.y,
                    GameManager.Instance.railsPlacer.placedRails[count].ClickZ);
                Vector3 goingTo = new Vector3(GameManager.Instance.railsPlacer.endRail.ClickX, 1,
                    GameManager.Instance.railsPlacer.endRail.ClickZ);
                pos = goingTo;
                point = peekingPoint(goingTo,
                    new Vector3(GameManager.Instance.railsPlacer.endRail.height, 1,
                        GameManager.Instance.railsPlacer.endRail.width));

                turnCounter = 0.0f;
                count = 0;
                StartCoroutine(Delay());
                stop = true;
                // if (LookCoroutine != null)
                //     StopCoroutine(LookCoroutine);
                // LookCoroutine = StartCoroutine(LookAt(temp));
            }
            else if (count < GameManager.Instance.railsPlacer.placedRails.Count - 1)
            {
                PoisitionBeforeTurn = transform.position;
                temp = new Vector3(GameManager.Instance.railsPlacer.placedRails[count].ClickX, transform.position.y,
                    GameManager.Instance.railsPlacer.placedRails[count].ClickZ);
                CheckForPlacedBlock();
                count++;

                Vector3 goingTo = new Vector3(GameManager.Instance.railsPlacer.placedRails[count].height, 1,
                    GameManager.Instance.railsPlacer.placedRails[count].width);
                pos = goingTo;
                point = peekingPoint(goingTo);
                turnCounter = 0.0f;
                // if (LookCoroutine != null)
                //     StopCoroutine(LookCoroutine);
                // StartCoroutine(LookAt(temp));
            }
        }

        else if (stop)
        {
            Vector3 goingTo = new Vector3(GameManager.Instance.railsPlacer.endRail.height + 6, 1,
                GameManager.Instance.railsPlacer.endRail.width);
            pos = goingTo;
        }

        if (turnCounter < 1.0f && isTurn)
        {
            turnCounter += 3.0f * Time.deltaTime;
            transform.position = GetPointByLerp(PoisitionBeforeTurn,
                PoisitionBeforeTurn + (temp - PoisitionBeforeTurn) * 0.4f, temp + (pos - temp) * 0.6f, pos,
                turnCounter);
            transform.rotation = Quaternion.LookRotation(GetFirstDerivate(PoisitionBeforeTurn,
                PoisitionBeforeTurn + (temp - PoisitionBeforeTurn) * 0.4f, temp + (pos - temp) * 0.6f, pos,
                turnCounter));
            //Vector3 dest = GetBezierPosition(transform.position, pos, point, turn);
        }
        else if (!isTurn)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, pos, step);
        }
    }

    void CheckForPlacedBlock()
    {
        Debug.Log("position " + pos);
        var nextPos = new Vector3(GameManager.Instance.railsPlacer.placedRails[count + 1].ClickX, transform.position.y,
            GameManager.Instance.railsPlacer.placedRails[count + 1].ClickZ);
        for (int i = 0; i < GameManager.Instance.railsPlacer.placedRails.Count; i++)
        {
            if (GameManager.Instance.railsPlacer.placedRails[i].ClickX == nextPos.x &&
                GameManager.Instance.railsPlacer.placedRails[i].ClickZ == nextPos.z &&
                GameManager.Instance.railsPlacer.placedRails[i].isBlock)
            {
                Debug.Log("We Lost bec of water");
                stop = true;
                GameManager.Instance.OnLoose.Invoke();
            }
        }
    }

    IEnumerator Delay()
    {
        DrillConeController controller =
            GameObject.FindGameObjectWithTag("WaterMill").GetComponent(typeof(DrillConeController)) as
                DrillConeController;
        controller.check = true;
        yield return new WaitForSeconds(1f);
        GameManager.Instance.OnWin.Invoke();
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
        if (!stop)
            if (GameManager.Instance.railsPlacer.placedRails[count - 1].height == goingTo.x ||
                GameManager.Instance.railsPlacer.placedRails[count - 1].width == goingTo.z)
            {
                Vector3 look = new Vector3(GameManager.Instance.railsPlacer.placedRails[count].ClickX, 1,
                    GameManager.Instance.railsPlacer.placedRails[count].ClickZ);
                transform.LookAt(look);
                isTurn = false;
                temppos = pos;
                PlaceWater();
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z &&
                     transform.position.z >
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.left * 0.2f;
                PlaceWater();
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z &&
                     transform.position.z <
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.right * 0.2f;
                PlaceWater();
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z &&
                     transform.position.z >
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.right * 0.2f;
                PlaceWater();
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z &&
                     transform.position.z <
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
            {
                isTurn = true;
                PlaceWater();
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.left * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z &&
                     transform.position.x >
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
            {
                isTurn = true;
                PlaceWater();
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.back * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z &&
                     transform.position.x <
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
            {
                isTurn = true;
                PlaceWater();
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.forward * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z &&
                     transform.position.x <
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.back * 0.2f;
                PlaceWater();
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z &&
                     transform.position.x >
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.forward * 0.2f;
                PlaceWater();
            }

        //Debug.Log(temppos);
        return temppos;
    }

    void PlaceWater()
    {
        if (pos.x > 0)
        {
            Instantiate(water, new Vector3(Convert.ToInt32(pos.x), 0.9f, Convert.ToInt32(pos.z)), Quaternion.identity);
        }
    }

    Vector3 peekingPoint(Vector3 goingTo, Vector3 EndRail)
    {
        Debug.Log("End Rail placed on");
        Vector3 temppos = transform.position;
        if (!stop)
            if (GameManager.Instance.railsPlacer.placedRails[count - 1].ClickX == goingTo.x ||
                GameManager.Instance.railsPlacer.placedRails[count - 1].ClickZ == goingTo.z)
            {
                transform.LookAt(goingTo);
                isTurn = false;
                temppos = pos;
                //meshDeformer.Append();
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z &&
                     transform.position.z >
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.left * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z &&
                     transform.position.z <
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.right * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z &&
                     transform.position.z >
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.right * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z &&
                     transform.position.z <
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.left * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z &&
                     transform.position.x >
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.back * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z &&
                     transform.position.x <
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.forward * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX > goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ < goingTo.z &&
                     transform.position.x <
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.back * 0.2f;
            }
            else if (GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX < goingTo.x &&
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickZ > goingTo.z &&
                     transform.position.x >
                     GameManager.Instance.railsPlacer.placedRails[count - 2].ClickX)
            {
                isTurn = true;
                temppos = transform.position + (goingTo - transform.position) / 2 + Vector3.forward * 0.2f;
            }

        return temppos;
    }
}