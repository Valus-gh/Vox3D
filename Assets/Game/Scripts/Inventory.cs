using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Stages;

namespace Game
{

    public class Inventory : NetworkBehaviour
    {
        public readonly SyncDictionary<string, uint> Projectiles = new SyncDictionary<string, uint>();

        public override void OnStartServer()
        {
            base.OnStartServer();

            Projectiles["Basic"] = 999;
        }

    }

}