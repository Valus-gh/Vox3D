using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Text;

namespace Vox3D
{
    public struct VoxelGenerationJob : IJobParallelFor
    {
        public NativeArray<byte> WorldID;

        [ReadOnly]
        public NativeArray<Color32>     BiomeTexture;
        public int                      TextureWidth;
        public int                      TextureHeight;
        public float                    WaterLevel;

        public NativeArray<Voxel>       Voxels;
        public int                      ChunkSize;
        public int                      VoxelSize;
        public Vector3                  ChunkPosition;

    public void Execute(int index)
        {
            int z = index % ChunkSize;
            int y = (index / ChunkSize) % ChunkSize;
            int x = (index / ChunkSize) / ChunkSize;

            Vector3 voxelWorldPosition          = ChunkPosition + (new Vector3(x, y, z) * VoxelSize);
            Vector3 voxelWorldPositionNoSize    = (ChunkPosition / VoxelSize) + new Vector3(x, y, z);

            // Calculate noise

            var world = Vox3DEngine.GetWorld(Encoding.ASCII.GetString(WorldID));

            if(world is null)
            {
                Debug.LogError($"World {WorldID} could not be found. Voxel Generation failed.");
                return;
            }

            //TODO GET WORLD BY USING WORLDID
            var hMap = world.HeightMap;
            var mMap = world.MoistureMap;

            float hNoise = hMap.ValueAt(voxelWorldPositionNoSize.x, voxelWorldPositionNoSize.z);
            float mNoise = mMap.ValueAt(voxelWorldPositionNoSize.x, voxelWorldPositionNoSize.z);

            // Find voxel color by using the biome lookup texture, with elevation and moisture as indices
            int colorY = (int) Mathf.Floor(hNoise * (TextureHeight - 1));
            int colorX = (int) Mathf.Floor(mNoise * (TextureWidth - 1));

            // Texture is passed as an array, and must be navigated row by row from bottom to top
            Color voxelColor = BiomeTexture[colorY * TextureHeight + colorX];

            float elevation = hNoise * hMap.MaxHeight;

            Voxel.VoxelType type = Voxel.VoxelType.Air;

            if (voxelWorldPosition.y <= elevation)                      type = Voxel.VoxelType.Solid;
            if (voxelWorldPosition.y <= WaterLevel * hMap.MaxHeight)    type = Voxel.VoxelType.Water;

            // Set voxel properties
            Voxels[index] = new Voxel(type, voxelWorldPosition, type != Voxel.VoxelType.Air, voxelColor);
        }
    }

}