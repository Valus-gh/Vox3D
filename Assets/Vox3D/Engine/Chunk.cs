using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;

namespace Vox3D
{
    public class Chunk : MonoBehaviour
    {
        private int             _chunkSize;     // Property taken from WorldProperties. We save it locally to avoid continuous Instance calls
        private int             _voxelSize;     // Property taken from WorldProperties. We save it locally to avoid continuous Instance calls
        private Voxel[,,]       _voxels;        // Voxels within this chunk. These are only Voxel objects, no geometry

        private List<Vector3>   _vertices;      // Holds the geometry vertices for this chunk
        private List<int>       _indices;     // Holds the indices for both triangles of each voxel face
        private List<Vector2>   _uvs;           // Holds uv coordinates for each voxel face

        private MeshFilter      _meshFilter;
        private MeshCollider    _meshCollider;
        private MeshRenderer    _meshRenderer;

        public int ChunkSize            { get => _chunkSize; set => _chunkSize = value; }
        public int VoxelSize            { get => _voxelSize; set => _voxelSize = value; }
        public Voxel[,,] Voxels         { get => _voxels; set => _voxels = value; }
        public List<Vector3> Vertices   { get => _vertices; set => _vertices = value; }
        public List<int> Indices        { get => _indices; set => _indices = value; }
        public List<Vector2> Uvs        { get => _uvs; set => _uvs = value; }
        public MeshFilter MeshFilter { get => _meshFilter; set => _meshFilter = value; }
        public MeshCollider MeshCollider { get => _meshCollider; set => _meshCollider = value; }
        public MeshRenderer MeshRenderer { get => _meshRenderer; set => _meshRenderer = value; }

        public Chunk()
        {
        }
        public Chunk(int chunkSize, int voxelSize)
        {
            _chunkSize = chunkSize;
            _voxelSize = voxelSize;
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

        public void GenerateGeometry_Greedy()
        {
            var nVoxelsInChunk  = ChunkSize * ChunkSize * ChunkSize;
            var voxelsCopy      = new NativeArray<(Vector3, Voxel)>(nVoxelsInChunk, Allocator.TempJob);

            // Six faces for each voxel
            var facesTop        = new NativeArray<GreedyVertexGeneratorJob.VoxelFace>(nVoxelsInChunk, Allocator.TempJob);
            var facesBottom     = new NativeArray<GreedyVertexGeneratorJob.VoxelFace>(nVoxelsInChunk, Allocator.TempJob);
            var facesLeft       = new NativeArray<GreedyVertexGeneratorJob.VoxelFace>(nVoxelsInChunk, Allocator.TempJob);
            var facesRight      = new NativeArray<GreedyVertexGeneratorJob.VoxelFace>(nVoxelsInChunk, Allocator.TempJob);
            var facesFront      = new NativeArray<GreedyVertexGeneratorJob.VoxelFace>(nVoxelsInChunk, Allocator.TempJob);
            var facesBack       = new NativeArray<GreedyVertexGeneratorJob.VoxelFace>(nVoxelsInChunk, Allocator.TempJob);

            var copyIndex       = 0;

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        voxelsCopy[copyIndex++] = (new Vector3(x, y, z), Voxels[x, y, z]);
                    }
                }
            }

            var job = new GreedyVertexGeneratorJob
            {
                voxels          = voxelsCopy,
                facesTop        = facesTop,
                facesBottom     = facesBottom,
                facesLeft       = facesLeft,
                facesRight      = facesRight,
                facesFront      = facesFront,
                facesBack       = facesBack,
                chunk           = transform.position
            };

            var handle = job.Schedule(nVoxelsInChunk, 16);
            handle.Complete();

            //job.Run(nVoxelsInChunk);

            // Refresh geometry buffers

            Vertices.Clear();
            Indices.Clear();
            Uvs.Clear();

            for(int i = 0; i < nVoxelsInChunk; i++)
            {
                GreedyVertexGeneratorJob.VoxelFace face;

                for (int orientation = 0; orientation < 6; orientation++)
                {

                    switch (orientation)
                    {
                        case 0:
                            face = facesTop[i];
                            break;
                        case 1:
                            face = facesBottom[i];
                            break;
                        case 2:
                            face = facesLeft[i];
                            break;
                        case 3:
                            face = facesRight[i];
                            break;
                        case 4:
                            face = facesFront[i];
                            break;
                        case 5:
                            face = facesBack[i];
                            break;

                        default:
                            face = new GreedyVertexGeneratorJob.VoxelFace();
                            break;
                    }

                    if (face.valid)
                    {
                        Vertices.Add(new Vector3(face.a.x, face.a.y, face.a.z));
                        Vertices.Add(new Vector3(face.b.x, face.b.y, face.b.z));
                        Vertices.Add(new Vector3(face.c.x, face.c.y, face.c.z));
                        Vertices.Add(new Vector3(face.d.x, face.d.y, face.d.z));

                        //Triangle A
                        Indices.Add(Vertices.Count - 4);
                        Indices.Add(Vertices.Count - 3);
                        Indices.Add(Vertices.Count - 2);

                        //Triangle B
                        Indices.Add(Vertices.Count - 4);
                        Indices.Add(Vertices.Count - 2);
                        Indices.Add(Vertices.Count - 1);

                        Uvs.Add(face.uvA);
                        Uvs.Add(face.uvB);
                        Uvs.Add(face.uvC);
                        Uvs.Add(face.uvD);
                    }

                }

            }

            Mesh mesh = MeshFilter.mesh;
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            if (mesh is null) mesh = new Mesh();

            mesh.Clear();
            mesh.vertices       = Vertices.ToArray();
            mesh.triangles      = Indices.ToArray();
            mesh.uv             = Uvs.ToArray();

            mesh.RecalculateNormals();

            MeshFilter.mesh         = mesh;
            MeshCollider.sharedMesh = mesh;
            MeshRenderer.material   = Vox3DProperties.Instance().VoxelDefaultMaterial;

            facesTop.Dispose();
            facesBottom.Dispose();
            facesLeft.Dispose();
            facesRight.Dispose();
            facesFront.Dispose();
            facesBack.Dispose();
            voxelsCopy.Dispose();

        }

    }

}