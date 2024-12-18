using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControlsKeyboard : MonoBehaviour
{
    public Projectile    Projectile;
    private Trajectory  _TowerTrajectory;

    public KeyCode fireCode = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        if (Projectile is null)
            Debug.LogWarning($"No projectile assigned to tower");
        else
        {
            _TowerTrajectory.DirectionXZ = Vector3.one;
            _TowerTrajectory.Angle = 45.0f;
            _TowerTrajectory.Speed = 30.0f;

            Projectile.Trajectory = _TowerTrajectory;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(fireCode))
        {
            if (Projectile is not null) { 

                Debug.Log($"Firing Projectile {Projectile.name}");

                var instance = Instantiate(Projectile, transform.position, transform.rotation, null);

                instance.Trajectory = _TowerTrajectory;

                instance.Aim();
                instance.Fire();
            }
        }

        if (Input.GetKey(KeyCode.Keypad6))
        {
            transform.Rotate(new Vector3(0.0f, 1.0f * 0.5f,  0.0f));
            _TowerTrajectory.DirectionXZ = new Vector2(transform.right.x, transform.right.z);
        }

        if (Input.GetKey(KeyCode.Keypad4))
        {
            transform.Rotate(new Vector3(0.0f, -1.0f * 0.5f, 0.0f));
            _TowerTrajectory.DirectionXZ = new Vector2(transform.right.x, transform.right.z);
        }

        if (Input.GetKey(KeyCode.Keypad8))
        {
            _TowerTrajectory.Angle += 1.0f * 0.5f;
            Debug.Log(_TowerTrajectory.Angle);
        }

        if (Input.GetKey(KeyCode.Keypad2))
        {
            _TowerTrajectory.Angle -= 1.0f * 0.5f;
            Debug.Log(_TowerTrajectory.Angle);
        }

    }
}
