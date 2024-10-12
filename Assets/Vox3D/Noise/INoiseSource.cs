namespace Noise
{
    public interface INoiseSource
    {
        float[,]    Noise2D(int width, int height);
        float[,,]   Noise3D(int width, int height, int depth);
    }

}