using UnityEngine;

public class Blast : MonoBehaviour
{
    public class BlastProperties
    {
        public float radius;
    }

    [SerializeField]
    private BlastProperties _Properties;
    public BlastProperties Properties { get => _Properties; set => _Properties = value; }

    // Start is called before the first frame update
    void Start()
    {
        _Properties.radius = 10;
    }
    public void Trigger(Collision collision)
    {
        float offset = Vox3D.Vox3DManager.Instance().Properties.VoxelSize;
        Vox3D.ChunkCollisionHandler.Instance().CollisionSphere(collision.contacts[0].point, Properties.radius, offset);
    }

}
