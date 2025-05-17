using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Resources;
using Game.Interaction;
using Game.Utilities;

//TODO use list <keyvaluepair> instead of dictionary to hold towers

namespace Game.Stages
{
    public class SelectionStage : GameStage
    {

        [SerializeField]
        private GameObject _SelectionStageHUD;
        private GameObject _SelectionStageHUD_Instance;

        private ProjectileResources _ProjectileTemplates;
        private Dictionary<uint, PlayerTower> _Towers;

        private int ReadyToProceed;

        public override void Initialize()
        {

            ReadyToProceed = 0;

            if (!IsInitialized)
            {
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

                _Towers = new Dictionary<uint, PlayerTower>();
                var towers = FindObjectsOfType<PlayerTower>();

                foreach (var tower in towers)
                {
                    var ownerID = tower.GetComponent<OwnedBy>().OwnerID;
                    _Towers.Add(ownerID, tower);
                }
                IsInitialized = true;

                RpcLoadHUD();
            }

            RpcToggleHUD(true);
            GetComponent<StageManager>().RpcToggleAllLoadingScreens(false);

        }

        public override void Deinitialize()
        {
            throw new System.NotImplementedException();
        }
        protected override void Run()
        {
   
        }

        [ClientRpc]
        private void RpcLoadHUD() 
        {
            if (_ProjectileTemplates is null)
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");
            
            _SelectionStageHUD_Instance = Instantiate(_SelectionStageHUD, this.transform);
            _SelectionStageHUD_Instance.GetComponent<SelectionHUDController>().InitializeHUD(_ProjectileTemplates);
            _SelectionStageHUD_Instance.GetComponent<SelectionHUDController>().ToggleDisplay(false);
        }

        //TODO add cost to projectiles
        [Command(requiresAuthority = false)]
        public void CmdConfirmPurchase(uint playerID, string weaponName, uint cost)
        {
            foreach(var player in Players)
            {
                if(player.GetComponent<NetworkRoomPlayer>().netId == playerID)
                {
                    if (cost <= player.GetComponent<Player>().Budget)
                    {
                        var projectileInventory = player.GetComponent<Player>().Inventory.Projectiles;

                        if (!projectileInventory.ContainsKey(weaponName))
                        {
                            projectileInventory.Add(weaponName, 1);
                        }
                        else
                        {
                            var weaponAmmo = projectileInventory[weaponName];
                            projectileInventory[weaponName] = weaponAmmo + 1;
                        }

                        Debug.Log("Weapon ammo adjusted for weapon " + weaponName + " on player " + playerID + ". Total ammo of " + projectileInventory[weaponName]);
                    }
                }
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdReadyToProceed()
        {
            ReadyToProceed++;

            if (ReadyToProceed >= Players.Count)
            {
                RpcToggleHUD(false);

                IsComplete = true;

                GetComponent<StageManager>().RpcToggleAllLoadingScreens(true);
            }
        }

        [ClientRpc]
        private void RpcToggleHUD(bool active)
        {
            ToggleHUD(active);
        }

        public void ToggleHUD(bool active)
        {
            _SelectionStageHUD_Instance.GetComponent<SelectionHUDController>().ToggleDisplay(active);
        }
    }

}