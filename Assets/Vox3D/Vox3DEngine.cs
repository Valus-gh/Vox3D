using Vox3D.Noise;
using System.Collections.Generic;
using UnityEngine;

using Vox3D.Engine;

namespace Vox3D {

    public static class Vox3DEngine
    {
        public static Dictionary<string, World> Worlds;

        static Vox3DEngine()
        {
            Worlds = new Dictionary<string, World>();
        }

        public static World GetWorld(string ID)
        {
            return Worlds.ContainsKey(ID) ? Worlds[ID] : null;
        }

        public static World FromJSON(string path, bool generateMaps = false)
        {
            var model = Vox3D.JSON.JsonImporter<JSON.Vox3DModel>.FromJSON(path);       

            return FromModel(model, generateMaps);
        }

        /// <summary>
        /// Takes the necessary properties from a Vox3DModel and loads them from Resources.
        /// Creates a world gameobject and component, sets the relevant fields and adds it to the 
        /// dictionary of worlds, with its ID as a key.
        /// If the GenerateMaps flag is set, map generation is also attempted.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static World FromModel(JSON.Vox3DModel model, bool generateMaps = false)
        {
            var properties = new Vox3DProperties(model.World.WorldSize, model.World.ChunkSize, model.World.VoxelSize);

            // Shading Properties

            properties.VoxelDefaultMaterial = Resources.Load(model.Shading.Material, typeof(Material)) as Material;
            properties.BiomeLookupTexture = Resources.Load(model.Shading.BiomeLookup, typeof(Texture2D)) as Texture2D;
            properties.WaterLevel = model.World.WaterHeight;

            // Noise Properties

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

            properties.HNoiseSource = noiseSourceHeight;
            properties.MNoiseSource = noiseSourceMoisture;

            // Create World Gameobject and script
            string worldID = System.Guid.NewGuid().ToString();

            GameObject worldObject = new GameObject($"World_{worldID}_{properties.WorldSize}_{properties.ChunkSize}_{properties.VoxelSize}");

            World world         = worldObject.AddComponent<World>();
            world.ID            = worldID;
            world.Properties    = properties;

            world.GenerateMaps();
            world.HeightMap.MaxHeight   = model.World.TerrainHeight;
            world.MoistureMap.MaxHeight = model.World.TerrainHeight;

            Worlds.Add(worldID, world);

            return world;
        }

    }

}