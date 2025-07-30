using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vox3D.Engine;
using Vox3D.Noise;

using Game;

namespace Demo
{
    public class BasicWorldInitializer : MonoBehaviour
    {

        public GameObject tower;

        // Start is called before the first frame update
        void Start()
        {

            //######################## IMPORT FROM JSON ########################//

            var world = Vox3D.Vox3DEngine.FromJSON("config", Vector3.zero, true);

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
            
          /*  PriorityCallStack.Instance().Push(() => {

                var tower = GameObject.Find("Tower_Simple");
                var towers = GameObject.FindObjectsByType<TowerControlsKeyboard>(FindObjectsSortMode.None);
                foreach(var t in towers)
                {
                    t.transform.parent.localScale = Vector3.one * (0.5f * world.Properties.VoxelSize);
                }

                GameObject.Find("FogManager").GetComponent<FogManager>().AttachFoW(world);

            }, 350);*/
            
            //######################## INSTANTIATE FOG ########################//

        }

    }

}