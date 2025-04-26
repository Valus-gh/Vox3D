using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace Game.Utilities
{
    public class OwnedBy : NetworkBehaviour
    {
        [SyncVar]
        public uint OwnerID;
    }

}