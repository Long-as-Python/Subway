using System.ComponentModel;
using System.Security.Principal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RailsPlacer : MonoBehaviour
{
    public Grid grid;
    public List<PlacedRail> placedRails;
    public List<RailToPlace> railToPlace;
    public PlacedRail endRail;
    [SerializeField] private GameObject Rails;
    [SerializeField] private GameObject poinMarker;

    void Awake()
    {
        placedRails = new List<PlacedRail>();
        railToPlace = new List<RailToPlace>();
        grid = FindObjectOfType<Grid>();
    }

    void Start()
    {
        //GameManager.Instance.OnWin.AddListener(ClearRails);
    }

    // void ClearRails()
    // {
    //     StartCoroutine(Timer());
    // }

    // IEnumerator Timer()
    // {
    //     yield return new WaitForSeconds(2);
    // }

    void FixedUpdate()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 50f))
        {
            if (hit.collider.gameObject.TryGetComponent<Way>(out var way))
            {
                GameObject temp = hit.transform.gameObject;
                CheckPlacament(temp, hit);
            }
        }

        //}
    }

    private void CheckPlacament(GameObject temp, RaycastHit hit)
    {
        if (temp.tag != "Generated Block" && temp.tag != "Wall Block" && temp.tag != "Bonus" && temp.tag != "Drill" && temp.tag != "train")
        {
            var finalPosition = grid.GetNearestPointOnGrid(hit.point);
            if (finalPosition.x == temp.transform.position.x && finalPosition.z == temp.transform.position.z)
            {
                
                //Debug.Log(grid.GetNearestPointOnGrid(hit.point));
                if (Mathf.RoundToInt(finalPosition.x) == placedRails[placedRails.Count - 1].ClickX + 1 &&
                    Mathf.RoundToInt(finalPosition.z) == placedRails[placedRails.Count - 1].ClickZ)
                {
                    //Destroy(temp);
                    PlacedRail start = new PlacedRail();
                    start.ClickX = Mathf.RoundToInt(finalPosition.x);
                    start.ClickZ = Mathf.RoundToInt(finalPosition.z);
                    start.height = Mathf.RoundToInt(finalPosition.x) - 0.4f;
                    start.width = Mathf.RoundToInt(finalPosition.z);
                    PlaceRailNear(finalPosition, temp, start);
                }
                else if (Mathf.RoundToInt(finalPosition.z) == placedRails[placedRails.Count - 1].ClickZ + 1 &&
                         Mathf.RoundToInt(finalPosition.x) == placedRails[placedRails.Count - 1].ClickX)
                {
                    //Destroy(temp);
                    PlacedRail start = new PlacedRail();
                    start.ClickX = Mathf.RoundToInt(finalPosition.x);
                    start.ClickZ = Mathf.RoundToInt(finalPosition.z);
                    start.height = Mathf.RoundToInt(finalPosition.x);
                    start.width = Mathf.RoundToInt(finalPosition.z) - 0.4f;
                    PlaceRailNear(finalPosition, temp, start);
                }
                else if (Mathf.RoundToInt(finalPosition.x) == placedRails[placedRails.Count - 1].ClickX - 1 &&
                         Mathf.RoundToInt(finalPosition.z) == placedRails[placedRails.Count - 1].ClickZ)
                {
                    //Destroy(temp);
                    PlacedRail start = new PlacedRail();
                    start.ClickX = Mathf.RoundToInt(finalPosition.x);
                    start.ClickZ = Mathf.RoundToInt(finalPosition.z);
                    start.height = Mathf.RoundToInt(finalPosition.x) + 0.4f;
                    start.width = Mathf.RoundToInt(finalPosition.z);
                    PlaceRailNear(finalPosition, temp, start);
                }
                else if (Mathf.RoundToInt(finalPosition.z) == placedRails[placedRails.Count - 1].ClickZ - 1 &&
                         Mathf.RoundToInt(finalPosition.x) == placedRails[placedRails.Count - 1].ClickX)
                {
                    //Destroy(temp);
                    PlacedRail start = new PlacedRail();
                    start.ClickX = Mathf.RoundToInt(finalPosition.x);
                    start.ClickZ = Mathf.RoundToInt(finalPosition.z);
                    start.height = Mathf.RoundToInt(finalPosition.x);
                    start.width = Mathf.RoundToInt(finalPosition.z) + 0.4f;
                    PlaceRailNear(finalPosition, temp, start);
                }
            }
        }
    }

    // void CreatingCoords(Vector3 finalPosition,GameObject temp)
    // {
    //     PlacedRail start = new PlacedRail();
    //     start.ClickX = Mathf.RoundToInt(finalPosition.x);
    //     start.ClickZ = Mathf.RoundToInt(finalPosition.z);
    //     start.height = Mathf.RoundToInt(finalPosition.x);
    //     start.width = Mathf.RoundToInt(finalPosition.z) + 0.4f;
    //     PlaceRailNear(finalPosition, temp, start);
    // }

    public void FingerUp()
    {
        Debug.Log("MouseUp");
        foreach (RailToPlace element in railToPlace)
        {
            PlaceRailNear(element.pos, element.temp, element.start);
        }

        railToPlace.Clear();
    }

    private void PlaceRailNear(Vector3 pos, GameObject temp, PlacedRail start)
    {
        if (placedRails.Count > 1 && placedRails[placedRails.Count - 2].ClickX == start.ClickX &&
            placedRails[placedRails.Count - 2].ClickZ == start.ClickZ)
        {
        }
        else
        {
            GameObject point = Instantiate(poinMarker, pos, Quaternion.identity);
            point.tag = "PointMarker";
            start.pointMarker = point;
            placedRails.Add(start);
            temp.tag = "Rail";
            Debug.Log("Placed Rails");
            //if (DrillController.Instance.Block == Vector3.zero && placedRails[placedRails.Count - 2].ClickX != start.ClickX && placedRails[placedRails.Count - 2].ClickZ != start.ClickZ)
            //{
            ////if (DrillController.Instance.Block == Vector3.zero)
            ////    DrillController.Instance.Block = pos;
            //}
        }
    }

    [System.Serializable]
    public class PlacedRail
    {
        public float height;
        public float width;
        public float ClickX;
        public float ClickZ;
        public GameObject pointMarker;
        public GameObject placedRail;
        public bool isBlock = false;
    }

    public class RailToPlace
    {
        public Vector3 pos;
        public GameObject temp;
        public PlacedRail start;
    }
}