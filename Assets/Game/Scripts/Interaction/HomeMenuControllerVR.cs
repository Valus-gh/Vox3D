using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Networking;
using UnityEngine.UI;

namespace Game.Interaction
{

    public class HomeMenuControllerVR : MonoBehaviour
    {

        [SerializeField] private NetworkManager         _Manager;

        [SerializeField] private GameObject             _MainSection;
        [SerializeField] private GameObject             _ReadySection;

        [SerializeField] private InputField             _AddressInput;
        [SerializeField] private TMPro.TextMeshProUGUI  _AddressLabel;

        public void Awake()
        {
        }

        public void Start()
        {
            if(_Manager is null)
                _Manager = FindObjectOfType<NetworkManager>();

            _AddressLabel.text = _Manager.networkAddress;
        }

        public void StartHost()
        {
            _Manager.StartHost();
        }

        public void ConnectClient()
        {
            _Manager.networkAddress = _AddressInput.text;
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