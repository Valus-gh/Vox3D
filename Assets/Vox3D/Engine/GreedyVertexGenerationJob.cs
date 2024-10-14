using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Vox3D
{
    public struct GreedyVertexGeneratorJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<(Vector3, Voxel)>    voxels;

        public NativeArray<VoxelFace>           facesTop;
        public NativeArray<VoxelFace>           facesBottom;
        public NativeArray<VoxelFace>           facesLeft;
        public NativeArray<VoxelFace>           facesRight;
        public NativeArray<VoxelFace>           facesFront;
        public NativeArray<VoxelFace>           facesBack;

        public Vector3                          chunk;

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
            // Debug.Log($"{index} - Voxel{voxels[index].Item1} + Chunk at: {chunk}");

            int x = (int)voxels[index].Item1.x;
            int y = (int)voxels[index].Item1.y;
            int z = (int)voxels[index].Item1.z;

            Voxel voxel = voxels[index].Item2;

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

                for (int i = 0; i < faces.Length; i++)
                    if (faces[i]) GenerateFace(x, y, z, i, index);

            }

        }

        private bool IsFaceVisible(int x, int y, int z)
        {
            return true;
        }

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
                    face = facesTop[voxelIndex];

                    face.a = new Vector3(x, y + offset, z);
                    face.b = new Vector3(x, y + offset, z + offset);
                    face.c = new Vector3(x + offset, y + offset, z + offset);
                    face.d = new Vector3(x + offset, y + offset, z);
                    face.uvA = new Vector2(0, 0);
                    face.uvB = new Vector2(1, 0);
                    face.uvC = new Vector2(1, 1);
                    face.uvD = new Vector2(0, 1);

                    face.valid = true;
                    facesTop[voxelIndex] = face;
                    break;

                case 1: // Bottom
                    face = facesBottom[voxelIndex];

                    face.a = new Vector3(x, y, z);
                    face.b = new Vector3(x + offset, y, z);
                    face.c = new Vector3(x + offset, y, z + offset);
                    face.d = new Vector3(x, y, z + offset);
                    face.uvA = new Vector2(0, 0);
                    face.uvB = new Vector2(0, 1);
                    face.uvC = new Vector2(1, 1);
                    face.uvD = new Vector2(1, 0);

                    face.valid = true;
                    facesBottom[voxelIndex] = face;
                    break;

                case 2: // Left
                    face = facesLeft[voxelIndex];

                    face.a = new Vector3(x, y, z);
                    face.b = new Vector3(x, y, z + offset);
                    face.c = new Vector3(x, y + offset, z + offset);
                    face.d = new Vector3(x, y + offset, z);
                    face.uvA = new Vector2(0, 0);
                    face.uvB = new Vector2(0, 0);
                    face.uvC = new Vector2(0, 1);
                    face.uvD = new Vector2(0, 1);

                    face.valid = true;
                    facesLeft[voxelIndex] = face;
                    break;

                case 3: // Right
                    face = facesRight[voxelIndex];

                    face.a = new Vector3(x + offset, y, z + offset);
                    face.b = new Vector3(x + offset, y, z);
                    face.c = new Vector3(x + offset, y + offset, z);
                    face.d = new Vector3(x + offset, y + offset, z + offset);
                    face.uvA = new Vector2(1, 0);
                    face.uvB = new Vector2(1, 1);
                    face.uvC = new Vector2(1, 1);
                    face.uvD = new Vector2(1, 0);

                    face.valid = true;
                    facesRight[voxelIndex] = face;
                    break;

                case 4: // Front
                    face = facesFront[voxelIndex];

                    face.a = new Vector3(x, y, z + offset);
                    face.b = new Vector3(x + offset, y, z + offset);
                    face.c = new Vector3(x + offset, y + offset, z + offset);
                    face.d = new Vector3(x, y + offset, z + offset);
                    face.uvA = new Vector2(0, 1);
                    face.uvB = new Vector2(0, 1);
                    face.uvC = new Vector2(1, 1);
                    face.uvD = new Vector2(1, 1);

                    face.valid = true;
                    facesFront[voxelIndex] = face;
                    break;

                case 5: // Back
                    face = facesBack[voxelIndex];

                    face.a = new Vector3(x + offset, y, z);
                    face.b = new Vector3(x, y, z);
                    face.c = new Vector3(x, y + offset, z);
                    face.d = new Vector3(x + offset, y + offset, z);
                    face.uvA = new Vector2(0, 0);
                    face.uvB = new Vector2(1, 0);
                    face.uvC = new Vector2(1, 0);
                    face.uvD = new Vector2(0, 0);

                    face.valid = true;
                    facesBack[voxelIndex] = face;
                    break;

                default:
                    Debug.LogError("Face index invalid");
                    break;
            }

        }
    }

}