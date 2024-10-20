using UnityEngine;

namespace Noise
{
    public class SimplexNoiseSource : INoiseSource
    {
        private PerlinProperties _Properties;

        public SimplexNoiseSource(PerlinProperties properties)
        {
            Properties = properties;
        }

        public PerlinProperties Properties  { get => _Properties; set => _Properties = value; }

        public float[,] Noise2D(int width, int height)
        {
            // TODO use new parameters to get a better noise output
            return SimplexNoise.SimplexNoise.Calc2D(width, height, Properties.Frequency);
        }

        public float[,,] Noise3D(int width, int height, int depth)
        {
            throw new System.NotImplementedException();
        }

    }

}