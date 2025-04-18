using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Networking;

namespace Game.Stages 
{ 

    public class StageManager : NetworkBehaviour
    {

        [SerializeReference]
        private List<GameObject> _Players = new List<GameObject>();

        public void RegisterPlayer(GameObject player)
        {
            if (!_Players.Contains(player)) _Players.Add(player);
            else Debug.LogWarning($"Player {player} already registered");
        }

        [SerializeField]
        private GameObject _LoadingScreen_Prefab;
        private GameObject _LoadingScreen_Instance;

        private GameStage _ConnectionStage;
        private GameStage _ShootingStage;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // All players have finished loading gameplay scene
            if (_Players.Count == NetworkRoomManagerV3D.singleton.minPlayers)
            {

                //SetLoadingScreen(true);

                //Create substages and fill with lobby players
                if (_ConnectionStage is null)
                {
                    _ConnectionStage = GetComponent<ConnectionStage>();
                    _ConnectionStage.Players = _Players;

                    if (isServer)
                    {
                        _ConnectionStage.Initialize();
                        _ConnectionStage.IsRunning = true;
                    }
                }

                if (_ShootingStage is null && _ConnectionStage.IsComplete)
                {
                    _ShootingStage = GetComponent<ShootingStage>();
                    _ShootingStage.Players = _Players;

                    _ShootingStage.Initialize();
                    _ShootingStage.IsRunning = true;
                }

            }
        }

        public void SetLoadingScreen(bool active)
        {
            if (_LoadingScreen_Instance is null)
            {
                _LoadingScreen_Instance = Instantiate(_LoadingScreen_Prefab);
            }

            _LoadingScreen_Instance.SetActive(active);
        }

    }

}