using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Interaction;

namespace Game
{
    public class PlayerTower : NetworkBehaviour
    {
        public bool CanFire = false;

        private Player _Player;

        private TowerControlsMultiplayer _TowerControls;

        public Player Player { get => _Player; set => _Player = value; }
        public TowerControlsMultiplayer TowerControls { get => _TowerControls; private set => _TowerControls = value; }


        public void Start()
        {
            TowerControls = GetComponentInChildren<TowerControlsMultiplayer>();
        }

        public void Update()
        {
            if (!TowerControls.enabled && CanFire)
                TowerControls.enabled = true;
        }


    }

}