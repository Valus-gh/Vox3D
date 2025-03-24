using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

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
}
