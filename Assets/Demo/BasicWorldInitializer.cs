using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vox3D;
using Noise;

namespace Demo
{
    public class BasicWorldInitializer : MonoBehaviour
    {
        // TODO Move Texture Loading to Properties.  
        // TODO Move Material loading to Properties. 
        // TODO Write Shader using mesh vertex colors. 
        // TODO Separate job start and job end.
        private Vox3DProperties properties;
        private World world;

        public int worldSize;//TODO to Vox3dProperties
        public int chunkSize;//TODO to Vox3dProperties
        public int voxelSize;//TODO to Vox3dProperties

        public float maxHeight;//TODO to Vox3dProperties

        public int noiseSeed;//TODO to Vox3dProperties
        public float noiseGain;//TODO to Vox3dProperties
        public float noiseRedistribution;//TODO to Vox3dProperties
        public float reshapingFactor;//TODO to Vox3dProperties
        public bool reshape;//TODO to Vox3dProperties

        // Start is called before the first frame update
        void Start()
        {
            var manager = Vox3DManager.Instance();
            manager.Properties = new Vox3DProperties(worldSize, chunkSize, voxelSize);
            manager.Properties.VoxelDefaultMaterial = Resources.Load("DefaultMaterial", typeof(Material)) as Material; //TODO to Vox3dProperties
            manager.Properties.BiomeLookupTexture = Resources.Load("biome-lookup-128x128", typeof(Material)) as Texture2D; //TODO to Vox3dProperties

            PerlinProperties props = new PerlinProperties(
                seed: noiseSeed,
                gain: noiseGain,
                redistribution: noiseRedistribution,
                doReshape: reshape,
                shapingFactor: reshapingFactor
                );

            SimplexNoiseSource noiseSourceHeight = new SimplexNoiseSource(props);
            SimplexNoiseSource noiseSourceMoisture = new SimplexNoiseSource(props);

            manager.HNoiseSource = noiseSourceHeight;
            manager.MNoiseSource = noiseSourceMoisture;
            manager.World = Vox3DManager.MakeWorld();
            manager.World.HeightMap.MaxHeight = maxHeight;
            manager.World.MoistureMap.MaxHeight = maxHeight;

            manager.World.PopulateWorld();
            manager.World.PopulateChunks();
            manager.World.GenerateGeometry();

        }

    }

}