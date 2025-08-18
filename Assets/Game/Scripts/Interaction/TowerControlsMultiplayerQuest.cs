using UnityEngine;

using Game.Networking;

namespace Game.Interaction
{

    /// <summary>
    /// Enables vertical rotation for a "Grabbed" tower, as well as the Shooting command
    /// </summary>
    public class TowerControlsMultiplayerQuest : TowerControlsMultiplayer
    {
        // Update is called once per frame
        void Update()
        {
            if (FindObjectOfType<TowerSelector_Quest>().SelectedTower == Tower)
            {
                Vector2 stickAxisXUpDown = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.GetActiveController());

                //if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                //{
                    transform.Rotate(new Vector3(0.0f, 0.0f, stickAxisXUpDown.y * 0.5f));
                    TowerTrajectory.Angle += stickAxisXUpDown.y * 0.5f;
                    Debug.Log(stickAxisXUpDown.y);

                    TowerTrajectory.DirectionXZ = new Vector2(transform.right.x, transform.right.z);
                //}

                // Buttons A or B (could be different depending on headset)
                if(OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three))
                {
                    RelayTrajectory(TowerTrajectory, NetworkRoomManagerV3D.singleton.PlayerID);
                }

            }

        }
    }

}