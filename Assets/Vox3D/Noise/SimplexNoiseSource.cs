using UnityEngine;

namespace Noise
{
    public class SimplexNoiseSource : INoiseSource
    {
        private int     _Seed;          // Seed for noise generation. If reused, leads to repetition.
        private float   _heightCutoff;  // Used to cut off noise values that are too high.
        private float   _Scale;         // Scale parameter in simplex, used to determine noise resolution.

        public SimplexNoiseSource(int seed, float heightCutoff, float scale)
        {
            Seed = seed;
            HeightCutoff = heightCutoff;
            Scale = scale;
        }

        public int Seed             { get => _Seed; set => _Seed = value; }
        public float HeightCutoff   { get => _heightCutoff; set => _heightCutoff = value; }
        public float Scale          { get => _Scale; set => _Scale = value; }

        public float[,] Noise2D(int width, int height)
        {
            return SimplexNoise.SimplexNoise.Calc2D(width, height, Scale);
        }

        public float[,,] Noise3D(int width, int height, int depth)
        {
            throw new System.NotImplementedException();
        }
    }

}