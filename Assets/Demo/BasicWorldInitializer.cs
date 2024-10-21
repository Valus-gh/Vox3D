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
            var manager = Vox3DManager.Instance();
            manager.Properties = new Vox3DProperties(worldSize, chunkSize, voxelSize);
            manager.Properties.VoxelDefaultMaterial = Resources.Load("DefaultMaterial", typeof(Material)) as Material;

            PerlinProperties props = new PerlinProperties(
                seed: noiseSeed,
                gain: noiseGain,
                redistribution: noiseRedistribution,
                doReshape: reshape,
                shapingFactor: reshapingFactor
                );

            SimplexNoiseSource noiseSource = new SimplexNoiseSource(props);

            manager.NoiseSource = noiseSource;
            manager.World = Vox3DManager.MakeWorld();
            manager.World.HeightMap.MaxHeight = maxHeight;

            manager.World.PopulateWorld();
            manager.World.PopulateChunks();
            manager.World.GenerateGeometry();

        }

    }

}