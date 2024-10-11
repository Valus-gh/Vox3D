using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private int             _chunkSize;     // Property taken from WorldProperties. We save it locally to avoid continuous Instance calls
    private int             _voxelSize;     // Property taken from WorldProperties. We save it locally to avoid continuous Instance calls
    private Voxel[,,]       _voxels;        // Voxels within this chunk. These are only Voxel objects, no geometry

    private List<Vector3>   _vertices;      // Holds the geometry vertices for this chunk
    private List<int>       _triangles;     // Holds the indices for both triangles of each voxel face
    private List<Vector2>   _uvs;           // Holds uv coordinates for each voxel face

    private MeshFilter      _meshFilter;
    private MeshCollider    _meshCollider;
    private MeshRenderer    _meshRenderer;

    public int ChunkSize    { get => _chunkSize; set => _chunkSize = value; }
    public int VoxelSize    { get => _voxelSize; set => _voxelSize = value; }
    public Voxel[,,] Voxels { get => _voxels; set => _voxels = value; }

    public Chunk()
    {
    }
    public Chunk(int chunkSize, int voxelSize)
    {
        _chunkSize = chunkSize;
        _voxelSize = voxelSize;
    }

}
