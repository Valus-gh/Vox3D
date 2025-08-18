using UnityEngine;

using Vox3D.Engine;
using Game.Fog.FischlWorks;
using System.Collections.Generic;

namespace Game.Fog
{
    /// <summary>
    /// This class is used to "inject" the fog onto a loaded world. 
    /// It uses a custom-built shader/material to take data from the 2D fogplane and applying it to the 3D terrain. 
    /// NOTE: The shader used for the material was originally made in ShaderGraph, then ported to normal shader.
    /// The reason is that in VR, ShaderGraphs do not seem to be supported, and I required additional parameters and flags in order to render to both eyes.
    /// See Assets -> Game -> Prefabs - Fog -> FogVertexColorShader_NoGraph.shader
    /// </summary>
    [RequireComponent(typeof(csFogWar))]
    [RequireComponent(typeof(Material))]
    public class FogInjector : MonoBehaviour
    {
        private World World;

        private csFogWar FoW;
        private Texture2D FogTexture;

        [SerializeField]
        private Material FogVertexColorMaterial;

        void Start()
        {
            World = transform.parent.GetComponent<World>();
            FoW = GetComponent<csFogWar>();
            FogTexture = FoW.FogPlaneTextureLerpTarget;

            FogVertexColorMaterial.SetTexture("_FogTexture", FogTexture);
            FogVertexColorMaterial.SetFloat("_WorldSize", World.Properties.WorldSize);
            FogVertexColorMaterial.SetFloat("_ChunkSize", World.Properties.ChunkSize);
            FogVertexColorMaterial.SetFloat("_VoxelSize", World.Properties.VoxelSize);
            FogVertexColorMaterial.SetVector("_MinPosition", World.transform.position);

            // This is needed to reapply vertex colors after changing material. Not yet sure why it's needed.
            RefreshVertexColors();
        }

        void Update()
        {
            FogVertexColorMaterial.SetVector("_MinPosition", World.transform.position);
        }

        private void RefreshVertexColors()
        {
            List<Color> colors = new List<Color>();

            foreach (Chunk chunk in World.Chunks.Values)
            {
                if (chunk.MeshFilter.sharedMesh is null) continue;

                chunk.MeshFilter.sharedMesh.GetColors(colors);
                chunk.MeshRenderer.material = FogVertexColorMaterial;
                chunk.MeshFilter.sharedMesh.SetColors(colors);
            }
        }

        public csFogWar.FogRevealer AddRevealer(Transform revealer, int radius, bool updateOnlyOnMove)
        {
            var rev = new csFogWar.FogRevealer(revealer, radius, updateOnlyOnMove);
            FoW.AddFogRevealer(rev);
            return rev;
        }


    }

}