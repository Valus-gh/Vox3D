using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace Vox3D.Networking 
{

    public class TowerControlsMultiplayer : NetworkBehaviour
    {
        [SerializeField]
        protected Projectile Projectile;
        protected Trajectory TowerTrajectory;

        public KeyCode fireCode = KeyCode.Space;

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