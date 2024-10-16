using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace Vox3D
{
    public struct VoxelGenerationJob : IJobParallelFor
    {
        public NativeArray<Voxel>   voxels;
        public int                  chunkSize;
        public int                  voxelSize;
        public Vector3              chunkPosition;

    public void Execute(int index)
        {
            int z = index % chunkSize;
            int y = (index / chunkSize) % chunkSize;
            int x = (index / chunkSize) / chunkSize;

            Vector3 voxelWorldPosition = chunkPosition + (new Vector3(x, y, z) * voxelSize);
            Vector3 voxelWorldPositionNoSize = (chunkPosition / voxelSize) + new Vector3(x, y, z);

            // Calculate noise
            var map = Vox3DProperties.Instance().World.HeightMap;

            float noise     = map.ValueAt(voxelWorldPositionNoSize.x, voxelWorldPositionNoSize.z);
            noise           = (noise + 1) / 2;
            float height    = noise * map.MaxHeight;

            // Set voxel properties
            Voxel.VoxelType type = (voxelWorldPosition.y <= height) ? Voxel.VoxelType.Grass : Voxel.VoxelType.Air;

            voxels[index] = new Voxel(type, voxelWorldPosition, type != Voxel.VoxelType.Air);
        }
    }

}