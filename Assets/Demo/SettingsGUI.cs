using System.Globalization;
using UnityEngine;

public class SettingsGUI : MonoBehaviour
{

    private float wsDefault;
    private float csDefault;
    private float vsDefault;

    private string noiseSeed;
    private string noiseFrequency;
    private string maxHeight;

    private bool init = false;

    void OnGUI()
    {
        if (init is false)
        {

            var prop = Vox3D.Vox3DProperties.Instance();
            wsDefault = prop.World.WorldSize;
            csDefault = prop.World.ChunkSize;
            vsDefault = prop.World.VoxelSize;

            var source = (Noise.SimplexNoiseSource)prop.World.HeightMap.Source;
            var noiseProps = source.Properties;
            noiseSeed = $"{noiseProps.Seed}";
            noiseFrequency = $"{noiseProps.Frequency}";
            noiseFrequency = noiseFrequency.Replace(',', '.');
            maxHeight = $"{prop.World.HeightMap.MaxHeight}";

            init = true;
        }

        GUI.Box(new Rect(10, 10, 300, 330), "Settings");

        GUI.Box(new Rect(20, 30, 280, 23), $"World Size - {(int)wsDefault} chunks per world");
        wsDefault = GUI.HorizontalSlider(new Rect(20, 55, 280, 20), wsDefault, 1, 8);

        GUI.Box(new Rect(20, 85, 280, 23), $"Chunk Size - {(int)csDefault}x{(int)csDefault}x{(int)csDefault} total voxels");
        csDefault = GUI.HorizontalSlider(new Rect(20, 110, 280, 20), csDefault, 1, 32);

        GUI.Box(new Rect(20, 140, 280, 23), $"Voxel Size - voxels edges are {(int)vsDefault} units long");
        vsDefault = GUI.HorizontalSlider(new Rect(20, 165, 280, 20), vsDefault, 1, 16);

        GUI.Label(new Rect(20, 195, 100, 23), $"Seed: ");
        noiseSeed = GUI.TextField(new Rect(57, 195, 80, 23), noiseSeed);

        GUI.Label(new Rect(20, 223, 100, 56), $"Frequency: ");
        noiseFrequency = GUI.TextField(new Rect(88, 223, 80, 23), noiseFrequency);

        GUI.Label(new Rect(20, 251, 100, 62), $"Max Height: ");
        maxHeight = GUI.TextField(new Rect(94, 251, 80, 23), maxHeight);

        if(GUI.Button(new Rect(20, 281, 200, 20), "Refresh Map & Geometry"))
        {
            RefreshHeightmap();
        }

    }

    private void RefreshGeometry()
    {
        var prop = Vox3D.Vox3DProperties.Instance();
        prop.WorldSize = (int)wsDefault;
        prop.ChunkSize = (int)csDefault;
        prop.VoxelSize = (int)vsDefault;

        prop.World.WorldSize = prop.WorldSize;
        prop.World.ChunkSize = prop.ChunkSize;
        prop.World.VoxelSize = prop.VoxelSize;

        prop.World.PurgeWorld();
        prop.World.PopulateWorld();
    }

    private void RefreshHeightmap()
    {
        var prop = Vox3D.Vox3DProperties.Instance();

        prop.WorldSize = (int)wsDefault;
        prop.ChunkSize = (int)csDefault;
        prop.VoxelSize = (int)vsDefault;
        int width = prop.WorldSize * prop.ChunkSize;
        int height = prop.WorldSize * prop.ChunkSize;

        PerlinProperties props = new PerlinProperties(
            seed: int.Parse(noiseSeed, CultureInfo.InvariantCulture),
            frequency: float.Parse(noiseFrequency, CultureInfo.InvariantCulture));

        var source = (Noise.SimplexNoiseSource)prop.World.HeightMap.Source;
        source.Properties = props;

        prop.World.HeightMap.Remap(width, height, float.Parse(maxHeight, CultureInfo.InvariantCulture));

        RefreshGeometry();
    }
}
