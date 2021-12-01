using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillConeController : MonoBehaviour
{
    public float speed = 20;
    public GameObject temp;
    void Update()
    {
        if (check)
        {
            temp.SetActive(true);
            transform.Rotate(Vector3.right, -1 * speed * Time.deltaTime);
        }
    }

    public bool check = false;
}
