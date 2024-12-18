using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FischlWorks_FogWar;

namespace Demo
{

    public class FindProjectiles : MonoBehaviour
    {

        public csFogWar Fow;

        private List<int> ids = new List<int>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Fow is not null)
            {
                var objects = FindObjectsByType<Projectile>(FindObjectsSortMode.None);
                if (objects is not null)
                {
                    foreach (var obj in objects)
                    {
                        if (!ids.Contains(obj.GetInstanceID()))
                        {
                            var rev = new csFogWar.FogRevealer(obj.gameObject.transform, 5, true);
                            var index = Fow.AddFogRevealer(rev);
                            ids.Add(obj.GetInstanceID());

                            var rod = obj.gameObject.AddComponent<RemoveOnDestroy>();
                            rod.Fow = Fow;
                            rod.rev = rev;
                        }
                    }
                }

            }
        }

    }

}