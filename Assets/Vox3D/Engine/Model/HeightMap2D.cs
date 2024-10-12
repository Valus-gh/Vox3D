using Noise;
using UnityEngine;

namespace Vox3D
{
    public class HeightMap2D
    {
        private INoiseSource    _source;
        private float[,]        _map;

        public INoiseSource Source  { get => _source; private set => _source = value; }
        public float[,] Map         { get => _map; private set => _map = value; }

        public HeightMap2D(int width, int height, INoiseSource source)
        {
            Source = source;
            
            Debug.Log($"GENERATING HeightMap2D[{width}, {height}]");
            Map = source.Noise2D(width, height);
        }

        public float GetValue(int x, int y)
        {
            if (x >= 0 && x < _map.GetLength(0) && 
                y >= 0 && y < _map.GetLength(1))
                return Map[x, y];
            else
                return 0; // Index out of bounds, default value.
        }

    }

}