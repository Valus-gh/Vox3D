using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace Vox3D
{
    public struct VoxelGenerationJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Color32>     BiomeTexture;
        public int                      TextureWidth;
        public int                      TextureHeight;

        public NativeArray<Voxel>       Voxels;
        public int                      ChunkSize;
        public int                      VoxelSize;
        public Vector3                  ChunkPosition;

    public void Execute(int index)
        {
            int z = index % ChunkSize;
            int y = (index / ChunkSize) % ChunkSize;
            int x = (index / ChunkSize) / ChunkSize;

            Vector3 voxelWorldPosition = ChunkPosition + (new Vector3(x, y, z) * VoxelSize);
            Vector3 voxelWorldPositionNoSize = (ChunkPosition / VoxelSize) + new Vector3(x, y, z);

            // Calculate noise
            var hMap = Vox3DManager.Instance().World.HeightMap;
            var mMap = Vox3DManager.Instance().World.MoistureMap;

            float hNoise = hMap.ValueAt(voxelWorldPositionNoSize.x, voxelWorldPositionNoSize.z);
            float mNoise = mMap.ValueAt(voxelWorldPositionNoSize.y, voxelWorldPositionNoSize.z);

            // Find voxel color by using the biome lookup texture, with elevation and moisture as indices
            int colorY = (int) Mathf.Floor(hNoise * TextureHeight);
            int colorX = (int) Mathf.Floor(mNoise * TextureWidth);

            Color32 voxelColor  = BiomeTexture[colorX * TextureHeight + colorY];
            float elevation     = hNoise * hMap.MaxHeight;

            // Set voxel properties
            Voxel.VoxelType type = (voxelWorldPosition.y <= elevation) ? Voxel.VoxelType.Solid : Voxel.VoxelType.Air;

            Voxels[index] = new Voxel(type, voxelWorldPosition, type != Voxel.VoxelType.Air, voxelColor);
        }
    }

}