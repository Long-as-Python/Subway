using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillConeController : MonoBehaviour
{
    public float speed = 20;
    void Update()
    {
        transform.Rotate(Vector3.right, speed * Time.deltaTime);
    }
}
