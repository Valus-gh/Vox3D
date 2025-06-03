using System;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Vox3D.Engine 
{
    public class ChunkDestructionHandler
    {
        private static ChunkDestructionHandler _Instance;
        public static ChunkDestructionHandler Instance()
        {
            if (_Instance is null)
                _Instance = new ChunkDestructionHandler();

            return _Instance;
        }

        /// <summary>
        /// Checks the world against two spheres. 
        /// The first represents the explosion radius passed as parameter.
        /// Voxels within this sphere are considered to be destroyed, therefore become inactive and their type becomes Air.
        /// The second sphere is used to populate the voxels nearby the explosion, so there are no holes in the world's mesh.
        /// All voxels outside of the explosion and within the larger sphere are set to active, and later become part of the mesh.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <param name="radiusOffset"></param>
        public void CollisionSphere_Destroy(Vector3 point, float radius, float radiusOffset)
        {
            // TODO Add water collision check

            // Explosion radius sphere
            Collider[] innerColliders = Physics.OverlapSphere(point, radius, LayerMask.GetMask("ChunkDestructionLayer"));

            if (innerColliders.Length == 0) 
                return;

            // Voxel creation sphere
            Collider[] outerColliders = Physics.OverlapSphere(point, radius + radiusOffset, LayerMask.GetMask("ChunkDestructionLayer"));

            // Filter colliders that are not from chunks
            List<Collider> innerChunkColliders = new List<Collider>();
            List<Collider> outerChunkColliders = new List<Collider>();

            foreach (var c in innerColliders) 
                if (c.transform.parent.tag.Equals("Chunk"))
                    innerChunkColliders.Add(c);

            foreach (var c in outerColliders)
                if (c.transform.parent.gameObject.tag.Equals("Chunk"))
                    outerChunkColliders.Add(c);

            // Find index of affected voxels
            List<(int, Vector3)> innerVoxels = new List<(int, Vector3)>();
            List<(int, Vector3)> allVoxels   = new List<(int, Vector3)>();

            // Find voxels within explosion radius
            foreach(var c in innerChunkColliders)
                innerVoxels.AddRange(FindImpactedVoxels_Sphere(c.GetComponentInParent<Chunk>(), point, radius));

            // Find voxels within creation radius
            foreach (var c in outerChunkColliders)
                allVoxels.AddRange(FindImpactedVoxels_Sphere(c.GetComponentInParent<Chunk>(), point, radius + radiusOffset));

            // Destroy voxels impacted by explosion
            innerChunkColliders.ForEach((collider) =>
            {
                Chunk chunk = collider.GetComponentInParent<Chunk>();

                innerVoxels.ForEach((pair) => {

                    if (pair.Item1 == chunk.GetInstanceID())
                    {
                        // Find impacted voxel in chunk
                        Vector3 index   = pair.Item2;
                        Voxel voxel     = chunk.Voxels[(int)index.x, (int)index.y, (int)index.z];

                        // Set to inactive, and make sure it is recognized as air in subsequent explosions
                        // If the chunk is set to not be destructible, skip the voxel
                        if (voxel.Destructible)
                        {
                            voxel.IsActive = false;
                            voxel.Type = Voxel.VoxelType.Air;

                            // If the chunk is correct, copy the new voxel values in the chunk's voxels
                            chunk.Voxels[(int)index.x, (int)index.y, (int)index.z] = new Voxel(voxel.Type, voxel.Position, voxel.IsActive, voxel.Color);
                        }
                    }

                });

            });


            // Create voxels at the bounds of the explosion
            List<(int, Vector3)> outerVoxels = allVoxels.Except(innerVoxels).ToList();

            outerChunkColliders.ForEach((collider) =>
            {
                Chunk chunk = collider.GetComponentInParent<Chunk>();

                outerVoxels.ForEach((pair) => {

                    if (pair.Item1 == chunk.GetInstanceID())
                    {
                        // Find impacted voxel in chunk
                        Vector3 index   = pair.Item2;
                        Voxel voxel     = chunk.Voxels[(int)index.x, (int)index.y, (int)index.z];

                        // Only set to Active if the voxel is supposed to be solid
                        if(voxel.Type != Voxel.VoxelType.Air) voxel.IsActive = true;

                        // If the chunk is correct, copy the new voxel values in the chunk's voxels
                        chunk.Voxels[(int)index.x, (int)index.y, (int)index.z] = new Voxel(voxel.Type, voxel.Position, voxel.IsActive, voxel.Color);
                    }

                });

                /*if (innerVoxels.Count == 0)
                {
                    chunk.ParentWorld.DeleteChunk(chunk);
                    outerChunkColliders.Remove(collider);
                }*/
            });

            // Fire Chunk recreation method
            outerChunkColliders.ForEach((c) => 
                PriorityCallStack.Instance().Push(() => {
                    c.GetComponentInParent<Chunk>().GenerateGeometry_Greedy();
                }, 0));

        }

        /// <summary>
        /// Checks the world against a sphere.
        /// Voxels within this sphere are set to no longer be Destructible
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        public void CollisionSphere_Preserve(Vector3 point, float radius, bool preserve = true)
        {
            // TODO Add water collision check

            // Explosion radius sphere
            Collider[] colliders = Physics.OverlapSphere(point, radius, LayerMask.GetMask("ChunkDestructionLayer"));

            if (colliders.Length == 0)
                return;

            // Filter colliders that are not from chunks
            List<Collider> chunkColliders = new List<Collider>();

            foreach (var c in colliders)
                if (c.transform.parent.tag.Equals("Chunk"))
                    chunkColliders.Add(c);

            // Find index of affected voxels
            List<(int, Vector3)> voxels = new List<(int, Vector3)>();

            // Find voxels within explosion radius
            foreach (var c in chunkColliders)
                voxels.AddRange(FindImpactedVoxels_Sphere(c.GetComponentInParent<Chunk>(), point, radius));

            // Destroy voxels impacted by explosion
            chunkColliders.ForEach((collider) =>
            {
                Chunk chunk = collider.GetComponentInParent<Chunk>();

                voxels.ForEach((pair) => {

                    if (pair.Item1 == chunk.GetInstanceID())
                    {
                        // Find impacted voxel in chunk
                        Vector3 index = pair.Item2;
                        Voxel voxel = chunk.Voxels[(int)index.x, (int)index.y, (int)index.z];

                        // Set to inactive, and make sure it is recognized as air in subsequent explosions
                        // If the chunk is set to not be destructible, skip the voxel
                        voxel.Destructible = !preserve;

                        // If the chunk is correct, copy the new voxel values in the chunk's voxels
                        chunk.Voxels[(int)index.x, (int)index.y, (int)index.z] = new Voxel(voxel.Type, voxel.Position, voxel.IsActive, voxel.Color, voxel.Destructible);
                    }

                });

            });
        }

        /// <summary>
        /// Checks the world against two spheres. 
        /// The first represents the explosion radius passed as parameter.
        /// Voxels within this sphere are considered to be destroyed, therefore become inactive and their type becomes Air.
        /// The second sphere is used to populate the voxels nearby the explosion, so there are no holes in the world's mesh.
        /// All voxels outside of the explosion and within the larger sphere are set to active, and later become part of the mesh.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <param name="radiusOffset"></param>
        public void CollisionCube_Preserve(Vector3 center, float width, float length, float height, Quaternion rotation, bool preserve = true)
        {
            // TODO Add water collision check

            // Explosion radius sphere
            Collider[] colliders = Physics.OverlapBox(center, new Vector3(width / 2, height / 2, length / 2), rotation, LayerMask.GetMask("ChunkDestructionLayer"));

            if (colliders.Length == 0)
                return;

            // Filter colliders that are not from chunks
            List<Collider> chunkColliders = new List<Collider>();

            foreach (var c in colliders)
                if (c.transform.parent.tag.Equals("Chunk"))
                    chunkColliders.Add(c);

            // Find index of affected voxels
            List<(int, Vector3)> voxels = new List<(int, Vector3)>();

            // Find voxels within explosion radius
            foreach (var c in chunkColliders)
                voxels.AddRange(FindImpactedVoxels_Cube(c.GetComponentInParent<Chunk>(), center, width, length, height, rotation));

            Debug.Log("Voxels to preserve: " + voxels.Count());

            // Destroy voxels impacted by explosion
            chunkColliders.ForEach((collider) =>
            {
                Chunk chunk = collider.GetComponentInParent<Chunk>();

                voxels.ForEach((pair) => {

                    if (pair.Item1 == chunk.GetInstanceID())
                    {
                        // Find impacted voxel in chunk
                        Vector3 index = pair.Item2;
                        Voxel voxel = chunk.Voxels[(int)index.x, (int)index.y, (int)index.z];

                        // Set to inactive, and make sure it is recognized as air in subsequent explosions
                        // If the chunk is set to not be destructible, skip the voxel
                        voxel.Destructible = !preserve;

                        // If the chunk is correct, copy the new voxel values in the chunk's voxels
                        chunk.Voxels[(int)index.x, (int)index.y, (int)index.z] = new Voxel(voxel.Type, voxel.Position, voxel.IsActive, voxel.Color, voxel.Destructible);
                    }

                });

            });
        }

        /// <summary>
        /// Checks the given chunk's voxels against a sphere passed as a center and a point.
        /// The voxels which have their centroid within the sphere have their index added to the returned list.
        /// Given that voxels are structs and are passed by values, we return the chunk's ID and the voxel indices, rather than the voxels themselves.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private List<(int, Vector3)> FindImpactedVoxels_Sphere(Chunk chunk, Vector3 point, float radius)
        {
            List<(int, Vector3)> impactedVoxels = new List<(int, Vector3)>();
            Voxel[,,] voxels = chunk.Voxels;

            // Find the extremes of the sphere
            var impactPointChunkSpace = chunk.transform.InverseTransformPoint(point);

            Vector2 xBounds = new Vector2(Mathf.Floor(impactPointChunkSpace.x - radius), Mathf.Ceil(impactPointChunkSpace.x + radius)) / chunk.VoxelSize;
            Vector2 yBounds = new Vector2(Mathf.Floor(impactPointChunkSpace.y - radius), Mathf.Ceil(impactPointChunkSpace.y + radius)) / chunk.VoxelSize;
            Vector2 zBounds = new Vector2(Mathf.Floor(impactPointChunkSpace.z - radius), Mathf.Ceil(impactPointChunkSpace.z + radius)) / chunk.VoxelSize;

            // Clamp extremes to the chunk's size
             
            if (xBounds.x < 0.0f)               xBounds.x = 0.0f;
            if (xBounds.y > chunk.ChunkSize)    xBounds.y = chunk.ChunkSize;

            if (yBounds.x < 0.0f)               yBounds.x = 0.0f;
            if (yBounds.y > chunk.ChunkSize)    yBounds.y = chunk.ChunkSize;

            if (zBounds.x < 0.0f)               zBounds.x = 0.0f;
            if (zBounds.y > chunk.ChunkSize)    zBounds.y = chunk.ChunkSize;

            // Identify voxels within explosion "bounding box", and check if they are within the explosion radius.
            for (int x = (int)xBounds.x; x < xBounds.y; x++)
            {
                for(int y = (int)yBounds.x; y < yBounds.y; y++)
                {
                    for(int z = (int)zBounds.x; z < zBounds.y; z++)
                    {
                        var voxelInChunkSpace   = chunk.transform.InverseTransformPoint(voxels[x, y, z].Position);
                        var voxelCenter         = voxelInChunkSpace + (new Vector3(chunk.VoxelSize, chunk.VoxelSize, chunk.VoxelSize) / 2.0f);

                        if(Vector3.Distance(voxelCenter, impactPointChunkSpace) <= radius && voxels[x, y, z].Type != Voxel.VoxelType.Air) 
                            impactedVoxels.Add((chunk.GetInstanceID(), new Vector3(x, y, z)));
                    }
                }
            }

            return impactedVoxels;
        }

        /// <summary>
        /// Checks the given chunk's voxels against a sphere passed as a center and a point.
        /// The voxels which have their centroid within the sphere have their index added to the returned list.
        /// Given that voxels are structs and are passed by values, we return the chunk's ID and the voxel indices, rather than the voxels themselves.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private List<(int, Vector3)> FindImpactedVoxels_Cube(Chunk chunk, Vector3 center, float width, float length, float height, Quaternion rotation)
        {
            List<(int, Vector3)> impactedVoxels = new List<(int, Vector3)>();
            Voxel[,,] voxels = chunk.Voxels;

            // Find the extremes of the sphere
            center = chunk.transform.InverseTransformPoint(center);

            Vector2 xBounds = new Vector2(Mathf.Floor(center.x - (width / 2)),  Mathf.Ceil(center.x + (width / 2))) / chunk.VoxelSize;
            Vector2 yBounds = new Vector2(Mathf.Floor(center.y - (height / 2)), Mathf.Ceil(center.y + (height / 2))) / chunk.VoxelSize;
            Vector2 zBounds = new Vector2(Mathf.Floor(center.z - (length / 2)), Mathf.Ceil(center.z + (length / 2))) / chunk.VoxelSize;

            // Clamp extremes to the chunk's size

            if (xBounds.x < 0.0f) xBounds.x = 0.0f;
            if (xBounds.y > chunk.ChunkSize) xBounds.y = chunk.ChunkSize;

            if (yBounds.x < 0.0f) yBounds.x = 0.0f;
            if (yBounds.y > chunk.ChunkSize) yBounds.y = chunk.ChunkSize;

            if (zBounds.x < 0.0f) zBounds.x = 0.0f;
            if (zBounds.y > chunk.ChunkSize) zBounds.y = chunk.ChunkSize;

            // Identify voxels within explosion "bounding box", and check if they are within the explosion radius.
            for (int x = (int)xBounds.x; x < xBounds.y; x++)
            {
                for (int y = (int)yBounds.x; y < yBounds.y; y++)
                {
                    for (int z = (int)zBounds.x; z < zBounds.y; z++)
                    {
                        var voxelInChunkSpace = chunk.transform.InverseTransformPoint(voxels[x, y, z].Position);

                        if (voxels[x, y, z].Type != Voxel.VoxelType.Air)
                            impactedVoxels.Add((chunk.GetInstanceID(), new Vector3(x, y, z)));
                    }
                }
            }

            return impactedVoxels;
        }

    }

}