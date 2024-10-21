using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vox3D;
using Noise;

namespace Demo
{
    public class BasicWorldInitializer : MonoBehaviour
    {

        private Vox3DProperties properties;
        private World world;

        public int worldSize;
        public int chunkSize;
        public int voxelSize;

        public float maxHeight;

        public int noiseSeed;
        public float noiseGain;
        public float noiseRedistribution;
        public float reshapingFactor;
        public bool reshape;

        // Start is called before the first frame update
        void Start()
        {
            properties = Vox3DProperties.Instance();
            properties.WorldSize = worldSize;
            properties.ChunkSize = chunkSize;
            properties.VoxelSize = voxelSize;

            // Create map from simplex

            int width = worldSize * chunkSize;
            int height = worldSize * chunkSize;

            PerlinProperties props = new PerlinProperties(
                seed: noiseSeed,
                gain: noiseGain,
                redistribution: noiseRedistribution,
                doReshape: reshape,
                shapingFactor: reshapingFactor
                );

            SimplexNoiseSource noiseSource = new SimplexNoiseSource(props);
            HeightMap2D map = new HeightMap2D(width, height, maxHeight, noiseSource);

            // Create gameobject and attach script, then set map

            GameObject worldObject = new GameObject($"World_{width}x{height}");
            World world = worldObject.AddComponent<World>();

            world.HeightMap = map;

            properties.World = world;
            properties.VoxelDefaultMaterial = Resources.Load("DefaultMaterial", typeof(Material)) as Material;

        }

    }

}