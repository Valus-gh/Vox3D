using UnityEngine;

public struct PerlinProperties
{
    public int      Seed;
    public float    Gain;
    public float[]  Amplitude;
    public float    Redistribution;
    public float    ShapingFactor;
    public bool     DoReshape;

    public float Distance_SquareBump(float x, float y)
    {
        return 1.0f - (1.0f - Mathf.Pow(x, 2)) * (1.0f - Mathf.Pow(y, 2));
    }

    public PerlinProperties(
        int seed, 
        float[] amplitude, 
        float gain              = 1, 
        float redistribution    = 1.0f, 
        bool doReshape          = false, 
        float shapingFactor     = 0.0f)
    {
        Seed                    = seed;
        Amplitude               = amplitude;
        Gain                    = gain;
        Redistribution          = redistribution;
        DoReshape               = doReshape;
        ShapingFactor           = shapingFactor;
    }

    public PerlinProperties(
        int seed,
        float gain              = 1,
        float redistribution    = 1.0f,
        bool doReshape          = false,
        float shapingFactor     = 0.0f)
    {
        Seed                    = seed;
        Amplitude               = new float[0];
        Gain                    = gain;
        Redistribution          = redistribution;
        DoReshape               = doReshape;
        ShapingFactor           = shapingFactor;
    }

}
