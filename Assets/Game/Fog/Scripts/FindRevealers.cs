using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FischlWorks_FogWar;

[RequireComponent(typeof(FogInjector))]
public class FindRevealers : MonoBehaviour
{
    private List<int> ids = new List<int>();

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<FogInjector>() is not null)
        {
            var objects = FindObjectsByType<FoWRevealer>(FindObjectsSortMode.None);

            if (objects is not null)
            {
                foreach (var obj in objects)
                {
                    if (!ids.Contains(obj.GetInstanceID()) && !obj.Revealing)
                    {
                        var revealer = GetComponent<FogInjector>().AddRevealer(obj.gameObject.transform, obj.Radius, true);
                        ids.Add(obj.GetInstanceID());

                        var rod         = obj.gameObject.AddComponent<RemoveOnDestroy>();
                        rod.Fow         = GetComponent<csFogWar>();
                        rod.revealer    = revealer;
                    }
                }
            }

        }
    }

}

