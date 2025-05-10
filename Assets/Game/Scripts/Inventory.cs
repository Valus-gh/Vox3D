using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace Game
{

    public class Inventory : NetworkBehaviour
    {
        public readonly SyncDictionary<string, uint> Projectiles = new SyncDictionary<string, uint>();
    }

}