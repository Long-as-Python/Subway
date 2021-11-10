using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Random;

public class CarController : MonoBehaviour
{
    public Transform path;
    public List<Transform> nodes;
    private float maxSteerAngle = 40f;
    public float Speed = 0.01f;
    public int currentNode = 0;
    public bool isInQueue = false;

    private void Start()
    {
        //nodes = new List<Transform>();
        //nodes = path.GetComponentsInChildren<Transform>().ToList();
    }
    private void FixedUpdate()
    {
        //ApplySteer();
        Drive();
        CheckWaypointDistance();
        //transform.LookAt(nodes[currentNode]);
        var diretion = nodes[currentNode != nodes.Count - 1 ? currentNode + 1 : currentNode].transform.position - transform.position;
        var rotation = Quaternion.LookRotation(diretion);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 6f);
    }

    public bool check = true;

    void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].TransformPoint(Vector3.zero)) < 0.001f)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else currentNode++;
        }

    }

    private void Drive()
    {
        //if (!isInQueue)
        transform.position = Vector3.MoveTowards(transform.position, nodes[currentNode].transform.position, Speed);
    }
}
