using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Networking;
using Game.Utilities;

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
        private GameObject  _LoadingScreen_Prefab;
        private GameObject  _LoadingScreen_Instance;

        private GameStage   _ConnectionStage;

        private StageQueue  _Stages;
        private GameStage   _CurrentStage;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            _Stages = new StageQueue(1);
            _Stages.Enqueue(GetComponent<SelectionStage>());
            _Stages.Enqueue(GetComponent<ShootingStage>());
        }

        // Update is called once per frame
        void Update()
        {
            // All players have finished loading gameplay scene
            if (_Players.Count == NetworkRoomManagerV3D.singleton.minPlayers)
            {
                //Create substages and fill with lobby players
                if (_ConnectionStage is null)
                {
                    _ConnectionStage = GetComponent<ConnectionStage>();
                    _ConnectionStage.Players = _Players;

                    _Stages.Initialize(_Players);

                    if (isServer)
                    {
                        _ConnectionStage.Initialize();
                        _ConnectionStage.IsRunning = true;
                        _CurrentStage = _ConnectionStage;
                    }
                }

                if (_CurrentStage.IsComplete)
                {
                    _CurrentStage = _Stages.Advance();
                    ToggleLoadingScreen(false);
                }
            }
        }

        public void ToggleLoadingScreen(bool active)
        {
            if (_LoadingScreen_Instance is null)
            {
                _LoadingScreen_Instance = Instantiate(_LoadingScreen_Prefab, this.transform);
            }

            _LoadingScreen_Instance.SetActive(active);
        }

    }

}