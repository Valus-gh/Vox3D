using UnityEngine;

using Game.Stages;
using Game.Resources;
using System.Collections.Generic;

namespace Game.Weapons
{
    [System.Serializable]
    public class Blast
    {
        public float    Radius;
        public float    RadiusOffset;

        public float    Damage;

        public bool     Scatter;
        public bool     ScatterOnImpact;
        public float    ScatterAngle;
        public float    ScatterAmount;
        public string   ScatterBehaviour;

        public string Child;

        public Blast() { }

        public Blast(
            float radius, 
            float radiusOffset, 
            float damage, 
            bool scatter, 
            bool scatterOnImpact, 
            float scatterAngle, 
            float scatterAmount, 
            string scatterBehaviour, 
            string child)
        {
            Radius = radius;
            RadiusOffset = radiusOffset;
            Damage = damage;
            Scatter = scatter;
            ScatterOnImpact = scatterOnImpact;
            ScatterAngle = scatterAngle;
            ScatterAmount = scatterAmount;
            ScatterBehaviour = scatterBehaviour;
            Child = child;
        }

        public void Trigger(Collision collision, uint ownerID)
        {
            //TODO change logic to spawn additional projectiles when blast is of scatter type

            if (Scatter)
            {
                DoScatter(collision, ownerID);
                return;
            }

            // Only check for collisions if the impact does NOT spawn any additional projectiles
            var particleSystemParent = GameObject.FindObjectOfType<ShootingStage>().ExplosionParticles;

            var particleSystems = particleSystemParent.GetComponentsInChildren<ParticleSystem>();

            Object.Instantiate(particleSystemParent, collision.contacts[0].point, Quaternion.identity, null);

            foreach (var system in particleSystems)
                system.Play();

            // Test terrain for collisions. Client side.
            Vox3D.Engine.ChunkDestructionHandler.Instance().CollisionSphere_Destroy(collision.contacts[0].point, Radius, RadiusOffset);

            // Test towers for collisions. Server-side.
            Object.FindObjectOfType<ShootingStage>().CmdTestTowerCollision(collision.contacts[0].point, Radius, Damage);
        }
        private void DoScatter(Collision collision, uint ownerID)
        {
            List<Trajectory> trajectories = new List<Trajectory>();

            float angleStep = 360f / ScatterAmount;

            for (int i = 0; i < ScatterAmount; i++)
            {
                float sphereSlice = angleStep * i * Mathf.Deg2Rad;
                Vector3 horizontalDir = new Vector3(Mathf.Cos(sphereSlice), 0f, Mathf.Sin(sphereSlice)).normalized;

                Trajectory trajectory = new Trajectory();
                trajectory.Angle = ScatterAngle;
                trajectory.DirectionXZ = horizontalDir;

                trajectories[i] = trajectory;
            }

            Object.FindObjectOfType<ShootingStage>().CmdScatterProjectiles(Child, trajectories, collision.contacts[0].point, ownerID);
        }
    }

}