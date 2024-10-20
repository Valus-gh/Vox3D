using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace Vox3D
{
    public struct VoxelGenerationJob : IJobParallelFor
    {
        public NativeArray<Voxel>   Voxels;
        public int                  ChunkSize;
        public int                  VoxelSize;
        public Vector3              ChunkPosition;

    public void Execute(int index)
        {
            int z = index % ChunkSize;
            int y = (index / ChunkSize) % ChunkSize;
            int x = (index / ChunkSize) / ChunkSize;

            Vector3 voxelWorldPosition = ChunkPosition + (new Vector3(x, y, z) * VoxelSize);
            Vector3 voxelWorldPositionNoSize = (ChunkPosition / VoxelSize) + new Vector3(x, y, z);

            // Calculate noise
            var map = Vox3DProperties.Instance().World.HeightMap;

            float noise     = map.ValueAt(voxelWorldPositionNoSize.x, voxelWorldPositionNoSize.z);
            noise           = (noise + 1) / 2;
            float height    = noise * map.MaxHeight;

            // Set voxel properties
            Voxel.VoxelType type = (voxelWorldPosition.y <= height) ? Voxel.VoxelType.Grass : Voxel.VoxelType.Air;

            Voxels[index] = new Voxel(type, voxelWorldPosition, type != Voxel.VoxelType.Air);
        }
    }

}