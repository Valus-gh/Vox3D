using System.Collections.Generic;
using UnityEngine;

using Game.Fog.FischlWorks;
using Game.Networking;
using Game.Utilities;

namespace Game.Fog
{

    /// <summary>
    /// Looks for FoWRevelers in the scene and adds them to the fog of war calculations, or removes them when destroyed.
    /// </summary>
    [RequireComponent(typeof(FogInjector))]
    public class FindRevealers : MonoBehaviour
    {
        private List<int> ids = new List<int>();

        // Update is called once per frame
        void Update()
        {
            if (GetComponent<FogInjector>() is not null)
            {
                var revealers = FindObjectsByType<FoWRevealer>(FindObjectsSortMode.None);

                if (revealers is not null)
                {
                    foreach (var obj in revealers)
                    {
                        if (!ids.Contains(obj.GetInstanceID()) && !obj.Revealing)
                        {

                            if (obj.GetComponent<OwnedBy>())
                            {
                                var owner = obj.GetComponent<OwnedBy>().OwnerID;

                                if (owner == NetworkRoomManagerV3D.singleton.PlayerID)
                                {
                                    var revealer = GetComponent<FogInjector>().AddRevealer(obj.gameObject.transform, obj.Radius, true);

                                    var rod = obj.gameObject.AddComponent<RemoveOnDestroy>();
                                    rod.Fow = GetComponent<csFogWar>();
                                    rod.revealer = revealer;

                                    ids.Add(obj.GetInstanceID());
                                }
                                else
                                {
                                    //Destroy(obj.GetComponent<FoWRevealer>());
                                }
                            }
                        }
                    }
                }
            }
        }

    }

}