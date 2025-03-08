using System.Collections.Generic;
using UnityEngine;

using Unity.Jobs;
using Unity.Collections;
using System.Text;

namespace Vox3D.Parallel
{

    public class ParallelVoxelBuilder
    {
        private class VoxelJobTracker
        {
            public bool         Complete;
            public JobHandle    Job;
            public Chunk        Chunk;

            public NativeArray<Voxel>   voxels;
            public NativeArray<Color32> colors;
            public NativeArray<byte>    worldId;
        }

        private static ParallelVoxelBuilder _Instance;
        public static ParallelVoxelBuilder Instance()
        {
            if (_Instance is null)
                _Instance = new ParallelVoxelBuilder();

            return _Instance;
        }

        private Dictionary<int, VoxelJobTracker> _Jobs;

        private ParallelVoxelBuilder()
        {
            _Jobs = new Dictionary<int, VoxelJobTracker>();
        }

        public void Build(Chunk chunk)
        {
            var tracker = new VoxelJobTracker();
            tracker.Chunk = chunk;

            _Jobs.Add(chunk.GetInstanceID(), tracker);

            GenerateVoxels(tracker);

        }

        private void GenerateVoxels(VoxelJobTracker tracker)
        {

            // Schedule Job Start
            PriorityCallStack.Instance().Push(() => {

                var chunkSize       = tracker.Chunk.ChunkSize;
                var voxelSize       = tracker.Chunk.VoxelSize;

                var nVoxelsInChunk  = chunkSize * chunkSize * chunkSize;
                tracker.voxels      = new NativeArray<Voxel>(nVoxelsInChunk, Allocator.Persistent);

                var biomeTex        = tracker.Chunk.ParentWorld.Properties.BiomeLookupTexture;
                tracker.colors      = new NativeArray<Color32>(biomeTex.GetPixels32().Length, Allocator.Persistent);
                tracker.colors.CopyFrom(biomeTex.GetPixels32());

                var idBytes         = Encoding.ASCII.GetBytes(tracker.Chunk.ParentWorld.ID);
                tracker.worldId     = new NativeArray<byte>(idBytes, Allocator.TempJob);

                VoxelGenerationJob job = new VoxelGenerationJob
                {
                    WorldID         = tracker.worldId,
                    BiomeTexture    = tracker.colors,
                    TextureWidth    = biomeTex.width,
                    TextureHeight   = biomeTex.height,
                    WaterLevel      = tracker.Chunk.ParentWorld.Properties.WaterLevel,
                    Voxels          = tracker.voxels,
                    ChunkSize       = chunkSize,
                    VoxelSize       = voxelSize,
                    ChunkPosition   = tracker.Chunk.transform.localPosition
                };

                tracker.Job = job.Schedule(nVoxelsInChunk, 1);

            }, 0);

            // Schedule Job End
            PriorityCallStack.Instance().Push(() => {

                tracker.Job.Complete();

                bool IsChunkSolid = false;

                var chunkSize = tracker.Chunk.ChunkSize;

                for (int x = 0; x < chunkSize; x++)
                {
                    for (int y = 0; y < chunkSize; y++)
                    {
                        for (int z = 0; z < chunkSize; z++)
                        {
                            int voxelIndex  = x * chunkSize * chunkSize + y * chunkSize + z;
                            Voxel voxel     = tracker.voxels[voxelIndex];

                            IsChunkSolid |= (voxel.Type == Voxel.VoxelType.Solid);

                            tracker.Chunk.Voxels[x, y, z] = new Voxel(voxel.Type, voxel.Position, voxel.IsActive, voxel.Color);
                        }
                    }
                }

                tracker.voxels.Dispose();
                tracker.colors.Dispose();
                tracker.worldId.Dispose();

                GameObject destructionColliderObject = new GameObject(
                                                    $"ChunkDestructionCollider_" +
                                                    $"{tracker.Chunk.transform.position.x / chunkSize}_" +
                                                    $"{tracker.Chunk.transform.position.y / chunkSize}_" +
                                                    $"{tracker.Chunk.transform.position.z / chunkSize}");
                destructionColliderObject.transform.position = tracker.Chunk.transform.position;
                destructionColliderObject.transform.parent = tracker.Chunk.transform;
                destructionColliderObject.layer = LayerMask.NameToLayer("ChunkDestructionLayer");

                tracker.Chunk.ChunkDestructionCollider = destructionColliderObject.AddComponent<MeshCollider>();
                tracker.Chunk.ChunkDestructionCollider.sharedMesh = Chunk.DefaultChunkColliderMesh(tracker.Chunk.ParentWorld.Properties);

                if (!IsChunkSolid)
                {
                    tracker.Chunk.ParentWorld.DeleteChunk(tracker.Chunk);
                    tracker.Chunk.PurgeChunk();
                }

            }, 30);

        }

    }

}