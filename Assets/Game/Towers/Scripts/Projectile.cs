using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Blast))]
public class Projectile : MonoBehaviour
{
    [SerializeReference]
    private Rigidbody   _ProjectileBody; 

    [SerializeReference]
    private Blast       _Blast;

    private Trajectory  _Trajectory;

    public Rigidbody    ProjectileBody { get => _ProjectileBody; set => _ProjectileBody = value; }
    public Trajectory   Trajectory { get => _Trajectory; set => _Trajectory = value; }
    public Blast        Blast { get => _Blast; set => _Blast = value; }

    public void Aim()
    {
        Quaternion lookTowardsTrajectory    = Quaternion.LookRotation(new Vector3(_Trajectory.DirectionXZ.x, 0.0f, _Trajectory.DirectionXZ.y));
        Quaternion angleRotation            = Quaternion.AngleAxis(360.0f - _Trajectory.Angle, new Vector3(1.0f, 0.0f, 0.0f));

        transform.rotation = lookTowardsTrajectory * angleRotation;
    }
    public void Aim(Trajectory trajectory)
    {
        Quaternion lookTowardsTrajectory    = Quaternion.LookRotation(new Vector3(trajectory.DirectionXZ.x, 0.0f, trajectory.DirectionXZ.y));
        Quaternion angleRotation            = Quaternion.AngleAxis(360.0f - trajectory.Angle, new Vector3(1.0f, 0.0f, 0.0f));

        transform.rotation = lookTowardsTrajectory * angleRotation;
    }
    public void Fire()
    {
        if(_ProjectileBody is not null)
        {
            _ProjectileBody.velocity = _Trajectory.Speed * transform.forward;
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        Blast.Trigger(collision);
    }

}
