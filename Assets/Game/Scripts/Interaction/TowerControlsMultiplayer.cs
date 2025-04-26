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
        [SerializeField]
        private Projectile _Projectile;
        protected Trajectory TowerTrajectory;

        public KeyCode fireCode = KeyCode.Space;

        public Projectile Projectile { get => _Projectile; set => _Projectile = value; }

        // Start is called before the first frame update
        void Start()
        {
            if (Projectile is null)
                Debug.LogWarning($"No projectile assigned to tower");
            else
            {
                TowerTrajectory.DirectionXZ = Vector3.one;
                TowerTrajectory.Angle = 45.0f;
                TowerTrajectory.Speed = 30.0f;

                transform.Rotate(new Vector3(0.0f, 0.0f, TowerTrajectory.Angle));

                Projectile.Trajectory = TowerTrajectory;
            }
        }

        public void RelayTrajectory(Trajectory trajectory, uint ownerID)
        {
            FindObjectOfType<ShootingStage>().CmdConfirmTrajectory(trajectory, ownerID);
        }

        public void FireProjectile(Trajectory trajectory, uint ownerID)
        {
            Debug.Log($"Firing Projectile {Projectile.name}");

            // TODO extract instantiate function to Projectile, so it can be specified for each type
            var instance = Instantiate(Projectile, transform.position, transform.rotation, null);

            instance.GetComponent<OwnedBy>().OwnerID = ownerID;

            instance.Trajectory = trajectory;
            instance.Aim();
            instance.Fire();

            NetworkServer.Spawn(instance.gameObject);
        }

    }

}