using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Noise;

namespace Vox3D
{

    // TODO Normalize heightmap to [0,1]
    // TODO Switch voxel generation to ParallelVoxelBuilder
    // TODO Try very large world
    // 
    /// <summary>
    /// There is only once instance of this class at runtime. 
    /// Holds the global world properties used by most classes within the engine.
    /// </summary>
    public class Vox3DProperties
    {
        private int         _WorldSize; // How many chunks for each axis in a world
        private int         _ChunkSize; // How many voxels for each axis in a chunk
        private int         _VoxelSize; // The size of the edges of the voxels

        private Material    _VoxelDefaultMaterial;
        private Texture2D   _BiomeLookupTexture;
        private float       _WaterLevel;

        private SimplexNoiseSource _HNoiseSource;
        private SimplexNoiseSource _MNoiseSource;
        public Vox3DProperties()
        {

        }
        public Vox3DProperties(int worldSize, int chunkSize, int voxelSize)
        {
            _WorldSize = worldSize;
            _ChunkSize = chunkSize;
            _VoxelSize = voxelSize;
        }
        public int WorldSize                    { get => _WorldSize; set => _WorldSize = value; }
        public int ChunkSize                    { get => _ChunkSize; set => _ChunkSize = value; }
        public int VoxelSize                    { get => _VoxelSize; set => _VoxelSize = value; }
        public Material VoxelDefaultMaterial    { get => _VoxelDefaultMaterial; set => _VoxelDefaultMaterial = value; }
        public Texture2D BiomeLookupTexture     { get => _BiomeLookupTexture; set => _BiomeLookupTexture = value; }
        public float WaterLevel                 { get => _WaterLevel; set => _WaterLevel = value; }
        public SimplexNoiseSource HNoiseSource  { get => _HNoiseSource; set => _HNoiseSource = value; }
        public SimplexNoiseSource MNoiseSource  { get => _MNoiseSource; set => _MNoiseSource = value; }

        public void LoadFromXML(string pathToXml)
        {
            // TODO Get Properties from XML Parser to fill an object when this is created
        }

    }

}