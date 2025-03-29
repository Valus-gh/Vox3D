using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
public class OwnedBy : NetworkBehaviour
{
    [SyncVar]
    public uint OwnerID;
}
