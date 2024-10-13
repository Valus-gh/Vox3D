using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

using Vox3D;
public struct GreedyMeshingJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<(Vector3, Voxel)>    voxels;

    public NativeList<Vector3>              vertices;
    public NativeList<int>                  indices;
    public NativeList<Vector2>              uv;

    public void Execute(int index)
    {
        Debug.Log($"{index} - Voxel{voxels[index].Item1}");
    }
}
