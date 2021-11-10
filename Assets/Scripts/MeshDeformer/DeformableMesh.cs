using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GeneratePlaneMesh))]
public class DeformableMesh : MonoBehaviour
{
    public float maximumDepression;
    public List<Vector3> originalVertices;
    public List<Vector3> modifiedVertices;

    private GeneratePlaneMesh plane;

    void Start()
    {
        GameManager.Instance.CollideMesh.AddListener(AddDepression);
        MeshRegenerated();
    }
    public void MeshRegenerated()
    {
        plane = GetComponent<GeneratePlaneMesh>();
        plane.mesh.MarkDynamic();
        originalVertices = plane.mesh.vertices.ToList();
        modifiedVertices = plane.mesh.vertices.ToList();
        Debug.Log("Mesh Regenerated");
    }

    public void AddDepression(Vector3 depressionPoint, float radius)
    {
        Vector3 pos = transform.position - depressionPoint;
        var worldPos4 = this.transform.worldToLocalMatrix * pos;
        var worldPos = new Vector3(worldPos4.x * -1, worldPos4.y, worldPos4.z * -1);
        for (int i = 0; i < modifiedVertices.Count; ++i)
        {
            var distance = (worldPos - (modifiedVertices[i] + Vector3.down * maximumDepression)).magnitude;
            //врубать дебаг только в ерайних случаях - лагает жутко
            //Debug.Log(distance + " distance");
            if (distance < radius)
            {
                var newVert = originalVertices[i] + Vector3.down * maximumDepression;
                modifiedVertices.RemoveAt(i);
                modifiedVertices.Insert(i, newVert);
            }
        }

        plane.mesh.SetVertices(modifiedVertices);
        //Debug.Log("Mesh Depressed");
    }
}