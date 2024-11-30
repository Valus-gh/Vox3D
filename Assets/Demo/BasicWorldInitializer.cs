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

        // Start is called before the first frame update
        void Start()
        {

            var model = Vox3D.JSON.JsonImporter.FromJSON("config");

            var manager = Vox3DManager.Instance();
            manager.Properties = new Vox3DProperties(model.World.WorldSize, model.World.ChunkSize, model.World.VoxelSize);
            manager.Properties.VoxelDefaultMaterial = Resources.Load("VoxelVertexColorMaterial", typeof(Material)) as Material; //TODO to Vox3dProperties
            manager.Properties.BiomeLookupTexture = Resources.Load("biome-lookup-128x128", typeof(Texture2D)) as Texture2D; //TODO to Vox3dProperties
            manager.Properties.WaterLevel = model.World.WaterHeight;

            PerlinProperties propsH = new PerlinProperties(
                seed: model.HeightMap.Seed,
                gain: model.HeightMap.Gain,
                redistribution: model.HeightMap.Redistribution,
                doReshape: model.HeightMap.Reshape,
                shapingFactor: model.HeightMap.ReshapeFactor
            );

            PerlinProperties propsM = new PerlinProperties(
                seed: model.MoistureMap.Seed,
                gain: model.MoistureMap.Gain,
                redistribution: model.MoistureMap.Redistribution,
                doReshape: model.MoistureMap.Reshape,
                shapingFactor: model.MoistureMap.ReshapeFactor
            );

            SimplexNoiseSource noiseSourceHeight = new SimplexNoiseSource(propsH);
            SimplexNoiseSource noiseSourceMoisture = new SimplexNoiseSource(propsM);

            manager.HNoiseSource = noiseSourceHeight;
            manager.MNoiseSource = noiseSourceMoisture;
            manager.World = Vox3DManager.MakeWorld();
            manager.World.HeightMap.MaxHeight = model.World.TerrainHeight;
            manager.World.MoistureMap.MaxHeight = model.World.TerrainHeight;

            manager.World.HeightMap.BakeTexture("Elevation");
            manager.World.MoistureMap.BakeTexture("Moisture");

            manager.World.PopulateWorld();
            manager.World.PopulateChunks();
            PriorityCallStack.Instance().Push(() => manager.World.GenerateGeometry(), 60);

        }

    }

}