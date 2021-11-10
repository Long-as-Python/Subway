using UnityEngine;

public class PhysicsDeformer : MonoBehaviour
{

    public float collisionRadius;
    public DeformableMesh deformableMesh;

    void Start()
    {
        deformableMesh = GetComponentInParent<DeformableMesh>();
    }

    void OnTriggerStay(Collider collision)
    {
        //foreach (var contact in collision.contacts)
        // {
        //pos = pos * -1;
        GameManager.Instance.CollideMesh.Invoke(collision.transform.position, collisionRadius);
        //deformableMesh.AddDepression(pos, collisionRadius);
        //}
    }
}