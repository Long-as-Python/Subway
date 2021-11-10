using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.MeshDeformer2;

public class RailPlacament
{
    public GameObject RaillOffset(Vector3 p1, Vector3 p2, Vector3 p3, List<ControlPoint> controlPoints)
    {
        List<GameObject> transformList = ControlPointToTransform(controlPoints);
        Debug.Log("PlacingRailCurve" + transformList[0].transform.localPosition);
        transformList[0].transform.position = p3;
        transformList[1].transform.position = p2;
        transformList[2].transform.position = p1;
        Debug.Log("PlacedRailCurve" + transformList[0].transform.localPosition);
        return transformList[0];
    }

    private List<GameObject> ControlPointToTransform(List<ControlPoint> PointsToMove)
    {
        List<GameObject> temp = new List<GameObject>();
        temp.Add(PointsToMove[PointsToMove.Count - 3].gameObject);
        temp.Add(PointsToMove[PointsToMove.Count - 2].gameObject);
        temp.Add(PointsToMove[PointsToMove.Count - 1].gameObject);
        foreach (GameObject obj in temp)
        {
            obj.SetActive(true);
        }
        return temp;
    }

    IEnumerator check(Transform trans)
    {
        yield return new WaitForSeconds(1);
        Debug.Log("PlacedRailCurveCheck" + trans.localPosition);

    }
}