using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Resources;
using Game.Interaction;
using Game.Utilities;
using Vox3D.Engine;
using System;

namespace Game.Stages
{
    public class SelectionStage : GameStage
    {

        [SerializeField]
        private GameObject _SelectionStageHUD;
        private GameObject _SelectionStageHUD_Instance;

        private ProjectileResources _ProjectileTemplates;
        private Dictionary<uint, List<PlayerTower>> _TowersByPlayer;

        private int ReadyToProceed;

        public override void Initialize()
        {
            ReadyToProceed = 0;

            if (!IsInitialized)
            {
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

                _TowersByPlayer = new Dictionary<uint, List<PlayerTower>>();
                var towersInScene = FindObjectsOfType<PlayerTower>();

                foreach (var tower in towersInScene)
                {
                    var ownerID = tower.GetComponent<OwnedBy>().OwnerID;

                    if (!_TowersByPlayer.ContainsKey(ownerID))
                    {
                        _TowersByPlayer.Add(ownerID, new List<PlayerTower>());
                    }

                    _TowersByPlayer[ownerID].Add(tower);
                }

                IsInitialized = true;
                
                CmdModifyBudget_All(+30);

                RpcLoadHUD();
            }

            CmdModifyBudget_All(+30);

            if (!ReportStage.GameEnded)
            {

                RpcToggleHUD(true);
                GetComponent<StageManager>().RpcToggleAllLoadingScreens(false);
                RpcDeactivateDestructionColliders();
                RpcScaleWorld();
            }
        }

        public override void Deinitialize()
        {
            throw new System.NotImplementedException();
        }
        protected override void Run()
        {

        }

        [ClientRpc]
        private void RpcScaleWorld()
        {
            var world = FindObjectOfType<World>();

           // world.transform.position = UnityEngine.Camera.main.transform.position + new Vector3(-4.0f, -6f, 4.0f);
        }

        [ClientRpc]
        private void RpcLoadHUD()
        {
            if (_ProjectileTemplates is null)
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

            _SelectionStageHUD_Instance = Instantiate(_SelectionStageHUD, UnityEngine.Camera.main.transform);
            _SelectionStageHUD_Instance.GetComponent<SelectionHUDControllerVR>().ToggleDisplay(false);
        }

        [ClientRpc]
        private void RpcDeactivateDestructionColliders()
        {
            foreach(var chunk in FindObjectOfType<World>().Chunks.Values)
            {
                chunk.ToggleDestructionCollider(false);
            }
        }

        //TODO add cost to projectiles
        [Command(requiresAuthority = false)]
        public void CmdConfirmPurchase(uint playerID, string weaponName, uint cost)
        {
            foreach(var player in Players)
            {
                if(player.GetComponent<NetworkRoomPlayer>().netId == playerID)
                {
                    Debug.Log("Attempting to purchase " + weaponName);
                    Debug.Log("Budget: " + player.GetComponent<Player>().Budget + " - cost:  " + cost);

                    if (cost <= player.GetComponent<Player>().Budget)
                    {
                        var inventory = player.GetComponent<Player>().Inventory;

                        inventory.IncreaseItem(weaponName);

                        Debug.Log("Weapon ammo adjusted for weapon " + weaponName + " on player " + playerID + ". Total ammo of " + inventory.GetProjectile(weaponName));

                        CmdModifyBudget_Player(-Convert.ToInt32(cost), player.GetComponent<Player>());

                        GetComponent<ReportStage>().GetPlayerData(playerID).Data[ReportStage.ReportData.Budget_Spent] += cost;
                        GetComponent<ReportStage>().GetPlayerData(playerID).Data[ReportStage.ReportData.Ammo_Purchased] ++;

                    }
                }
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdModifyBudget_All(int amount)
        {
            foreach(var player in Players)
            {
                CmdModifyBudget_Player(amount, player.GetComponent<Player>());
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdModifyBudget_Player(int amount, Player player)
        {
            player.Budget += amount;
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
            _SelectionStageHUD_Instance.GetComponent<SelectionHUDControllerVR>().ToggleDisplay(active);
        }
    }

}