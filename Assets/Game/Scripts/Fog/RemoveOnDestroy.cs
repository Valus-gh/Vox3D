using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game.Fog.FischlWorks;

namespace Game.Fog
{
    public class RemoveOnDestroy : MonoBehaviour
    {
        public csFogWar Fow;
        public csFogWar.FogRevealer revealer;

        public void OnDestroy()
        {
            Fow.RemoveFogRevealer(Fow._FogRevealers.IndexOf(revealer));
        }

    }

}