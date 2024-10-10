using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public enum VoxelType
    {
        Air,
        Grass,
        Sand,
        Rock,
        Snow,
    }

    public VoxelType    type;
    public Vector3      position;
    public bool         isActive;
    public Voxel(VoxelType type, Vector3 position, bool isActive)
    {
        this.type = type;
        this.position = position;
        this.isActive = isActive;
    }
}
