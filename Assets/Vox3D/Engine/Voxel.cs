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

        private VoxelType   _type;
        private Vector3     _position;
        private bool        _isActive;

        public VoxelType Type   { get => _type; set => _type = value; }
        public Vector3 Position { get => _position; set => _position = value; }
        public bool IsActive    { get => _isActive; set => _isActive = value; }

        public Voxel(VoxelType type, Vector3 position, bool isActive)
        {
            this._type      = type;
            this._position  = position;
            this._isActive  = isActive;
        }
    }

}
