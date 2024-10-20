using Noise;
using UnityEngine;

namespace Vox3D
{
    public class HeightMap2D
    {
        private INoiseSource    _Source;
        private float[,]        _Map;
        private float           _MaxHeight;
        private int             _Width;
        private int             _Height;

        public INoiseSource Source  { get => _Source; private set => _Source = value; }
        public float[,] Map         { get => _Map; private set => _Map = value; }
        public float MaxHeight      { get => _MaxHeight; set => _MaxHeight = value; }
        public int Width            { get => _Width; set => _Width = value; }
        public int Height           { get => _Height; set => _Height = value; }

        public HeightMap2D(int width, int height, float maxHeight, INoiseSource source)
        {
            Source      = source;
            _MaxHeight  = maxHeight;

            Debug.Log($"GENERATING HeightMap2D[{width}, {height}]");

            Map = Source.Noise2D(width, height);
        }

        public void Remap()
        {
            Map = null;
            Map = Source.Noise2D(_Width, _Height);
        }

        public void Remap(int width, int height, float maxHeight)
        {
            _MaxHeight  = maxHeight;
            _Width      = width;
            _Height     = height;

            Map = null;
            Map = Source.Noise2D(width, height);
        }

        public float ValueAt(float x, float y)
        {
            int mapX = (int)x % _Map.GetLength(0);
            int mapY = (int)y % _Map.GetLength(1);

            if (mapX >= 0 && mapX < _Map.GetLength(0) &&
                mapY >= 0 && mapY < _Map.GetLength(1))
                return Map[mapX, mapY];
            else
                return 0; // Index out of bounds, default value.
        }

    }

}