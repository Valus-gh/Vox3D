using System.Collections.Generic;
using UnityEngine;

using Unity.Jobs;
using Unity.Collections;

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
                tracker.voxels      = new NativeArray<Voxel>(nVoxelsInChunk, Allocator.TempJob);

                var biomeTex        = Vox3DManager.Instance().Properties.BiomeLookupTexture;
                tracker.colors      = new NativeArray<Color32>(biomeTex.GetPixels32().Length, Allocator.TempJob);
                tracker.colors.CopyFrom(biomeTex.GetPixels32());

                VoxelGenerationJob job = new VoxelGenerationJob
                {
                    BiomeTexture    = tracker.colors,
                    TextureWidth    = biomeTex.width,
                    TextureHeight   = biomeTex.height,
                    Voxels          = tracker.voxels,
                    ChunkSize       = chunkSize,
                    VoxelSize       = voxelSize,
                    ChunkPosition   = tracker.Chunk.transform.localPosition
                };

                tracker.Job = job.Schedule(nVoxelsInChunk, 16);

            }, 0);

            // Schedule Job End
            PriorityCallStack.Instance().Push(() => {

                tracker.Job.Complete();

                var chunkSize = tracker.Chunk.ChunkSize;

                for (int x = 0; x < chunkSize; x++)
                {
                    for (int y = 0; y < chunkSize; y++)
                    {
                        for (int z = 0; z < chunkSize; z++)
                        {
                            int voxelIndex  = x * chunkSize * chunkSize + y * chunkSize + z;
                            Voxel voxel     = tracker.voxels[voxelIndex];

                            tracker.Chunk.Voxels[x, y, z] = new Voxel(voxel.Type, voxel.Position, voxel.IsActive, voxel.Color);
                        }
                    }
                }

                tracker.voxels.Dispose();
                tracker.colors.Dispose();

            }, 10);

        }

    }

}