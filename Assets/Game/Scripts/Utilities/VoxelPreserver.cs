using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vox3D.Engine;

namespace Game.Utilities
{
    public class VoxelPreserver : MonoBehaviour
    {
        private float _Width;
        private float _Length;
        private float _Height;

        // Start is called before the first frame update
        void Start()
        {
            _Width = transform.lossyScale.x;
            _Length = transform.lossyScale.z;
            _Height = transform.lossyScale.y;
        }

        public void PreserveVoxels(bool preserve = true)
        {
            ChunkDestructionHandler.Instance().CollisionCube_Preserve(transform.position, _Width, _Length, _Height, transform.rotation, preserve);

        }
    }
}