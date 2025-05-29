using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Interaction;
using Game.Weapons;
using Game.Utilities;
using Game.Networking;
using Game.Resources;

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

        private bool _Shooting = false;
        public override void Initialize()
        {
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

                foreach (var player in Players)
                {
                    foreach(var towerList in _TowersByPlayer)
                    {
                        if(player.GetComponent<Player>().netId == towerList.Key)
                        {
                            player.GetComponent<Player>().Towers = towerList.Value;
                        }
                    }

                    // Equip basic projectile for all towers
                    foreach(var tower in player.GetComponent<Player>().Towers)
                    {
                        var equipped = tower.EquippedWeapon;

                        for (int i = 0; i < _ProjectileTemplates.Projectiles.Length; i++)
                        {
                            if (_ProjectileTemplates.Projectiles[i].Name == "Basic")
                            {
                                equipped.FromModel(_ProjectileTemplates.Projectiles[i]);
                                break;
                            }
                        }
                    }
                }

                // Instantiate and initialize HUD
                LoadHUD();

                IsInitialized = true;
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

                damagedPlayer.RpcDamageTowerWithId(c.GetComponentInParent<PlayerTower>().TowerID, damage);
            }
        }

        public void Selectweapon(string weaponName)
        {
            var currentTower = GetComponent<TowerSelector_Click>().SelectedTower;

            if (currentTower is null) return;

            for (int i = 0; i < _ProjectileTemplates.Projectiles.Length; i++)
            {
                if (_ProjectileTemplates.Projectiles[i].Name == weaponName)
                {
                    currentTower.EquippedWeapon.FromModel(_ProjectileTemplates.Projectiles[i]);
                    break;
                }
            }
        }

        private void LoadHUD()
        {
            GetComponent<TowerSelector_Click>().enabled = true;

            if (_WeaponBarHUD_Instance is not null) return;
            if (_ProjectileTemplates is null)
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

            _WeaponBarHUD_Instance = Instantiate(_WeaponBarHUD, this.transform);
            _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDController>().InitializeHUD(_ProjectileTemplates);
            _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDController>().ToggleDisplay(false);
        }

        [ClientRpc]
        public void RpcUpdateHUD()
        {
            LoadHUD();

            foreach(var player in FindObjectsOfType<Player>())
            {
                if(player.netId == NetworkRoomManagerV3D.singleton.PlayerID)
                {
                    _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDController>().UpdateHUD(player.Inventory);
                }
            }
        }

        [ClientRpc]
        private void RpcToggleHUD(bool active)
        {
            _WeaponBarHUD_Instance.GetComponent<WeaponBarHUDController>().ToggleDisplay(active);
        }

        [ClientRpc]
        private void RpcToggleAimingArrows()
        {
            var arrows = FindObjectsByType<AimingArrow>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach(var arrow in arrows)
            {
                if (arrow.GetComponentInParent<OwnedBy>().OwnerID == NetworkRoomManagerV3D.singleton.PlayerID)
                    arrow.gameObject.SetActive(!arrow.gameObject.activeSelf);
            }
        }

        private void FireProjectiles()
        {
            _Shooting = true;

            foreach(var (owner, trajectories) in _ConfirmedTrajectories)
            {
                foreach(var (tower, trajectory) in trajectories)
                {
                    foreach(var firingSpot in _TowersByPlayer[owner])
                    {
                        if(firingSpot.TowerID == tower)
                        {
                            firingSpot.GetComponentInChildren<TowerControlsMultiplayer>().FireProjectileOnAllClients(trajectory, owner);
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
                if (confirmedTrajectories == Players.Count * StageManager.TowersPerPlayer)
                {
                    Debug.Log("ALL PLAYERS CONFIRMED THEIR TRAJECTORY. FIRING PROJECTILES.");
                    FireProjectiles();

                    RpcToggleAimingArrows();
                    RpcToggleHUD(false);

                    GetComponent<TowerSelector_Click>().enabled = false;
                }
            }
        }

    }
}