using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vox3D {

    public class ChunkCollisionHandler
    {
        private static ChunkCollisionHandler _Instance;
        public static ChunkCollisionHandler Instance()
        {
            if (_Instance is null)
                _Instance = new ChunkCollisionHandler();

            return _Instance;
        }

        public void CollisionSphere(Vector3 point, float radius, float radiusOffset)
        {

            // Explosion radius sphere
            Collider[] innerColliders = Physics.OverlapSphere(point, radius);

            if (innerColliders.Length == 0) return;

            // Voxel creation sphere
            Collider[] outerColliders = Physics.OverlapSphere(point, radius + radiusOffset);

            foreach (var c in innerColliders)
            {
                Debug.Log($"Chunk {c.GetComponentInParent<Chunk>().name} within explosion radius");
            }

            foreach (var c in outerColliders)
            {
                Debug.Log($"Chunk {c.GetComponentInParent<Chunk>().name} within creation radius");
            }

            // Filter colliders that are not from chunks

            // Find affected voxels

            // Destroy voxels within first sphere, create voxels within second sphere

            // Fire Chunk recreation method

        }


    }

}