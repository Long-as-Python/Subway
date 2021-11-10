using UnityEngine;

public static class Bezier
{
    public static Vector3 GetPoint(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, float t)
    {
        Vector3 pointAB = Vector3.Lerp(pointA, pointB, t);
        Vector3 pointBC = Vector3.Lerp(pointB, pointC, t);
        Vector3 pointCD = Vector3.Lerp(pointC, pointD, t);

        Vector3 pointABBC = Vector3.Lerp(pointAB, pointBC, t);
        Vector3 pointBCCD = Vector3.Lerp(pointBC, pointCD, t);
        
        Vector3 point = Vector3.Lerp(pointABBC, pointBCCD, t);

        return point;
    }
}
