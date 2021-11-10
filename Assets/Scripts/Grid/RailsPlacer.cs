using System.ComponentModel;
using System.Security.Principal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class RailsPlacer : MonoBehaviour
{
    private Grid grid;
    public List<PlacedRail> placedRails;
    public PlacedRail endRail;
    [SerializeField] private GameObject Rails;
    [SerializeField] private GameObject poinMarker;
    void Awake()
    {
        placedRails = new List<PlacedRail>();
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
        if (temp.tag != "Rail" && temp.tag != "Generated Block" && temp.tag != "Wall Block" && temp.tag != "Bonus" && temp.tag != "Drill" && temp.tag != "train")
        {
            var finalPosition = grid.GetNearestPointOnGrid(hit.point);
            if (finalPosition.x == temp.transform.position.x && finalPosition.z == temp.transform.position.z)
            {
                //Debug.Log(grid.GetNearestPointOnGrid(hit.point));
                if (Mathf.RoundToInt(finalPosition.x) == placedRails[placedRails.Count - 1].ClickX + 1 && Mathf.RoundToInt(finalPosition.z) == placedRails[placedRails.Count - 1].ClickZ)
                {
                    //Destroy(temp);
                    PlacedRail start = new PlacedRail();
                    start.ClickX = Mathf.RoundToInt(finalPosition.x);
                    start.ClickZ = Mathf.RoundToInt(finalPosition.z);
                    start.height = Mathf.RoundToInt(finalPosition.x) - 0.4f;
                    start.width = Mathf.RoundToInt(finalPosition.z);
                    PlaceRailNear(finalPosition, temp, start);

                }
                else if (Mathf.RoundToInt(finalPosition.z) == placedRails[placedRails.Count - 1].ClickZ + 1 && Mathf.RoundToInt(finalPosition.x) == placedRails[placedRails.Count - 1].ClickX)
                {
                    //Destroy(temp);
                    PlacedRail start = new PlacedRail();
                    start.ClickX = Mathf.RoundToInt(finalPosition.x);
                    start.ClickZ = Mathf.RoundToInt(finalPosition.z);
                    start.height = Mathf.RoundToInt(finalPosition.x);
                    start.width = Mathf.RoundToInt(finalPosition.z) - 0.4f;
                    PlaceRailNear(finalPosition, temp, start);
                }
                else if (Mathf.RoundToInt(finalPosition.x) == placedRails[placedRails.Count - 1].ClickX - 1 && Mathf.RoundToInt(finalPosition.z) == placedRails[placedRails.Count - 1].ClickZ)
                {
                    //Destroy(temp);
                    PlacedRail start = new PlacedRail();
                    start.ClickX = Mathf.RoundToInt(finalPosition.x);
                    start.ClickZ = Mathf.RoundToInt(finalPosition.z);
                    start.height = Mathf.RoundToInt(finalPosition.x) + 0.4f;
                    start.width = Mathf.RoundToInt(finalPosition.z);
                    PlaceRailNear(finalPosition, temp, start);
                }
                else if (Mathf.RoundToInt(finalPosition.z) == placedRails[placedRails.Count - 1].ClickZ - 1 && Mathf.RoundToInt(finalPosition.x) == placedRails[placedRails.Count - 1].ClickX)
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


    private void PlaceRailNear(Vector3 pos, GameObject temp, PlacedRail start)
    {
        GameObject point = Instantiate(poinMarker, pos, Quaternion.identity);
        point.tag = "PointMarker";
        start.PointMarker = point;
        placedRails.Add(start);
        temp.tag = "Rail";
        Debug.Log("Placed Rails");
    }

    public class PlacedRail
    {
        public float height;
        public float width;
        public float ClickX;
        public float ClickZ;
        public GameObject PointMarker;
    }
}
