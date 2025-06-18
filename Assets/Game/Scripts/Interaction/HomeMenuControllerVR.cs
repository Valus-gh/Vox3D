using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Networking;

namespace Game.Interaction
{

    public class HomeMenuControllerVR : MonoBehaviour
    {

        [SerializeField] private NetworkManager _Manager;

        [SerializeField] private GameObject     _MainSection;
        [SerializeField] private GameObject     _ReadySection;

        public void Awake()
        {
        }

        public void StartHost()
        {
            _Manager.StartHost();
        }

        public void ConnectClient()
        {
            _Manager.StartClient(); 
        }

        public void ReadyClient()
        {
            if (NetworkClient.active)
            {
                NetworkClient.localPlayer.gameObject.GetComponent<NetworkRoomPlayerV3D>().CmdChangeReadyState(true);
            }
        }
    }

}