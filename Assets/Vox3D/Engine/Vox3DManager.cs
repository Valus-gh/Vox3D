using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Noise;

// TODO add moisture map to world, try to make shader with different biomes

namespace Vox3D {
    public class Vox3DManager
    {

        private static Vox3DManager _Instance;
        public static Vox3DManager Instance()
        {
            if (_Instance is null)
                _Instance = new Vox3DManager();

            return _Instance;
        }

        private Vox3DProperties     _Properties;
        private SimplexNoiseSource  _HNoiseSource;
        private SimplexNoiseSource  _MNoiseSource;
        private World               _World;
        public Vox3DProperties Properties       { get => _Properties; set => _Properties = value; }
        public SimplexNoiseSource HNoiseSource  { get => _HNoiseSource; set => _HNoiseSource = value; }
        public SimplexNoiseSource MNoiseSource  { get => _MNoiseSource; set => _MNoiseSource = value; }
        public World World                      { get => _World; set => _World = value; }

        public static World MakeWorld()
        {
            var manager     = Instance();
            var properties  = manager.Properties;
            var noiseSource = manager.HNoiseSource;

            GameObject worldObject  = new GameObject($"World_{properties.WorldSize}_{properties.ChunkSize}_{properties.VoxelSize}");
            World world             = worldObject.AddComponent<World>();

            if (world.TryInitialize()) return world;

            return null;
        }

    }

}