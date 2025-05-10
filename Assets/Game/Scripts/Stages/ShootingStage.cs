using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Interaction;
using Game.Weapons;
using Game.Utilities;
using Game.Networking;
using Game.Resources;

namespace Game.Stages 
{
    public class ShootingStage : GameStage
    {
        private Dictionary<uint, PlayerTower> _Towers;
        private ProjectileResources _ProjectileTemplates;

        public override void Initialize()
        {
            if (!IsInitialized)
            {
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

                _Towers = new Dictionary<uint, PlayerTower>();
                var towers = FindObjectsOfType<PlayerTower>();

                foreach (var tower in towers)
                {
                    for(int i = 0; i < _ProjectileTemplates.Projectiles.Length; i++)
                        if (_ProjectileTemplates.Projectiles[i].Name == "Basic")
                            tower.TowerControls.Projectile.FromModel(_ProjectileTemplates.Projectiles[i]);

                    var ownerID = tower.GetComponent<OwnedBy>().OwnerID;

                    _Towers.Add(ownerID, tower);
                }

                IsInitialized = true;
            }

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

                if (player.CurrentHitpoints <= 0)
                {
                    //Fire player gameover event
                }
            }
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
                    tower.TowerControls.FireProjectile(trajectory, owner);
                }
            }

            _ConfirmedTrajectories.Clear();
        }
       
        protected override void Run()
        {
            if (!IsComplete)
            {
                // if all players confirmed a trajectory, shoot projectiles on all clients
                if (_ConfirmedTrajectories.Count == Players.Count)
                {
                    Debug.Log("ALL PLAYERS CONFIRMED THEIR TRAJECTORY. FIRING PROJECTILES.");
                    FireProjectiles();

                    IsComplete = true;

                    RpcToggleAimingArrows();
                }
            }
        }

    }
}