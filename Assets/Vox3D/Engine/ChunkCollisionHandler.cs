using System;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

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

            // Filter colliders that are not from chunks

            List<Collider> innerChunkColliders = new List<Collider>();
            List<Collider> outerChunkColliders = new List<Collider>();

            foreach (var c in innerColliders) 
                if (c.gameObject.tag.Equals("Chunk"))
                    innerChunkColliders.Add(c);

            foreach (var c in outerColliders)
                if (c.gameObject.tag.Equals("Chunk"))
                    outerChunkColliders.Add(c);

            // Find affected voxels

            List<Voxel> destroyedVoxels = new List<Voxel>();
            List<Voxel> createdVoxels = new List<Voxel>();

            // Find voxels within explosion radius
            foreach(var c in innerChunkColliders)
                destroyedVoxels.AddRange(FindImpactedVoxels(c.GetComponentInParent<Chunk>(), point, radius));

            // Find voxels within creation radius
            foreach (var c in outerChunkColliders)
                createdVoxels.AddRange(FindImpactedVoxels(c.GetComponentInParent<Chunk>(), point, radius + radiusOffset));

            // Destroy voxels impacted by explosion
            destroyedVoxels.ForEach((v) => v.IsActive = false);

            // Create voxels at the bounds of the explosion
            createdVoxels.Except(destroyedVoxels).ToList().ForEach((v) => v.IsActive = true);

            // Fire Chunk recreation method

        }

        private List<Voxel> FindImpactedVoxels(Chunk chunk, Vector3 point, float radius)
        {
            List<Voxel> impactedVoxels = new List<Voxel>();
            Voxel[,,] voxels = chunk.Voxels;

            // Find the extremes of the sphere
            var impactPointChunkSpace = chunk.transform.InverseTransformPoint(point);

            Vector2 xBounds = new Vector2(Mathf.Floor(impactPointChunkSpace.x - radius), Mathf.Ceil(impactPointChunkSpace.x + radius));
            Vector2 yBounds = new Vector2(Mathf.Floor(impactPointChunkSpace.y - radius), Mathf.Ceil(impactPointChunkSpace.y + radius));
            Vector2 zBounds = new Vector2(Mathf.Floor(impactPointChunkSpace.z - radius), Mathf.Ceil(impactPointChunkSpace.z + radius));

            // Clamp extremes to the chunk's size

            if (xBounds.x < 0.0f)               xBounds.x = 0.0f;
            if (xBounds.y > chunk.ChunkSize)    xBounds.y = chunk.ChunkSize;

            if (yBounds.x < 0.0f)               yBounds.x = 0.0f;
            if (yBounds.y > chunk.ChunkSize)    yBounds.y = chunk.ChunkSize;

            if (zBounds.x < 0.0f)               zBounds.x = 0.0f;
            if (zBounds.y > chunk.ChunkSize)    zBounds.y = chunk.ChunkSize;

            // Identify voxels within explosion "bounding box", and check if they are within the explosion radius.
            for(int x = (int)xBounds.x; x < xBounds.y; x++)
            {
                for(int y = (int)yBounds.x; y < yBounds.y; y++)
                {
                    for(int z = (int)zBounds.x; z < zBounds.y; z++)
                    {
                        var voxelInChunkSpace   = chunk.transform.InverseTransformPoint(voxels[x, y, z].Position);
                        var voxelCenter         = voxelInChunkSpace + (new Vector3(chunk.VoxelSize, chunk.VoxelSize, chunk.VoxelSize) / 2.0f);

                        if(Vector3.Distance(voxelCenter, impactPointChunkSpace) <= radius && voxels[x, y, z].Type != Voxel.VoxelType.Air) 
                            impactedVoxels.Add(voxels[x, y, z]);
                    }
                }
            }

            Debug.Log($"Chunk {chunk.name} has {impactedVoxels.Count} impacted voxels:");

            return impactedVoxels;
        }


    }

}