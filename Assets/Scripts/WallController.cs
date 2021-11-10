using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public List<GameObject> obstacles;
    void Start()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        GameObject temp = Instantiate(obstacles[Random.Range(0, obstacles.Count - 1)], pos, Quaternion.identity);
        temp.transform.SetParent(this.gameObject.transform);
    }
}
