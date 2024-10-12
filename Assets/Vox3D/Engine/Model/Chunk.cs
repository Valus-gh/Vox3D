using System.Collections.Generic;
using UnityEngine;

namespace Vox3D
{
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

        private void Start()
        {
            _meshFilter     = new MeshFilter();
            _meshCollider   = new MeshCollider();
            _meshRenderer   = new MeshRenderer();
        }

        public void PopulateChunk()
        {
            Debug.Log("POPULATING CHUNK " + name);

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        // Position the voxel by using an offset from the origin of the chunk, scaled by the VoxelSize
                        Vector3 voxelPosition   = transform.position + (new Vector3(x, y, z) * VoxelSize);

                        // Use default voxel type for now
                        Voxel.VoxelType type    = Voxel.VoxelType.Grass;
                        Voxels[x, y, z]         = new Voxel(Voxel.VoxelType.Grass, voxelPosition, type != Voxel.VoxelType.Air);
                    }
                }
            }
        }

    }

}