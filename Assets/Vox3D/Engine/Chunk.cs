using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;

namespace Vox3D
{
    public class Chunk : MonoBehaviour
    {
        private int             _ChunkSize;     // Property taken from WorldProperties. We save it locally to avoid continuous Instance calls
        private int             _VoxelSize;     // Property taken from WorldProperties. We save it locally to avoid continuous Instance calls
        private Voxel[,,]       _Voxels;        // Voxels within this chunk. These are only Voxel objects, no geometry

        private List<Vector3>   _Vertices;      // Holds the geometry vertices for this chunk
        private List<int>       _Indices;       // Holds the indices for both triangles of each voxel face
        private List<Vector2>   _Uvs;           // Holds uv coordinates for each voxel face
        private List<Color32>   _Colors;           // Holds uv coordinates for each voxel face

        private MeshFilter      _MeshFilter;
        private MeshCollider    _MeshCollider;
        private MeshRenderer    _MeshRenderer;

        public int ChunkSize                { get => _ChunkSize; set => _ChunkSize = value; }
        public int VoxelSize                { get => _VoxelSize; set => _VoxelSize = value; }
        public Voxel[,,] Voxels             { get => _Voxels; set => _Voxels = value; }
        public List<Vector3> Vertices       { get => _Vertices; set => _Vertices = value; }
        public List<int> Indices            { get => _Indices; set => _Indices = value; }
        public List<Vector2> Uvs            { get => _Uvs; set => _Uvs = value; }
        public MeshFilter MeshFilter        { get => _MeshFilter; set => _MeshFilter = value; }
        public MeshCollider MeshCollider    { get => _MeshCollider; set => _MeshCollider = value; }
        public MeshRenderer MeshRenderer    { get => _MeshRenderer; set => _MeshRenderer = value; }
        public List<Color32> Colors         { get => _Colors; set => _Colors = value; }

        public void PopulateChunk()
        {
            var nVoxelsInChunk  = ChunkSize * ChunkSize * ChunkSize;
            var voxelsData      = new NativeArray<Voxel>(nVoxelsInChunk, Allocator.TempJob);

            var biomeTex            = Vox3DManager.Instance().Properties.BiomeLookupTexture;
            var biomeTexData        = new NativeArray<Color32>(biomeTex.GetPixels32().Length, Allocator.TempJob);
            biomeTexData.CopyFrom(biomeTex.GetPixels32());

            VoxelGenerationJob job = new VoxelGenerationJob
            {
                BiomeTexture    = biomeTexData,
                TextureWidth    = biomeTex.width,
                TextureHeight   = biomeTex.height,
                Voxels          = voxelsData,
                ChunkSize       = ChunkSize,
                VoxelSize       = VoxelSize,
                ChunkPosition   = transform.position
            };

            var handle = job.Schedule(nVoxelsInChunk, 16);
            handle.Complete();

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        int voxelIndex  = x * ChunkSize * ChunkSize + y * ChunkSize + z;
                        Voxel voxel     = voxelsData[voxelIndex];

                        Voxels[x, y, z] = new Voxel(voxel.Type, voxel.Position, voxel.IsActive, voxel.Color);
                    }
                }
            }

            voxelsData.Dispose();
            biomeTexData.Dispose();

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
                Voxels          = voxelsCopy,
                FacesTop        = facesTop,
                FacesBottom     = facesBottom,
                FacesLeft       = facesLeft,
                FacesRight      = facesRight,
                FacesFront      = facesFront,
                FacesBack       = facesBack,
                Chunk           = transform.position
            };
            
            var handle = job.Schedule(nVoxelsInChunk, 16);
            
            // Refresh geometry buffers

            Vertices.Clear();
            Indices.Clear();
            Uvs.Clear();

            handle.Complete();

            for (int i = 0; i < nVoxelsInChunk; i++)
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

                        Colors.Add(face.color);
                        Colors.Add(face.color);
                        Colors.Add(face.color);
                        Colors.Add(face.color);

                    }

                }

            }

            Mesh mesh = MeshFilter.mesh;
            //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            if (mesh is null) mesh = new Mesh();

            mesh.Clear();
            mesh.vertices       = Vertices.ToArray();
            mesh.triangles      = Indices.ToArray();
            mesh.uv             = Uvs.ToArray();
            mesh.colors32       = Colors.ToArray();

            mesh.RecalculateNormals();

            MeshFilter.mesh         = mesh;
            MeshCollider.sharedMesh = mesh;
            MeshRenderer.material   = Vox3DManager.Instance().Properties.VoxelDefaultMaterial;

            facesTop.Dispose();
            facesBottom.Dispose();
            facesLeft.Dispose();
            facesRight.Dispose();
            facesFront.Dispose();
            facesBack.Dispose();
            voxelsCopy.Dispose();

        }

        public void PurgeChunk()
        {
            System.Array.Clear(Voxels, 0, Voxels.Length);
            Vertices.Clear();
            Indices.Clear();
            Uvs.Clear();

            _MeshFilter.sharedMesh.Clear();
            _MeshCollider.sharedMesh.Clear();
        }

    }

}
// TODO modularize vertex building to very small functions that can be called from outside