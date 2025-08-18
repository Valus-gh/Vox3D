using System.Collections.Generic;
using UnityEngine;

using Mirror;

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

        private Dictionary<uint, List<PlayerTower>> _TowersByPlayer;

        private int _ReadyToProceed;

        /// <summary>
        /// Set up the initial data needed to run the stage.
        /// Add the initial budget to each player, as well as additional budget at the start of each round
        /// </summary>
        public override void Initialize()
        {
            Debug.Log("Initializing SelectionStage");

            _ReadyToProceed = 0;

            #region One-time operations

            if (!IsInitialized)
            {
                // Add each tower to a list based on its owner

                _TowersByPlayer = new Dictionary<uint, List<PlayerTower>>();
                var towersInScene = FindObjectsOfType<PlayerTower>();

                foreach (var tower in towersInScene)
                {
                    var ownerID = tower.GetComponent<OwnedBy>().OwnerID;

                    if (!_TowersByPlayer.ContainsKey(ownerID))
                        _TowersByPlayer.Add(ownerID, new List<PlayerTower>());

                    _TowersByPlayer[ownerID].Add(tower);
                }

                IsInitialized = true;
                
                CmdModifyBudget_All(+30);

                RpcLoadHUD();
            }

            #endregion

            #region Each round

            CmdModifyBudget_All(+30);

            if (!ReportStage.GameEnded)
            {
                RpcToggleHUD(true);
                GetComponent<StageManager>().RpcToggleAllLoadingScreens(false);
                RpcDeactivateDestructionColliders();
            }
            #endregion
        }

        public override void Deinitialize()
        {
            Debug.Log("Deinitializing SelectionStage");
        }
        protected override void Run()
        {

        }

        /// <summary>
        /// Called by clients when attempting to make a purchase. 
        /// Verifies there is enough budget, and adjusts players' inventories accordingly.
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="weaponName"></param>
        /// <param name="cost"></param>
        [Command(requiresAuthority = false)]
        public void CmdConfirmPurchase(uint playerID, string weaponName, uint cost)
        {
            foreach(var player in Players)
            {
                if(player.GetComponent<Player>().netId == playerID)
                {
                    Debug.Log($"Player {playerID} attempting to purchase " + weaponName);
                    Debug.Log($"Budget: {player.GetComponent<Player>().Budget} - cost: {cost}");

                    if (cost <= player.GetComponent<Player>().Budget)
                    {
                        var inventory = player.GetComponent<Player>().Inventory;

                        inventory.IncreaseItem(weaponName);

                        Debug.Log($"Weapon ammo adjusted for weapon {weaponName} on player {playerID}. Total ammo of {inventory.GetProjectile(weaponName)}");

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
            _ReadyToProceed++;

            if (_ReadyToProceed >= Players.Count)
            {
                RpcToggleHUD(false);

                IsComplete = true;

                GetComponent<StageManager>().RpcToggleAllLoadingScreens(true);
            }
        }

        [ClientRpc]
        private void RpcDeactivateDestructionColliders()
        {
            foreach (var chunk in FindObjectOfType<World>().Chunks.Values)
            {
                chunk.ToggleDestructionCollider(false);
            }
        }

        [ClientRpc]
        private void RpcLoadHUD()
        {
            _SelectionStageHUD_Instance = Instantiate(_SelectionStageHUD, Camera.main.transform);
            _SelectionStageHUD_Instance.GetComponent<SelectionHUDControllerVR>().ToggleDisplay(false);
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