using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Stages;
using Game.Interaction;

namespace Game
{

    public class Inventory : NetworkBehaviour
    {
        private static uint MAX_PROJECTILES = 999;

        public readonly SyncDictionary<string, uint> Projectiles = new SyncDictionary<string, uint>();

        public override void OnStartServer()
        {
            base.OnStartServer();

            Projectiles["Basic"] = MAX_PROJECTILES;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            Projectiles.OnAdd += OnItemAdded;
            Projectiles.OnSet += OnItemChanged;
        }
        public override void OnStopClient()
        {
            base.OnStartClient();

            Projectiles.OnAdd -= OnItemAdded;
            Projectiles.OnSet -= OnItemChanged;
        }

        public uint GetProjectile(string key)
        {
            if (Projectiles.ContainsKey(key))
                return Projectiles[key];
            else
                return 0;
        }

        public void DecreaseItem(string key)
        {
            if (key == "Basic") return;

            if (Projectiles.ContainsKey(key) && Projectiles[key] > 0)
                Projectiles[key]--;
        }

        public void IncreaseItem(string key)
        {
            if (key == "Basic") return;

            if (!Projectiles.ContainsKey(key))
                Projectiles.Add(key, 1);
            else if (Projectiles[key] < MAX_PROJECTILES)
                Projectiles[key]++;
        }

        void OnItemAdded(string key)
        {
            OnItemChanged(key, 0);
        }

        void OnItemChanged(string key, uint oldValue)
        {
            var weaponBar = FindObjectOfType<WeaponBarHUDController>();

            if(weaponBar is not null)
                weaponBar.UpdateHUD(this);
        }

    }

}