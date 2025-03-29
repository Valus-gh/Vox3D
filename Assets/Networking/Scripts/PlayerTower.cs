using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class PlayerTower : NetworkBehaviour
{

    public bool CanFire = false;

    private TowerControlsMultiplayer TowerControls;

    public void Start()
    {
        TowerControls = GetComponentInChildren<TowerControlsMultiplayer>();
    }

    public void Update()
    {
        if(!TowerControls.enabled && CanFire)
            TowerControls.enabled = true;
    }


}
