using UnityEngine;

public class BasicBlast : Blast
{
    private float _Radius;
    private float _RadiusOffset;

    public BasicBlast(float radius, float radiusOffset)
    {
        _Radius = radius;
        _RadiusOffset = radiusOffset;
    }

    public override void Trigger(Collision collision)
    {
        Vox3D.ChunkCollisionHandler.Instance().CollisionSphere(collision.contacts[0].point, _Radius, _RadiusOffset);
    }
}
