using UnityEngine;

namespace Game.Weapons
{
    [System.Serializable]
    public class Blast
    {
        public float Radius;
        public float RadiusOffset;

        public bool Scatter;
        public bool ScatterOnImpact;
        public float ScatterAngle;
        public float ScatterAmount;
        public string ScatterBehaviour;

        public string Child;

        public Blast() { }
        public Blast(float radius, float radiusOffset, bool scatter, bool scatterOnImpact, float scatterAngle, float scatterAmount, string scatterBehaviour, string child)
        {
            Radius = radius;
            RadiusOffset = radiusOffset;
            Scatter = scatter;
            ScatterOnImpact = scatterOnImpact;
            ScatterAngle = scatterAngle;
            ScatterAmount = scatterAmount;
            ScatterBehaviour = scatterBehaviour;
            Child = child;
        }

        public void Trigger(Collision collision)
        {
            //TODO change logic to spawn additional projectiles when blast is of scatter type
            Vox3D.Engine.ChunkCollisionHandler.Instance().CollisionSphere(collision.contacts[0].point, Radius, RadiusOffset);
        }
    }

}