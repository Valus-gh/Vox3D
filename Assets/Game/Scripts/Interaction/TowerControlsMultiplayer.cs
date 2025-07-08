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
        public Trajectory TowerTrajectory;
        public PlayerTower Tower;

        void Start()
        {
            TowerTrajectory.DirectionXZ = Vector3.one;
            TowerTrajectory.Angle = 45.0f;
            TowerTrajectory.Speed = 30.0f;

            transform.Rotate(new Vector3(0.0f, 0.0f, TowerTrajectory.Angle));
        }

        public void RelayTrajectory(Trajectory trajectory, uint ownerID)
        {
            FindObjectOfType<ShootingStage>().CmdConfirmTrajectory(trajectory, Tower.TowerID, ownerID);
        }

        public void FireProjectileOnAllClients(Trajectory trajectory, uint ownerID)
        {
            var weaponTemplate = GetComponentInParent<PlayerTower>().WeaponTemplate;
            var currentWeapon = GetComponentInParent<PlayerTower>().EquippedWeapon;

            Debug.Log($"Firing Projectile {weaponTemplate.Name} from player {ownerID}");

            var instance = Instantiate(currentWeapon, transform.position, transform.rotation, null);

            instance.GetComponent<OwnedBy>().OwnerID = ownerID;

            instance.Trajectory = trajectory;
            instance.Aim();
            instance.Fire();

            NetworkServer.Spawn(instance.gameObject);

            instance.RpcSetValuesAfterSpawn(
                weaponTemplate.Blast.Radius,
                weaponTemplate.Blast.RadiusOffset,
                weaponTemplate.Blast.Damage,
                weaponTemplate.Blast.Scatter,
                weaponTemplate.Blast.ScatterOnImpact,
                weaponTemplate.Blast.ScatterAngle,
                weaponTemplate.Blast.ScatterAmount,
                weaponTemplate.Blast.ScatterBehaviour,
                weaponTemplate.Blast.Child
            );
        }

    }

}