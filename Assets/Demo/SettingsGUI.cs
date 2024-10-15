using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsGUI : MonoBehaviour
{

    public float wsDefault = 2;
    public float csDefault = 16;
    public float vsDefault = 1;

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 310, 10, 300, 200), "Settings");

        GUI.Box(new Rect(Screen.width - 300, 30, 280, 23), $"World Size - {(int)wsDefault} chunks per world");
        wsDefault = GUI.HorizontalSlider(new Rect(Screen.width - 300, 55, 280, 20), wsDefault, 1, 8);

        GUI.Box(new Rect(Screen.width - 300, 85, 280, 23), $"Chunk Size - {(int)csDefault}x{(int)csDefault}x{(int)csDefault} total voxels");
        csDefault = GUI.HorizontalSlider(new Rect(Screen.width - 300, 110, 280, 20), csDefault, 1, 128);

        GUI.Box(new Rect(Screen.width - 300, 140, 280, 23), $"Voxel Size - voxels edges are {(int)vsDefault} units long");
        vsDefault = GUI.HorizontalSlider(new Rect(Screen.width - 300, 165, 280, 20), vsDefault, 1, 16);

        
    }

}
