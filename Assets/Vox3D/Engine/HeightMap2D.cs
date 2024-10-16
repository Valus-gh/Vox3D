using Noise;
using UnityEngine;

namespace Vox3D
{
    public class HeightMap2D
    {
        private INoiseSource    _source;
        private float[,]        _map;
        private float           _maxHeight;

        public INoiseSource Source  { get => _source; private set => _source = value; }
        public float[,] Map         { get => _map; private set => _map = value; }
        public float MaxHeight      { get => _maxHeight; set => _maxHeight = value; }

        public HeightMap2D(int width, int height, float maxHeight, INoiseSource source)
        {
            Source = source;
            _maxHeight = maxHeight;

            Debug.Log($"GENERATING HeightMap2D[{width}, {height}]");
            Map = source.Noise2D(width, height);
        }

        public float ValueAt(float x, float y)
        {
            int mapX = (int)x % _map.GetLength(0);
            int mapY = (int)y % _map.GetLength(1);

            if (mapX >= 0 && mapX < _map.GetLength(0) &&
                mapY >= 0 && mapY < _map.GetLength(1))
                return Map[mapX, mapY];
            else
                return 0; // Index out of bounds, default value.
        }

    }

}