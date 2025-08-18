using UnityEngine;
using static Game.Resources.ProjectileResources;
using Mirror;

using Game.Utilities;
using Game.Fog;
using Game.Networking;
using Game.Fog.FischlWorks;

namespace Game.Weapons
{

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : NetworkBehaviour
    {

        [SerializeReference] private Rigidbody  _ProjectileBody;

        private Trajectory                      _Trajectory;
        [SerializeField] private Blast          _Blast;

        public Rigidbody ProjectileBody         { get => _ProjectileBody; set => _ProjectileBody = value; }
        public Trajectory Trajectory            { get => _Trajectory; set => _Trajectory = value; }
        public Blast Blast                      { get => _Blast; set => _Blast = value; }

        public void Start()
        {
            Debug.Log($"Projectile spawned. BLAST: {_Blast}");
            Debug.Log($"Blast INFO: {_Blast.Radius} - {_Blast.RadiusOffset} - {_Blast.Damage}");
        }

        public void Aim()
        {
            Quaternion lookTowardsTrajectory = Quaternion.LookRotation(new Vector3(_Trajectory.DirectionXZ.x, 0.0f, _Trajectory.DirectionXZ.y));
            Quaternion angleRotation = Quaternion.AngleAxis(360.0f - _Trajectory.Angle, new Vector3(1.0f, 0.0f, 0.0f));

            transform.rotation = lookTowardsTrajectory * angleRotation;
        }
        public void Aim(Trajectory trajectory)
        {
            Quaternion lookTowardsTrajectory = Quaternion.LookRotation(new Vector3(trajectory.DirectionXZ.x, 0.0f, trajectory.DirectionXZ.y));
            Quaternion angleRotation = Quaternion.AngleAxis(360.0f - trajectory.Angle, new Vector3(1.0f, 0.0f, 0.0f));

            transform.rotation = lookTowardsTrajectory * angleRotation;

            _Trajectory = trajectory;
        }
        public void Fire()
        {
            if (_ProjectileBody is not null)
            {
                _ProjectileBody.velocity = _Trajectory.Speed * transform.forward;
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
             _Blast.Trigger(collision, GetComponent<OwnedBy>().OwnerID);
            Destroy(gameObject);
        }

        /// <summary>
        /// Needed to properly apply Blast and Projectile values after it has been spawned.
        /// Loads the 3D model for the projectile, scaling it accordingly.
        /// Adds the projectiles as fog revealers, unless they are of the Bomb type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="radius"></param>
        /// <param name="radiusOffset"></param>
        /// <param name="damage"></param>
        /// <param name="scatter"></param>
        /// <param name="scatterOnImpact"></param>
        /// <param name="scatterAngle"></param>
        /// <param name="scatterAmount"></param>
        /// <param name="scatterBehaviour"></param>
        /// <param name="child"></param>
        [ClientRpc]
        public void RpcSetValuesAfterSpawn(string name, float radius, float radiusOffset, float damage, bool scatter, bool scatterOnImpact, float scatterAngle, float scatterAmount, string scatterBehaviour, string child)
        {
            Debug.Log($"Attempting to instantiate projectile model {name}.obj");
            var projectileModel = UnityEngine.Resources.Load($"ProjectileData/{name}");

            var model = Instantiate(projectileModel, this.transform) as GameObject;
            model.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

            var rotObject = model.AddComponent<RotatingObject>();
            rotObject.Speed = 120.0f;
            rotObject.Axis = new Vector3(Vector2.Perpendicular(Trajectory.DirectionXZ).x, 0, Vector2.Perpendicular(Trajectory.DirectionXZ).y);
            rotObject.ForUI = false;

            if (_Blast is null) _Blast = new Blast();

            if(GetComponent<OwnedBy>().OwnerID == NetworkRoomManagerV3D.singleton.PlayerID)
            {
                Destroy(gameObject.GetComponent<csFogVisibilityAgent>());
                if (name != "Bomb")
                {
                    gameObject.AddComponent<FoWRevealer>();
                }
            }

            _Blast.Radius = radius;
            _Blast.RadiusOffset = radiusOffset;
            _Blast.Damage = damage;
            _Blast.Scatter = scatter;
            _Blast.ScatterOnImpact = scatterOnImpact;
            _Blast.ScatterAngle = scatterAngle;
            _Blast.ScatterAmount = scatterAmount;
            _Blast.ScatterBehaviour = scatterBehaviour;
            _Blast.Child = child;
        }

    }

}