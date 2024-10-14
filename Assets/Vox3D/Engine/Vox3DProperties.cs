using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vox3D
{
    /// <summary>
    /// There is only once instance of this class at runtime. 
    /// Holds the global world properties used by most classes within the engine.
    /// </summary>
    public class Vox3DProperties
    {
        private static Vox3DProperties _instance;
        public static Vox3DProperties Instance()
        {
            if (_instance is null)
                _instance = new Vox3DProperties();

            return _instance;
        }

        private int         _worldSize = -1; // How many chunks for each axis in a world
        private int         _chunkSize = -1; // How many voxels for each axis in a chunk
        private int         _voxelSize = -1; // The size of the edges of the voxels

        private World       _world;

        private Material    _voxelDefaultMaterial;

        public int WorldSize    { get => _worldSize; set => _worldSize = value; }
        public int ChunkSize    { get => _chunkSize; set => _chunkSize = value; }
        public int VoxelSize    { get => _voxelSize; set => _voxelSize = value; }
        public World World      { get => _world; set => _world = value; }
        public Material VoxelDefaultMaterial { get => _voxelDefaultMaterial; set => _voxelDefaultMaterial = value; }

        public void LoadFromXML(string pathToXml)
        {
            // TODO Get Properties from XML Parser to fill an object when this is created
        }

        public bool IsReady()
        {
            return _worldSize >= 0 || _chunkSize >= 0 || _voxelSize >= 0;
        }

    }

}