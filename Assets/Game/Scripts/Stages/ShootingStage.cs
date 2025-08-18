using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Interaction;
using Game.Weapons;
using Game.Utilities;
using Game.Networking;
using Vox3D.Engine;

namespace Game.Stages 
{
    public class ShootingStage : GameStage
    {

        [SerializeField]
        private GameObject _WeaponBarHUD;
        private GameObject _WeaponBarHUD_Instance;

        [SerializeField] public GameObject ExplosionParticles;

        private Dictionary<uint, List<PlayerTower>> _TowersByPlayer;

        private int     _DestroyedTowers;
        private bool    _Shooting = false;

        /// <summary>
        /// Set up the initial data needed to run the stage.
        /// Initializes tower data for each player.
        /// Equips basic projectile to each player.
        /// </summary>
        public override void Initialize()
        {

            Debug.Log("Initializing ShootingStage");

            #region One-time operations

            if (!IsInitialized)
            {
                _DestroyedTowers = 0;

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

                // Assign each tower to their respective player, then initialize their stats

                foreach (var player in Players)
                {
                    foreach(var towerList in _TowersByPlayer)
                    {
                        if(player.GetComponent<Player>().netId == towerList.Key)
                        {
                            player.GetComponent<Player>().Towers = towerList.Value;
                            player.GetComponent<Player>().InitializeTowerHitpoints();
                            player.GetComponent<Player>().RpcPreserveVoxels(true);
                        }
                    }
                }

                LoadHUD();

                IsInitialized = true;
            }

            #endregion

            #region Each round

            // Equip basic projectile for all towers

            foreach (var player in Players)
            {
                foreach (var tower in player.GetComponent<Player>().Towers)
                {
                    CmdEquipWeapon(tower.TowerID, "Bomb", true);
                }
            }

            GetComponent<StageManager>().RpcToggleAllLoadingScreens(false);

            RpcToggleAimingArrows();

            #endregion
        }

        public override void Deinitialize()
        {
            Debug.Log("Deinitializing ShootingStage");
        }

        private Dictionary<uint, List<(int, Trajectory)>> _ConfirmedTrajectories = new Dictionary<uint, List<(int, Trajectory)>>();

        /// <summary>
        /// Called by clients when they take the "shoot" action. The highlighted tower's trajectory is passed to the server.
        /// If that same client has yet to confirm a trajectory for the tower, it is added.
        /// </summary>
        /// <param name="trajectory"></param>
        /// <param name="towerID"></param>
        /// <param name="ownerID"></param>
        [Command(requiresAuthority = false)]
        public void CmdConfirmTrajectory(Trajectory trajectory, int towerID, uint ownerID)
        {
            if (!_ConfirmedTrajectories.ContainsKey(ownerID))
            {
                _ConfirmedTrajectories.Add(ownerID, new List<(int, Trajectory)>());
            }

            if (_ConfirmedTrajectories[ownerID].Count < StageManager.TowersPerPlayer)
            {
                foreach (var element in _ConfirmedTrajectories[ownerID])
                    if (element.Item1 == towerID)
                    {
                        _ConfirmedTrajectories[ownerID][_ConfirmedTrajectories[ownerID].IndexOf(element)] = (towerID, trajectory);
                        Debug.Log($"So far, we have confirmed {_ConfirmedTrajectories[ownerID].Count} trajectories for owner {ownerID}.");
                        return;
                    }

                _ConfirmedTrajectories[ownerID].Add((towerID, trajectory));

                Debug.Log($"So far, we have confirmed {_ConfirmedTrajectories[ownerID].Count} trajectories for owner {ownerID}.");
            }
        }

        /// <summary>
        /// Checks whether any tower was within the blast radius of a given projectile. Called by Blast.cs, on clients.
        /// If a tower was hit, it takes the according amount of damage.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="damage"></param>
        /// <param name="shooterID"></param>
        [Command(requiresAuthority = false)]
        public void CmdTestTowerCollision(Vector3 center, float radius, float damage, uint shooterID)
        {
            Collider[] colliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask("Player"));

            if (colliders.Length == 0)
                return;

            foreach (var c in colliders)
            {
                var ownerID = c.GetComponentInParent<OwnedBy>().OwnerID;
                var damagedPlayer = StageManager.GetPlayerByID(ownerID);

                //var damagedPlayer = Players.Find((p) => p.GetComponent<Player>().netId == ownerID).GetComponent<Player>();

                damagedPlayer.CurrentHitpoints -= damage;
                damagedPlayer.DamageTowerWithId(c.GetComponentInParent<PlayerTower>().TowerID, damage);

                GetComponent<ReportStage>().GetPlayerData(shooterID).Data[ReportStage.ReportData.Ammo_Hit]++;
                GetComponent<ReportStage>().GetPlayerData(shooterID).Damage_Dealt += damage;
                GetComponent<ReportStage>().GetPlayerData(damagedPlayer).Damage_Taken += damage;
                if (damagedPlayer.CurrentHitpoints <= 0)
                    GetComponent<ReportStage>().GetPlayerData(shooterID).Data[ReportStage.ReportData.Players_Eliminated]++;
            }
        }

        /// <summary>
        /// Called by clients to attempt to equip a weapon to a specified tower.
        /// Checks whether there is enough ammo in their inventory, and acts accordingly
        /// </summary>
        /// <param name="towerID"></param>
        /// <param name="weaponName"></param>
        /// <param name="isSetup"></param>
        [Command(requiresAuthority = false)]
        private void CmdEquipWeapon(int towerID, string weaponName, bool isSetup)
        {
            foreach(var (player, towers) in _TowersByPlayer)
            {
                foreach(var tower in towers)
                {
                    if(tower.TowerID == towerID)
                    {
                        var towerOwner = StageManager.GetPlayerByID(player);
                        var inventory = towerOwner.Inventory;

                        for (int i = 0; i < StageManager.ProjectileTemplates.Projectiles.Length; i++)
                        {
                            if (StageManager.ProjectileTemplates.Projectiles[i].Name == weaponName)
                            {
                                if(tower.WeaponTemplate != null && !isSetup)
                                    inventory.IncreaseItem(tower.WeaponTemplate.Name);

                                tower.WeaponTemplate = StageManager.ProjectileTemplates.Projectiles[i];
                                inventory.DecreaseItem(tower.WeaponTemplate.Name);

                                Debug.Log($"Equipping weapon {weaponName} on Tower {tower.TowerID}");
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Client-side method called by UI components to pass the request forward to the server.
        /// </summary>
        /// <param name="weaponName"></param>
        public void EquipWeapon(string weaponName)
        {
            var currentTower = GetComponent<TowerSelector_Quest>().SelectedTower;

            if (currentTower is null) return;

            if (currentTower.Player.Inventory.GetProjectile(weaponName) == 0)
            {
                Debug.Log($"Unable to equip {weaponName} on tower {currentTower.TowerID}. Insufficient ammo.");
                return;
            }

            for (int i = 0; i < StageManager.ProjectileTemplates.Projectiles.Length; i++)
            {
                if (StageManager.ProjectileTemplates.Projectiles[i].Name == weaponName)
                {
                    CmdEquipWeapon(currentTower.TowerID, weaponName, false);
                    break;
                }
            }
        }

        [ClientRpc]
        public void RpcUpdateHUD()
        {
            LoadHUD();

            foreach(var player in FindObjectsOfType<Player>())
            {
                if(player.netId == NetworkRoomManagerV3D.singleton.PlayerID)
                {
                    _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDControllerVR>().UpdateHUD(player.Inventory);
                }
            }
        }

        [ClientRpc]
        private void RpcToggleHUD(bool active)
        {
            _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDControllerVR>().ToggleDisplay(active);
        }
        private void LoadHUD()
        {
            GetComponent<TowerSelector_Quest>().enabled = true;

            if (_WeaponBarHUD_Instance is not null) return;

            _WeaponBarHUD_Instance = Instantiate(_WeaponBarHUD, Camera.main.transform);
            _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDControllerVR>().InitializeHUD(StageManager.ProjectileTemplates);
            _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDControllerVR>().ToggleDisplay(false);
        }

        [ClientRpc]
        private void RpcToggleAimingArrows()
        {
            var arrows = FindObjectsByType<AimingArrow>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach(var arrow in arrows)
            {
                if (arrow.GetComponentInParent<OwnedBy>().OwnerID == NetworkRoomManagerV3D.singleton.PlayerID)
                {
                    if (arrow.GetComponentInParent<PlayerTower>().IsDestroyed)
                        arrow.gameObject.SetActive(false);
                    else
                        arrow.gameObject.SetActive(!arrow.gameObject.activeSelf);
                }

            }
        }

        [ClientRpc]
        private void RpcActivateDestructionColliders()
        {
            foreach (var chunk in FindObjectOfType<World>().Chunks.Values)
            {
                chunk.ToggleDestructionCollider(true);
            }
        }

        /// <summary>
        /// Called in Run() once all players have confirmed all trajectories.
        /// Fires projectiles on all clients via the apposite method, as long a tower is not destroyed.
        /// </summary>
        private void FireProjectiles()
        {
            _Shooting = true;

            RpcActivateDestructionColliders();

            foreach(var (owner, trajectories) in _ConfirmedTrajectories)
            {
                foreach(var (tower, trajectory) in trajectories)
                {
                    foreach(var firingSpot in _TowersByPlayer[owner])
                    {
                        if(!firingSpot.IsDestroyed && firingSpot.TowerID == tower)
                        {
                            firingSpot.GetComponentInChildren<TowerControlsMultiplayer>().FireProjectileOnAllClients(trajectory, owner);

                            GetComponent<ReportStage>().GetPlayerData(owner).Data[ReportStage.ReportData.Ammo_Shot]++;
                        }
                    }
                }
            }

            _ConfirmedTrajectories.Clear();

            StartCoroutine(CompleteStageInSeconds(10));
        }

        private IEnumerator CompleteStageInSeconds(float delay)
        {
            yield return new WaitForSeconds(delay);

            _Shooting = false;
            IsComplete = true;
        }
        public void TowerDestroyed()
        {
            _DestroyedTowers++;
        }

        protected override void Run()
        {
            if (!IsComplete && !_Shooting)
            {
                RpcUpdateHUD();
                RpcToggleHUD(true);

                int confirmedTrajectories = 0;

                foreach(var (owner, trajectories) in _ConfirmedTrajectories)
                    confirmedTrajectories += trajectories.Count;

                Debug.Log($"Confirmed Trajectories: {confirmedTrajectories}");

                // if all players confirmed a trajectory, shoot projectiles on all clients
                if (confirmedTrajectories == Players.Count * StageManager.TowersPerPlayer - _DestroyedTowers)
                {
                    Debug.Log("ALL PLAYERS CONFIRMED THEIR TRAJECTORY. FIRING PROJECTILES.");
                    FireProjectiles();

                    RpcToggleAimingArrows();
                    RpcToggleHUD(false);

                    GetComponent<TowerSelector_Quest>().enabled = false;
                }

                foreach(var (player, towers) in _TowersByPlayer)
                {
                    bool eliminated = true;

                    foreach(var tower in towers)
                    {
                        if(!tower.IsDestroyed)
                            eliminated = false;
                    }

                    if (eliminated)
                    {

                        foreach(var playerObject in Players)
                        {
                            if(playerObject.GetComponent<OwnedBy>().OwnerID == player && !playerObject.GetComponent<Player>().Eliminated)
                            {
                                playerObject.GetComponent<Player>().Eliminated = true;

                                GetComponent<ReportStage>().EliminatePlayer(playerObject.GetComponent<Player>());
                                break;
                            }
                        }

                    }
                }

                // Check if there is still more than one player

                int playersLeft = 0;
                Player winner = null;

                foreach (var player in Players) 
                {
                    if (!player.GetComponent<Player>().Eliminated)
                    {
                        playersLeft++;
                        winner = player.GetComponent<Player>();
                    }
                }

                // If only one remains, flag as winner and end game.

                if(playersLeft <= 1)
                {
                    ReportStage.GameEnded = true;
                    winner.Winner = true;
                }

            }
        }

    }
}