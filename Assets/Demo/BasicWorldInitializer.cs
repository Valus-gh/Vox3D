using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vox3D;
using Noise;

namespace Demo
{
    public class BasicWorldInitializer : MonoBehaviour
    {
        // TODO Move Texture Loading to Properties.  
        // TODO Move Material loading to Properties. 
        // TODO Write Shader using mesh vertex colors. 
        // TODO Separate job start and job end.
        public GameObject tower;
        public GameObject fowManager;


        // Start is called before the first frame update
        void Start()
        {

            //######################## IMPORT FROM JSON ########################//

            var world = Vox3DEngine.FromJSON("config", true);

            world.HeightMap.BakeTexture("Elevation");
            world.MoistureMap.BakeTexture("Moisture");

            //######################## IMPORT FROM JSON ########################//

            //######################## INSTANTIATE WORLD ########################//

            world.PopulateWorld();
            world.PopulateChunks();
            PriorityCallStack.Instance().Push(() => world.GenerateGeometry(), 60);
            
            PriorityCallStack.Instance().Push(() => {
                List<Vector3> locations = new List<Vector3>();
                locations = TowerLocator.GenerateTowerLocations(world, 4);
                locations.ForEach((l) => Instantiate(tower, l, Quaternion.identity, this.transform));
            }, 300);

            //######################## INSTANTIATE WORLD ########################//

            //######################## INSTANTIATE FOG ########################//
            /*
            PriorityCallStack.Instance().Push(() => {

                fowManager.GetComponent<FischlWorks_FogWar.csFogWar>().unitScale = world.Properties.VoxelSize;
                var fow = Instantiate(fowManager);
                var tower = GameObject.Find("Tower_Simple");
                var towers = GameObject.FindObjectsByType<TowerControlsKeyboard>(FindObjectsSortMode.None);
                foreach(var t in towers)
                {
                    t.transform.parent.localScale = Vector3.one * (0.5f * world.Properties.VoxelSize);
                }
                fow.GetComponent<FischlWorks_FogWar.csFogWar>().AddFogRevealer(new FischlWorks_FogWar.csFogWar.FogRevealer(tower.transform, 10, true));

                var fowInstance = fow.GetComponent<FowManager>();

                foreach(Chunk c in world.Chunks.Values)
                {
                    fowInstance.ApplyToChunk(c);
                }

                GetComponent<FindProjectiles>().Fow = fow.GetComponent<FischlWorks_FogWar.csFogWar>();

            }, 350);
            */
            //######################## INSTANTIATE FOG ########################//

        }

    }

}