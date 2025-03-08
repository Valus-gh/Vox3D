using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FischlWorks_FogWar;
using Vox3D; 

[RequireComponent(typeof(csFogWar))]
[RequireComponent(typeof(Material))]
public class FowManager : MonoBehaviour
{

    public csFogWar     Fog;
    public Texture2D    FogTexture;
    public Material     FogVertexColorMaterial;

    // Start is called before the first frame update
    void Start()
    {
        Fog         = GetComponent<csFogWar>();
        FogTexture  = Fog.FogPlaneTextureLerpTarget;

        var manager = Vox3DManager.Instance();

        FogVertexColorMaterial.SetTexture("_FogTexture", FogTexture);
        FogVertexColorMaterial.SetFloat("_WorldSize", manager.World.WorldSize);
        FogVertexColorMaterial.SetFloat("_ChunkSize", manager.World.ChunkSize);
        FogVertexColorMaterial.SetFloat("_VoxelSize", manager.World.VoxelSize);
        FogVertexColorMaterial.SetVector("_MinPosition", manager.World.transform.position);

        var midPointPosition = (manager.World.WorldSize * manager.World.ChunkSize * manager.World.VoxelSize) / 2.0f;
        Fog._LevelMidPoint.transform.position = manager.World.transform.position + new Vector3(midPointPosition, 0.0f, midPointPosition);

    }

    // Update is called once per frame
    void Update()
    {
        var manager = Vox3DManager.Instance();
        FogVertexColorMaterial.SetVector("_MinPosition", manager.World.transform.position);
    }

    // Needs to reapply vertex colors after changing material. Not sure why.
    public void ApplyToChunk(Chunk chunk)
    {
        List<Color> colors = new List<Color>();
        chunk.MeshFilter.sharedMesh.GetColors(colors);
        chunk.MeshRenderer.material = FogVertexColorMaterial;
        chunk.MeshFilter.sharedMesh.SetColors(colors);
    }

}
