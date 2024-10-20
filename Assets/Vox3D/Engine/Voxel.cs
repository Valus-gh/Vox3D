using UnityEngine;

namespace Vox3D
{
    /// <summary>
    /// Struct is required by NativeArray. Reference types are not allowed.
    /// </summary>
    public struct Voxel
    {
        public enum VoxelType
        {
            Air,
            Grass,
            Sand,
            Rock,
            Snow,
        }

        private VoxelType   _Type;
        private Vector3     _Position;
        private bool        _IsActive;

        public VoxelType Type   { get => _Type; set => _Type = value; }
        public Vector3 Position { get => _Position; set => _Position = value; }
        public bool IsActive    { get => _IsActive; set => _IsActive = value; }

        public Voxel(VoxelType type, Vector3 position, bool isActive)
        {
            this._Type      = type;
            this._Position  = position;
            this._IsActive  = isActive;
        }

        public override string ToString()
        {
            return $"Type: {_Type}\nPosition: {_Position}\nIsActive: {_IsActive}";
        }
    }

}
