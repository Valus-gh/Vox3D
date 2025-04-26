using UnityEngine;

using Vox3D.Engine;
using Game.Fog.FischlWorks;
using System.Collections.Generic;

namespace Game.Fog
{

    [RequireComponent(typeof(csFogWar))]
    [RequireComponent(typeof(Material))]
    public class FogInjector : MonoBehaviour
    {
        private World World;

        private csFogWar FoW;
        private Texture2D FogTexture;

        [SerializeField]
        private Material FogVertexColorMaterial;

        // Start is called before the first frame update
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

        // Update is called once per frame
        void Update()
        {
            FogVertexColorMaterial.SetVector("_MinPosition", World.transform.position);
        }

        private void RefreshVertexColors()
        {
            List<Color> colors = new List<Color>();

            foreach (Chunk chunk in World.Chunks.Values)
            {
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