namespace Vox3D.JSON {

    [System.Serializable]
    public struct Vox3DModel
    {
        [System.Serializable]
        public struct WorldModel
        {
            public int      WorldSize;
            public int      ChunkSize;
            public int      VoxelSize;
            public float    TerrainHeight;
            public float    WaterHeight;

            public WorldModel(int worldSize, int chunkSize, int voxelSize, float terrainHeight, float waterHeight)
            {
                WorldSize = worldSize;
                ChunkSize = chunkSize;
                VoxelSize = voxelSize;
                TerrainHeight = terrainHeight;
                WaterHeight = waterHeight;
            }
        }
        [System.Serializable]
        public struct NoiseModel
        {
            public string   NoiseType;
            public int      Seed;
            public float    Gain;
            public float    Redistribution;
            public bool     Reshape;
            public float    ReshapeFactor;


            public NoiseModel(string noiseType, int seed, float gain, float redistribution, bool reshape, float reshapeFactor)
            {
                NoiseType = noiseType;
                Seed = seed;
                Gain = gain;
                Redistribution = redistribution;
                Reshape = reshape;
                ReshapeFactor = reshapeFactor;
            }
        }
        [System.Serializable]
        public struct ShadingModel
        {
            public string   Material;
            public string   BiomeLookup;

            public ShadingModel(string material, string biomeLookup)
            {
                Material = material;
                BiomeLookup = biomeLookup;
            }
        }

        public WorldModel   World;
        public NoiseModel   HeightMap;
        public NoiseModel   MoistureMap;
        public ShadingModel Shading;

        public Vox3DModel(WorldModel world, NoiseModel heightMap, NoiseModel moistureMap, ShadingModel shading)
        {
            World = world;
            HeightMap = heightMap;
            MoistureMap = moistureMap;
            Shading = shading;
        }

        public override string ToString()
        {
            return
                $"{World.WorldSize}-{World.WorldSize}-{World.WorldSize}\n" +
                $"{HeightMap.Redistribution}-{HeightMap.Gain}-{HeightMap.NoiseType}-{HeightMap.Seed}\n" +
                $"{MoistureMap.Redistribution}-{MoistureMap.Gain}-{MoistureMap.NoiseType}-{MoistureMap.Seed}\n";
        }
    }

}