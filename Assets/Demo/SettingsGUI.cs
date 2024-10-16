using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SettingsGUI : MonoBehaviour
{

    public float wsDefault = 2;
    public float csDefault = 16;
    public float vsDefault = 1;

    public string noiseSeed = "1111";
    public string noiseScale = "0.012";
    public string maxHeight = "0.2";

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 300, 330), "Settings");

        GUI.Box(new Rect(20, 30, 280, 23), $"World Size - {(int)wsDefault} chunks per world");
        wsDefault = GUI.HorizontalSlider(new Rect(20, 55, 280, 20), wsDefault, 1, 8);

        GUI.Box(new Rect(20, 85, 280, 23), $"Chunk Size - {(int)csDefault}x{(int)csDefault}x{(int)csDefault} total voxels");
        csDefault = GUI.HorizontalSlider(new Rect(20, 110, 280, 20), csDefault, 1, 32);

        GUI.Box(new Rect(20, 140, 280, 23), $"Voxel Size - voxels edges are {(int)vsDefault} units long");
        vsDefault = GUI.HorizontalSlider(new Rect(20, 165, 280, 20), vsDefault, 1, 16);

        GUI.Label(new Rect(20, 195, 100, 23), $"Seed: ");
        noiseSeed = GUI.TextField(new Rect(52, 195, 80, 23), noiseSeed);

        GUI.Label(new Rect(20, 223, 100, 23), $"Scale: ");
        noiseScale = GUI.TextField(new Rect(52, 223, 80, 23), noiseScale);

        GUI.Label(new Rect(20, 251, 100, 23), $"Max Height: ");
        maxHeight = GUI.TextField(new Rect(52, 251, 80, 23), maxHeight);

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

        var newSource = new Noise.SimplexNoiseSource(int.Parse(noiseSeed, CultureInfo.InvariantCulture), float.Parse(noiseScale, CultureInfo.InvariantCulture));

        int width = prop.WorldSize * prop.ChunkSize;
        int height = prop.WorldSize * prop.ChunkSize;
        var map = new Vox3D.HeightMap2D(width, height, float.Parse(maxHeight, CultureInfo.InvariantCulture), newSource);

        prop.World.HeightMap = map;

        RefreshGeometry();
    }
}
