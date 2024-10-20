using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Vox3D
{
    // TODO replace calls to Vox3DProperties with parameters passed to the Job

    /// <summary>
    /// Execute parallelizes the generation of mesh vertices for all voxels in a chunk
    /// Each thread instance has a set range of voxels it works on.
    /// Rather than generating a cube, a voxel is treated as a set of 6 faces.
    /// A face is only generated when it considered visible. 
    /// It needs to be at the edge of both a chunk and the world.
    /// </summary>
    public struct GreedyVertexGeneratorJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<(Vector3, Voxel)>    Voxels;

        public NativeArray<VoxelFace>           FacesTop;
        public NativeArray<VoxelFace>           FacesBottom;
        public NativeArray<VoxelFace>           FacesLeft;
        public NativeArray<VoxelFace>           FacesRight;
        public NativeArray<VoxelFace>           FacesFront;
        public NativeArray<VoxelFace>           FacesBack;

        public Vector3                          Chunk;

        public struct VoxelFace
        {
            public Vector3 a;
            public Vector3 b;
            public Vector3 c;
            public Vector3 d;

            public Vector2 uvA;
            public Vector2 uvB;
            public Vector2 uvC;
            public Vector2 uvD;

            public bool valid;
        }

        public void Execute(int index)
        {

            int x = (int)Voxels[index].Item1.x;
            int y = (int)Voxels[index].Item1.y;
            int z = (int)Voxels[index].Item1.z;

            Voxel voxel = Voxels[index].Item2;

            if (voxel.IsActive)
            {
                bool[] faces = new bool[6];

                // Check visibility for each face
                faces[0] = IsFaceVisible(x, y + 1, z); // Top
                faces[1] = IsFaceVisible(x, y - 1, z); // Bottom
                faces[2] = IsFaceVisible(x - 1, y, z); // Left
                faces[3] = IsFaceVisible(x + 1, y, z); // Right
                faces[4] = IsFaceVisible(x, y, z + 1); // Front
                faces[5] = IsFaceVisible(x, y, z - 1); // Back

                // For any face that was marked as visible, generate its vertices
                for (int i = 0; i < faces.Length; i++)
                    if (faces[i]) GenerateFace(x, y, z, i, index);

            }

        }

        private bool IsFaceVisible(int x, int y, int z)
        {
            return IsFaceVisibleInChunk(x, y, z)
                && IsFaceVisibleInWorld(x, y, z);
        }

        /// <summary>
        /// Given the position of the neighboring voxel, check whether it is outside of the chunk's bounds.
        /// If it is, then it shouldn't exist. Therefore, the current face is visible within the chunk.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsFaceVisibleInChunk(int x, int y, int z)
        {
            var chunkSize = Vox3DProperties.Instance().ChunkSize;

            if (x < 0 || x >= chunkSize
                || y < 0 || y >= chunkSize
                || z < 0 || z >= chunkSize) return true;
            
            int index = x * chunkSize * chunkSize + y * chunkSize + z;

            return !Voxels[index].Item2.IsActive;
        }

        /// <summary>
        /// Given the position of the neighboring voxel, check whether it belongs to another chunk.
        /// If it doesn't, the current face is visible within the world.
        /// If it does, check whether the voxel at the neighboring position is active.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsFaceVisibleInWorld(int x, int y, int z)
        {
            var properties = Vox3DProperties.Instance();

            // Position of the adjacent voxel in world space. Takes into consideration the size of the voxels.
            Vector3 voxelWorldPosition = Chunk + (new Vector3(x, y, z) * properties.VoxelSize);

            Chunk neighbor = properties.World.GetChunkAt(voxelWorldPosition);

            // If there is no neighbor in this direction, the face is visible
            if (neighbor is null) return true;

            // If there is a neighbor, check whether the adjacent voxel within it is active

            var voxelSize = properties.VoxelSize;
            var chunkSize = properties.ChunkSize;

            // This operation is done in GetChunkAt(), but needs to be repeated as we dont have access to Transforms
            // in threads other then the Main thread handled by Unity.
            
            Vector3Int neighborPosition = new Vector3Int(
                Mathf.FloorToInt(voxelWorldPosition.x / (chunkSize * voxelSize)) * (chunkSize * voxelSize),
                Mathf.FloorToInt(voxelWorldPosition.y / (chunkSize * voxelSize)) * (chunkSize * voxelSize),
                Mathf.FloorToInt(voxelWorldPosition.z / (chunkSize * voxelSize)) * (chunkSize * voxelSize)
            );


            Vector3 voxelChunkPosition = voxelWorldPosition - neighborPosition;
            
            int xNeighbor = Mathf.RoundToInt(voxelChunkPosition.x) / voxelSize;
            int yNeighbor = Mathf.RoundToInt(voxelChunkPosition.y) / voxelSize;
            int zNeighbor = Mathf.RoundToInt(voxelChunkPosition.z) / voxelSize;

            // If the position of the voxel within the neighboring chunk is valid, check if it is active
            if (xNeighbor >= 0 && xNeighbor < chunkSize
                && yNeighbor >= 0 && yNeighbor < chunkSize
                && zNeighbor >= 0 && zNeighbor < chunkSize) return !neighbor.Voxels[xNeighbor, yNeighbor, zNeighbor].IsActive;

            return false;

        }
        
        /// <summary>
        /// Generates the basic vertex geometry and uv coordinates for a given face. 
        /// If a face is generated, its respective valid flag is set to true.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="faceIndex"></param>
        /// <param name="voxelIndex"></param>
        private void GenerateFace(int x, int y, int z, int faceIndex, int voxelIndex)
        {
            var voxelSize = Vox3DProperties.Instance().VoxelSize;
            x *= voxelSize;
            y *= voxelSize;
            z *= voxelSize;
            int offset = voxelSize;

            VoxelFace face;

            switch (faceIndex)
            {
                case 0: // Top
                    face = FacesTop[voxelIndex];

                    face.a = new Vector3(x, y + offset, z);
                    face.b = new Vector3(x, y + offset, z + offset);
                    face.c = new Vector3(x + offset, y + offset, z + offset);
                    face.d = new Vector3(x + offset, y + offset, z);
                    face.uvA = new Vector2(0, 0);
                    face.uvB = new Vector2(1, 0);
                    face.uvC = new Vector2(1, 1);
                    face.uvD = new Vector2(0, 1);

                    face.valid = true;
                    FacesTop[voxelIndex] = face;
                    break;

                case 1: // Bottom
                    face = FacesBottom[voxelIndex];

                    face.a = new Vector3(x, y, z);
                    face.b = new Vector3(x + offset, y, z);
                    face.c = new Vector3(x + offset, y, z + offset);
                    face.d = new Vector3(x, y, z + offset);
                    face.uvA = new Vector2(0, 0);
                    face.uvB = new Vector2(0, 1);
                    face.uvC = new Vector2(1, 1);
                    face.uvD = new Vector2(1, 0);

                    face.valid = true;
                    FacesBottom[voxelIndex] = face;
                    break;

                case 2: // Left
                    face = FacesLeft[voxelIndex];

                    face.a = new Vector3(x, y, z);
                    face.b = new Vector3(x, y, z + offset);
                    face.c = new Vector3(x, y + offset, z + offset);
                    face.d = new Vector3(x, y + offset, z);
                    face.uvA = new Vector2(0, 0);
                    face.uvB = new Vector2(0, 0);
                    face.uvC = new Vector2(0, 1);
                    face.uvD = new Vector2(0, 1);

                    face.valid = true;
                    FacesLeft[voxelIndex] = face;
                    break;

                case 3: // Right
                    face = FacesRight[voxelIndex];

                    face.a = new Vector3(x + offset, y, z + offset);
                    face.b = new Vector3(x + offset, y, z);
                    face.c = new Vector3(x + offset, y + offset, z);
                    face.d = new Vector3(x + offset, y + offset, z + offset);
                    face.uvA = new Vector2(1, 0);
                    face.uvB = new Vector2(1, 1);
                    face.uvC = new Vector2(1, 1);
                    face.uvD = new Vector2(1, 0);

                    face.valid = true;
                    FacesRight[voxelIndex] = face;
                    break;

                case 4: // Front
                    face = FacesFront[voxelIndex];

                    face.a = new Vector3(x, y, z + offset);
                    face.b = new Vector3(x + offset, y, z + offset);
                    face.c = new Vector3(x + offset, y + offset, z + offset);
                    face.d = new Vector3(x, y + offset, z + offset);
                    face.uvA = new Vector2(0, 1);
                    face.uvB = new Vector2(0, 1);
                    face.uvC = new Vector2(1, 1);
                    face.uvD = new Vector2(1, 1);

                    face.valid = true;
                    FacesFront[voxelIndex] = face;
                    break;

                case 5: // Back
                    face = FacesBack[voxelIndex];

                    face.a = new Vector3(x + offset, y, z);
                    face.b = new Vector3(x, y, z);
                    face.c = new Vector3(x, y + offset, z);
                    face.d = new Vector3(x + offset, y + offset, z);
                    face.uvA = new Vector2(0, 0);
                    face.uvB = new Vector2(1, 0);
                    face.uvC = new Vector2(1, 0);
                    face.uvD = new Vector2(0, 0);

                    face.valid = true;
                    FacesBack[voxelIndex] = face;
                    break;

                default:
                    Debug.LogError("Face index invalid");
                    break;
            }

        }

        
    }

}