using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vox3D;

public class TowerLocator
{
    public static List<Vector3> GenerateTowerLocations(World world, int towers)
    {
        List<Vector3> locations     = new List<Vector3>();
        List<Vector3> usedChunks    = new List<Vector3>();

        if (world is null) 
            return null;

        var chunks  = world.Chunks;

        if (chunks.Count == 0)
            return null;

        for(int i = 0; i < towers; i++)
        {
            Vector3 chunkIndex = new Vector3();

            do {

                chunkIndex = RandomChunk(chunks, world.Properties.WorldSize, world.Properties.ChunkSize, world.Properties.VoxelSize);

            } while (usedChunks.Contains(chunkIndex));

            usedChunks.Add(chunkIndex);
            
            Chunk chunk; 
            chunks.TryGetValue(chunkIndex, out chunk);

            Vector3 voxelIndex = RandomVoxel(chunk.Voxels, chunk.ChunkSize);
            if (voxelIndex == new Vector3(-1, -1, -1))
            {
                i--;
                continue;
            }
            
            locations.Add(chunkIndex + (voxelIndex * world.Properties.VoxelSize));
            
        }

        Debug.Log("Chunks:");
        usedChunks.ForEach(x => Debug.Log(x / world.Properties.ChunkSize));
        Debug.Log("Voxels:");
        locations.ForEach(x => Debug.Log(x));

        return locations;
    }

    private static Vector3 RandomChunk(Dictionary<Vector3, Chunk> chunks, int worldSize, int chunkSize, int voxelSize)
    {

        int chunkX = (int)(Random.value * (worldSize - 1)) * chunkSize * voxelSize;
        int chunkZ = (int)(Random.value * (worldSize - 1)) * chunkSize * voxelSize;

        Vector3 chunkIndex = new Vector3(chunkX, 0, chunkZ);

        for (int j = 0; j < worldSize; j++)
        {
            chunkIndex.y = j * chunkSize * voxelSize;

            if (!chunks.ContainsKey(chunkIndex))
            {
                chunkIndex.y -= chunkSize * voxelSize;
                break;
            }

        }

        return chunkIndex;

    }

    private static Vector3 RandomVoxel(Voxel[,,] voxels, int chunkSize, int maxTries = 100)
    {
        Vector3 voxelIndex = new Vector3();

        do
        {

            int voxelX = (int)(Random.value * (chunkSize - 1));
            int voxelZ = (int)(Random.value * (chunkSize - 1));

            voxelIndex = new Vector3(voxelX, 0, voxelZ);

            if (maxTries-- == 0)
                return new Vector3(-1, -1, -1);

        } while(
                voxels[(int)voxelIndex.x, (int)voxelIndex.y, (int)voxelIndex.z].Type == Voxel.VoxelType.Water 
                || voxels[(int)voxelIndex.x, (int)voxelIndex.y, (int)voxelIndex.z].Type == Voxel.VoxelType.Air
            );

        for (int j = 0; j < chunkSize; j++)
        {
            voxelIndex.y = j;

            if(voxels[(int)voxelIndex.x, (int)voxelIndex.y, (int)voxelIndex.z].Type == Voxel.VoxelType.Air)
            {
                voxelIndex.y--;
                break;
            }
        }

        return voxelIndex;

    }

}
