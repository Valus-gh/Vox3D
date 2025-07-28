using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Interaction;
using Game.Weapons;
using Game.Utilities;
using Game.Networking;
using Game.Resources;
using Vox3D.Engine;

//TODO tidy up order of RPCS AND CMDS

namespace Game.Stages 
{
    public class ShootingStage : GameStage
    {
        [SerializeField] public GameObject ExplosionParticles;

        private Dictionary<uint, List<PlayerTower>> _TowersByPlayer;
        private ProjectileResources _ProjectileTemplates;

        [SerializeField]
        private GameObject _WeaponBarHUD;
        private GameObject _WeaponBarHUD_Instance;

        private int _DestroyedTowers;

        private bool _Shooting = false;
        public override void Initialize()
        {
            if (!IsInitialized)
            {
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

                _TowersByPlayer = new Dictionary<uint, List<PlayerTower>>();
                _DestroyedTowers = 0;

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

                // Instantiate and initialize HUD
                LoadHUD();

                IsInitialized = true;
            }

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
        }

        public override void Deinitialize()
        {
            throw new System.NotImplementedException();
        }

        private Dictionary<uint, List<(int, Trajectory)>> _ConfirmedTrajectories = new Dictionary<uint, List<(int, Trajectory)>>();

        [Command(requiresAuthority = false)]
        public void CmdConfirmTrajectory(Trajectory trajectory, int towerID, uint ownerID)
        {
            if (!_ConfirmedTrajectories.ContainsKey(ownerID))
            {
                _ConfirmedTrajectories.Add(ownerID, new List<(int, Trajectory)>());
            }

            if (_ConfirmedTrajectories[ownerID].Count < StageManager.TowersPerPlayer)
            {
                // Check if towerID is already present
                foreach(var element in _ConfirmedTrajectories[ownerID])
                    if (element.Item1 == towerID) return;

                _ConfirmedTrajectories[ownerID].Add((towerID, trajectory));
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdTestTowerCollision(Vector3 center, float radius, float damage)
        {
            Collider[] colliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask("Player"));

            if (colliders.Length == 0)
                return;

            foreach (var c in colliders)
            {
                var ownerID = c.GetComponentInParent<OwnedBy>().OwnerID;

                var damagedPlayer = Players.Find((p) => p.GetComponent<Player>().netId == ownerID).GetComponent<Player>();
                damagedPlayer.CurrentHitpoints -= damage;

                damagedPlayer.DamageTowerWithId(c.GetComponentInParent<PlayerTower>().TowerID, damage);
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdEquipWeapon(int towerID, string weaponName, bool isSetup)
        {
            foreach(var (player, towers) in _TowersByPlayer)
            {
                foreach(var tower in towers)
                {
                    if(tower.TowerID == towerID)
                    {
                        var inventory = tower.Player.Inventory;

                        for (int i = 0; i < _ProjectileTemplates.Projectiles.Length; i++)
                        {
                            if (_ProjectileTemplates.Projectiles[i].Name == weaponName)
                            {
                                if(tower.WeaponTemplate != null && !isSetup)
                                    inventory.IncreaseItem(tower.WeaponTemplate.Name);

                                tower.WeaponTemplate = _ProjectileTemplates.Projectiles[i];
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

        public void Selectweapon(string weaponName)
        {
            var currentTower = GetComponent<TowerSelector_Quest>().SelectedTower;

            if (currentTower is null) return;

            if (currentTower.Player.Inventory.GetProjectile(weaponName) == 0)
            {
                Debug.Log($"Unable to equip {weaponName} on tower {currentTower.TowerID}. Insufficient ammo.");
                return;
            }

            for (int i = 0; i < _ProjectileTemplates.Projectiles.Length; i++)
            {
                if (_ProjectileTemplates.Projectiles[i].Name == weaponName)
                {
                    CmdEquipWeapon(currentTower.TowerID, weaponName, false);
                    break;
                }
            }
        }

        private void LoadHUD()
        {
            GetComponent<TowerSelector_Quest>().enabled = true;

            if (_WeaponBarHUD_Instance is not null) return;
            if (_ProjectileTemplates is null)
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

            _WeaponBarHUD_Instance = Instantiate(_WeaponBarHUD, UnityEngine.Camera.main.transform);
            _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDControllerVR>().InitializeHUD(_ProjectileTemplates);
            _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDControllerVR>().ToggleDisplay(false);
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

        [Command(requiresAuthority = false)]
        public void CmdScatterProjectiles(string template, List<Trajectory> trajectories, Vector3 scatterOrigin, uint ownerID)
        {
            foreach(var pTemplate in _ProjectileTemplates.Projectiles)
            {
                if(pTemplate.Name == template)
                {
                    foreach (var tower in _TowersByPlayer[ownerID])
                    {
                        var projectile = tower.EquippedWeapon;

                        foreach(var trajectory in trajectories)
                        {
                            var instance = Instantiate(projectile, scatterOrigin, Quaternion.identity, null);

                            var projectileTrajectory = new Trajectory();
                            projectileTrajectory.DirectionXZ = trajectory.DirectionXZ;
                            projectileTrajectory.Angle = trajectory.Angle;

                            instance.GetComponent<OwnedBy>().OwnerID = ownerID;
                            instance.Trajectory = projectileTrajectory;
                            instance.Aim();
                            instance.Fire();

                            NetworkServer.Spawn(instance.gameObject);

                            instance.RpcSetValuesAfterSpawn(
                                pTemplate.Blast.Radius,
                                pTemplate.Blast.RadiusOffset,
                                pTemplate.Blast.Damage,
                                pTemplate.Blast.Scatter,
                                pTemplate.Blast.ScatterOnImpact,
                                pTemplate.Blast.ScatterAngle,
                                pTemplate.Blast.ScatterAmount,
                                pTemplate.Blast.ScatterBehaviour,
                                pTemplate.Blast.Child
                            );
                        }
                    }
                }
            }
        }

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
                        }
                    }
                }
            }

            _ConfirmedTrajectories.Clear();

            StartCoroutine(CompleteStageInSeconds(10));
        }
       
        public void TowerDestroyed()
        {
            _DestroyedTowers++;
        }

        private IEnumerator CompleteStageInSeconds(float delay)
        {
            yield return new WaitForSeconds(delay);

            _Shooting = false;
            IsComplete = true;
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

                // if all players confirmed a trajectory, shoot projectiles on all clients
                if (confirmedTrajectories == Players.Count * StageManager.TowersPerPlayer - _DestroyedTowers)
                {
                    Debug.Log("ALL PLAYERS CONFIRMED THEIR TRAJECTORY. FIRING PROJECTILES.");
                    FireProjectiles();

                    RpcToggleAimingArrows();
                    RpcToggleHUD(false);

                    GetComponent<TowerSelector_Quest>().enabled = false;
                }
            }
        }

    }
}