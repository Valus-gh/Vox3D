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
        private Dictionary<uint, PlayerTower> _Towers;
        private Dictionary<uint, PlayerTower> _SelectedTowers;
        
        private ProjectileResources _ProjectileTemplates;

        [SerializeField]
        private GameObject _WeaponBarHUD;
        private GameObject _WeaponBarHUD_Instance;

        public override void Initialize()
        {
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

                foreach (var player in Players)
                {
                    foreach(var tower in _Towers)
                    {
                        if(player.GetComponent<Player>().netId == tower.Key)
                        {
                            player.GetComponent<Player>().Tower = tower.Value;
                        }
                    }

                    var playerEquippedWeapon = player.GetComponent<Player>().Tower.EquippedWeapon;
                    for (int i = 0; i < _ProjectileTemplates.Projectiles.Length; i++)
                    {
                        if (_ProjectileTemplates.Projectiles[i].Name == "Basic")
                        {
                            playerEquippedWeapon.FromModel(_ProjectileTemplates.Projectiles[i]);
                            break;
                        }
                    }
                }

                // Instantiate and initialize HUD
                LoadHUD();

                IsInitialized = true;
            }

            GetComponent<TowerSelector_Click>().enabled = true;
            GetComponent<StageManager>().RpcToggleAllLoadingScreens(false);

            RpcToggleAimingArrows();
        }

        public override void Deinitialize()
        {
            throw new System.NotImplementedException();
        }

        private Dictionary<uint, Trajectory> _ConfirmedTrajectories = new Dictionary<uint, Trajectory>();

        [Command(requiresAuthority = false)]
        public void CmdConfirmTrajectory(Trajectory trajectory, uint ownerID)
        {
            if(_ConfirmedTrajectories.Count < Players.Count)
                _ConfirmedTrajectories.Add(ownerID, trajectory);
        }

        [Command(requiresAuthority = false)]
        public void CmdTestTowerCollision(Vector3 center, float radius, float damage)
        {
            Collider[] colliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask("Player"));

            if (colliders.Length == 0)
                return;

            foreach (var c in colliders)
            {
                var player = c.GetComponentInParent<PlayerTower>().Player;
                player.CurrentHitpoints -= damage;
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdSelectWeapon(string weaponName)
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
            foreach(var (owner, trajectory) in _ConfirmedTrajectories)
            {
                PlayerTower tower;
                _Towers.TryGetValue(owner, out tower);

                if(tower is not null)
                {
                    tower.TowerControls.FireProjectileOnAllClients(trajectory, owner);
                }
            }

            _ConfirmedTrajectories.Clear();
        }
       
        protected override void Run()
        {
            if (!IsComplete)
            {
                RpcUpdateHUD();
                RpcToggleHUD(true);

                // if all players confirmed a trajectory, shoot projectiles on all clients
                if (_ConfirmedTrajectories.Count == Players.Count)
                {
                    Debug.Log("ALL PLAYERS CONFIRMED THEIR TRAJECTORY. FIRING PROJECTILES.");
                    FireProjectiles();

                    RpcToggleAimingArrows();
                    RpcToggleHUD(false);

                    GetComponent<TowerSelector_Click>().enabled = false;

                    IsComplete = true;
                }
            }
        }

    }
}