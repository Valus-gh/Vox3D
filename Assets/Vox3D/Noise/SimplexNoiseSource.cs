using UnityEngine;

using Simplex;

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
            float[,] elevation = new float[width, height];

            //e = SimplexNoise.SimplexNoise.Calc2D(width, height, Properties.Frequency);

            if(_Properties.Amplitude.Length == 0)
            {
                _Properties.Amplitude = new float[6];

                for(int i = 0; i < 6; i++)
                {
                    if (i == 0) _Properties.Amplitude[i] = 1.0f;
                    else _Properties.Amplitude[i] = _Properties.Amplitude[i - 1] * _Properties.Gain;
                }
            }

            SimplexNoise.Seed = _Properties.Seed;

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    float e = 0.0f;
                    float[] amplitude = _Properties.Amplitude;
                    float resize = 0.0f;

                    // Basic noise calculation with octaves
                    for(int amp = 0; amp < amplitude.Length; amp++)
                    {
                        float f = (Mathf.Pow(2, amp) / 100.0f);

                        e = e + amplitude[amp] * ((SimplexNoise.CalcPixel2D(x, y, f) + 1) / 2);
                        resize += amplitude[amp];

                        // Adjust seed to get independent values
                        SimplexNoise.Seed = _Properties.Seed + amp; 

                    }

                    SimplexNoise.Seed = _Properties.Seed;

                    // Resizing to fit [0, 1]
                    e /= resize;

                    // Applying redistribution factor
                    e = Mathf.Pow(e, _Properties.Redistribution);


                    // Reshape the noise to the shape of an island
                    if (_Properties.DoReshape)
                    {
                        var dx = ((2.0f * x) / width) - 1.0f;
                        var dy = ((2.0f * y) / height) - 1.0f;

                        var d = _Properties.Distance_SquareBump(dx, dy);

                        e = Mathf.Lerp(e, 1 - d, _Properties.ShapingFactor);
                    }

                    elevation[x, y] = e;

                }
            }

            return elevation;
        }

        public float[,,] Noise3D(int width, int height, int depth)
        {
            throw new System.NotImplementedException();
        }

    }

}