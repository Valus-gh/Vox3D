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

        public int seedHeight;//TODO to Vox3dProperties
        public float gainHeight;//TODO to Vox3dProperties
        public float redistributionHeight;//TODO to Vox3dProperties
        public float reshapeFactorHeight;//TODO to Vox3dProperties
        public int seedMoisture;//TODO to Vox3dProperties
        public float gainMoisture;//TODO to Vox3dProperties
        public float redistributionMoisture;//TODO to Vox3dProperties
        public float reshapeFactorMoisture;//TODO to Vox3dProperties
        public bool reshape;//TODO to Vox3dProperties

        // Start is called before the first frame update
        void Start()
        {
            var manager = Vox3DManager.Instance();
            manager.Properties = new Vox3DProperties(worldSize, chunkSize, voxelSize);
            manager.Properties.VoxelDefaultMaterial = Resources.Load("VoxelVertexColorMaterial", typeof(Material)) as Material; //TODO to Vox3dProperties
            manager.Properties.BiomeLookupTexture = Resources.Load("biome-lookup-128x128", typeof(Texture2D)) as Texture2D; //TODO to Vox3dProperties

            PerlinProperties propsH = new PerlinProperties(
                seed: seedHeight,
                gain: gainHeight,
                redistribution: redistributionHeight,
                doReshape: reshape,
                shapingFactor: reshapeFactorHeight
            );

            PerlinProperties propsM = new PerlinProperties(
                seed: seedMoisture,
                gain: gainMoisture,
                redistribution: redistributionMoisture,
                doReshape: reshape,
                shapingFactor: reshapeFactorMoisture
            );

            SimplexNoiseSource noiseSourceHeight = new SimplexNoiseSource(propsH);
            SimplexNoiseSource noiseSourceMoisture = new SimplexNoiseSource(propsM);

            manager.HNoiseSource = noiseSourceHeight;
            manager.MNoiseSource = noiseSourceMoisture;
            manager.World = Vox3DManager.MakeWorld();
            manager.World.HeightMap.MaxHeight = maxHeight;
            manager.World.MoistureMap.MaxHeight = maxHeight;

            manager.World.HeightMap.BakeTexture("Elevation");
            manager.World.MoistureMap.BakeTexture("Moisture");

            manager.World.PopulateWorld();
            manager.World.PopulateChunks();
            manager.World.GenerateGeometry();

        }

    }

}