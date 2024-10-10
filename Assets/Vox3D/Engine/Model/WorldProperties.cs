using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldProperties
{
    private int _worldSize; // How many chunks for each axis in a world
    private int _chunkSize; // How many voxels for each axis in a chunk
    private int _voxelSize; // The size of the edges of the voxels
    public int WorldSize { get => _worldSize; set => _worldSize = value; }
    public int ChunkSize { get => _chunkSize; set => _chunkSize = value; }
    public int VoxelSize { get => _voxelSize; set => _voxelSize = value; }

    public WorldProperties(int worldSize, int chunkSize, int voxelSize)
    {
        WorldSize = worldSize;
        ChunkSize = chunkSize;
        VoxelSize = voxelSize;
    }
    public WorldProperties(string pathToXml)
    {
        // TODO Get Properties from XML Parser to fill an object when this is created
    }

}
