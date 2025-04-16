using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Networking;

namespace Game.Interaction
{
    public class TowerControlsMultiplayerKeyboard : TowerControlsMultiplayer
    {

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(fireCode))
            {
                if (Projectile is not null)
                {
                    RelayTrajectory(TowerTrajectory, NetworkRoomManagerV3D.singleton.PlayerID);
                }
            }

            if (Input.GetKey(KeyCode.Keypad6))
            {
                transform.Rotate(new Vector3(0.0f, -1.0f * 0.5f, 0.0f), Space.World);
                TowerTrajectory.DirectionXZ = new Vector2(transform.right.x, transform.right.z);
            }

            if (Input.GetKey(KeyCode.Keypad4))
            {
                transform.Rotate(new Vector3(0.0f, 1.0f * 0.5f, 0.0f), Space.World);
                TowerTrajectory.DirectionXZ = new Vector2(transform.right.x, transform.right.z);
            }

            if (Input.GetKey(KeyCode.Keypad8))
            {
                transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f * 0.5f));
                TowerTrajectory.Angle += 1.0f * 0.5f;
            }

            if (Input.GetKey(KeyCode.Keypad2))
            {
                transform.Rotate(new Vector3(0.0f, 0.0f, -1.0f * 0.5f));
                TowerTrajectory.Angle -= 1.0f * 0.5f;
            }
        }


        
    }
}