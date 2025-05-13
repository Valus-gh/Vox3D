using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Weapons;
using Game.Stages;
using Game.Utilities;

namespace Game.Interaction 
{

    public class TowerControlsMultiplayer : NetworkBehaviour
    {
        protected Trajectory TowerTrajectory;

        public KeyCode fireCode = KeyCode.Space;

        // Start is called before the first frame update
        void Start()
        {
            TowerTrajectory.DirectionXZ = Vector3.one;
            TowerTrajectory.Angle = 45.0f;
            TowerTrajectory.Speed = 30.0f;

            transform.Rotate(new Vector3(0.0f, 0.0f, TowerTrajectory.Angle));
        }

        public void RelayTrajectory(Trajectory trajectory, uint ownerID)
        {
            FindObjectOfType<ShootingStage>().CmdConfirmTrajectory(trajectory, ownerID);
        }

        public void FireProjectileOnAllClients(Trajectory trajectory, uint ownerID)
        {
            var currentWeapon = GetComponentInParent<PlayerTower>().EquippedWeapon;

            Debug.Log($"Firing Projectile {currentWeapon.name}");

            var instance = Instantiate(currentWeapon, transform.position, transform.rotation, null);
            instance.Blast = currentWeapon.Blast;

            instance.GetComponent<OwnedBy>().OwnerID = ownerID;

            instance.Trajectory = trajectory;
            instance.Aim();
            instance.Fire();

            NetworkServer.Spawn(instance.gameObject);
        }

    }

}