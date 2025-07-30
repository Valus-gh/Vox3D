using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

using Game.Networking;
using Oculus.Interaction;

namespace Game.Interaction
{
    public class TowerControlsMultiplayerQuest : TowerControlsMultiplayer
    {
        // Update is called once per frame
        void Update()
        {
            if (FindObjectOfType<TowerSelector_Quest>().SelectedTower == Tower)
            {
                Vector2 stickAxisXUpDown = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.GetActiveController());

                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                {
                    transform.Rotate(new Vector3(0.0f, 0.0f, stickAxisXUpDown.y * 0.5f));
                    TowerTrajectory.Angle += stickAxisXUpDown.y * 0.5f;

                    TowerTrajectory.DirectionXZ = new Vector2(transform.right.x, transform.right.z);
                }

                if(OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three))
                {
                    RelayTrajectory(TowerTrajectory, NetworkRoomManagerV3D.singleton.PlayerID);
                }

            }

        }
    }

}