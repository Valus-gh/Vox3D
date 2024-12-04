using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTestDemo : MonoBehaviour
{

    public GameObject ProjectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        var instance = Instantiate(ProjectilePrefab);

        var projectile = instance.GetComponent<Projectile>();
        Trajectory trajectory = new Trajectory();
        trajectory.Speed = 20.0f;
        trajectory.Angle = 60.0f;
        trajectory.DirectionXZ = new Vector3(1.0f, 0.0f);

        projectile.Trajectory = trajectory;
        projectile.Aim();
        projectile.Fire();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
