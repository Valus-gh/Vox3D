using UnityEngine;

public struct PerlinProperties
{
    public int      Seed;
    public float    Frequency;
    public float    AmplitudeRatio;
    public float[]  Amplitude;
    public float    Redistribution;
    public float    ShapingFactor;

    public bool     FreqToWave;
    public bool     DoReshape;

    private float Distance_SquareBump(float x, float y)
    {
        return 1 - (1 - Mathf.Pow(x, 2)) * (1 - Mathf.Pow(y, 2));
    }

    public PerlinProperties(int seed, 
        float[] amplitude, 
        float amplitudeRatio    = 1, 
        float redistribution    = 1.0f, 
        bool freqToWave         = false, 
        float frequency         = 1.0f, 
        bool doReshape          = false, 
        float shapingFactor     = 0.0f)
    {
        Seed                    = seed;
        Amplitude               = amplitude;
        AmplitudeRatio          = amplitudeRatio;
        Redistribution          = redistribution;
        FreqToWave              = freqToWave;
        Frequency               = frequency;
        DoReshape               = doReshape;
        ShapingFactor           = shapingFactor;
    }

    public PerlinProperties(int seed,
        float amplitudeRatio    = 1,
        float redistribution    = 1.0f,
        bool freqToWave         = false,
        float frequency         = 1.0f,
        bool doReshape          = false,
        float shapingFactor     = 0.0f)
    {
        Seed                    = seed;
        Amplitude               = new float[3]{1.0f, 1.0f, 1.0f};
        AmplitudeRatio          = amplitudeRatio;
        Redistribution          = redistribution;
        FreqToWave              = freqToWave;
        Frequency               = frequency;
        DoReshape               = doReshape;
        ShapingFactor           = shapingFactor;
    }

}
