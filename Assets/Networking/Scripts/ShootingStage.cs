using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace Vox3D.Networking 
{
    public class ShootingStage : GameStage
    {

        private Dictionary<uint, PlayerTower> _Towers;

        public override void Deinitialize()
        {
            throw new System.NotImplementedException();
        }

        public override void Initialize()
        {
            _Towers = new Dictionary<uint, PlayerTower>();
            var towers = FindObjectsOfType<PlayerTower>();

            foreach (var tower in towers)
            {
                var ownerID = tower.GetComponent<OwnedBy>().OwnerID;

                _Towers.Add(ownerID, tower);
            }

            RpcToggleAimingArrows();

        }

        private Dictionary<uint, Trajectory> _ConfirmedTrajectories = new Dictionary<uint, Trajectory>();

        [Command(requiresAuthority = false)]
        public void CmdConfirmTrajectory(Trajectory trajectory, uint ownerID)
        {
            if(_ConfirmedTrajectories.Count < Players.Count)
                _ConfirmedTrajectories.Add(ownerID, trajectory);
        }

        [ClientRpc]
        private void RpcToggleAimingArrows()
        {
            var arrows = FindObjectsByType<AimingArrow>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach(var arrow in arrows)
            {
                Debug.Log("arrow --->" + arrow);
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