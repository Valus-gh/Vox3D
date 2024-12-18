using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FischlWorks_FogWar;

public class RemoveOnDestroy : MonoBehaviour
{
    public csFogWar Fow;
    public csFogWar.FogRevealer rev;

    public void OnDestroy()
    {
        Fow.RemoveFogRevealer(Fow._FogRevealers.IndexOf(rev));
    }

}
