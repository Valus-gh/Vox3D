using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interaction
{
    public class PlayerControlsKeyboard : PlayerControls
    {

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(fireCode))
            {
                if (Projectile is not null)
                {

                    Debug.Log($"Firing Projectile {Projectile.name}");

                    var instance = Instantiate(Projectile, transform.position, transform.rotation, null);

                    instance.Trajectory = TowerTrajectory;

                    instance.Aim();
                    instance.Fire();
                }
            }

            if (Input.GetKey(KeyCode.Keypad6))
            {
                transform.Rotate(new Vector3(0.0f, 1.0f * 0.5f, 0.0f));
                TowerTrajectory.DirectionXZ = new Vector2(transform.right.x, transform.right.z);
            }

            if (Input.GetKey(KeyCode.Keypad4))
            {
                transform.Rotate(new Vector3(0.0f, -1.0f * 0.5f, 0.0f));
                TowerTrajectory.DirectionXZ = new Vector2(transform.right.x, transform.right.z);
            }

            if (Input.GetKey(KeyCode.Keypad8))
            {
                TowerTrajectory.Angle += 1.0f * 0.5f;
                Debug.Log(TowerTrajectory.Angle);
            }

            if (Input.GetKey(KeyCode.Keypad2))
            {
                TowerTrajectory.Angle -= 1.0f * 0.5f;
                Debug.Log(TowerTrajectory.Angle);
            }

        }
    }

}