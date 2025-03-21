using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class PlayerTower : NetworkBehaviour
{
    [SyncVar]
    public uint PlayerID;

}
