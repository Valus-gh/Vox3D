using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vox3D;
using Noise;

namespace Demo
{
    public class BasicWorldInitializer : MonoBehaviour
    {

        private WorldProperties properties;
        private World world;

        public int worldSize;
        public int chunkSize;
        public int voxelSize;

        public int noiseSeed;
        public float heightCutoff;
        public float noiseScale;

        // Start is called before the first frame update
        void Start()
        {
            properties = WorldProperties.Instance();
            properties.WorldSize = worldSize;
            properties.ChunkSize = chunkSize;
            properties.VoxelSize = voxelSize;

            // Create map from simplex

            int width = worldSize * chunkSize;
            int height = worldSize * chunkSize;

            SimplexNoiseSource noiseSource = new SimplexNoiseSource(noiseSeed, heightCutoff, noiseScale);
            HeightMap2D map = new HeightMap2D(width, height, noiseSource);

            // Create gameobject and attach script, then set map

            GameObject worldObject = new GameObject($"World_{width}x{height}");
            World world = worldObject.AddComponent<World>();

            world.HeightMap = map;

        }

    }

}