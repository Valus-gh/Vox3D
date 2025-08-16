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

        [SerializeField] public static int TowersPerPlayer = 1;

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
            _Stages = new StageQueue(3);
            _Stages.Enqueue(GetComponent<SelectionStage>());
            _Stages.Enqueue(GetComponent<ShootingStage>());
            _Stages.Enqueue(GetComponent<ReportStage>());
        }

        // Update is called once per frame
        void Update()
        {
            // All players have finished loading gameplay scene
            if (_Players.Count == NetworkRoomManagerV3D.singleton.minPlayers)
            {
                // First stage is always connectioon, and is only executed once at the very start.
                if (_ConnectionStage is null)
                {
                    _ConnectionStage = GetComponent<ConnectionStage>();
                    _ConnectionStage.Players = _Players;

                    // This only populates the player lists for each stage atm
                    _Stages.Initialize(_Players);

                    if (isServer)
                    {
                        _ConnectionStage.IsRunning = true;
                        _CurrentStage = _ConnectionStage;
                    }
                }

                // After connecting, the gameplay stagequeue begins playing
                if(_CurrentStage == _ConnectionStage && _CurrentStage.IsComplete)
                {
                    if (isServer)
                        _CurrentStage = _Stages.Start();
                }
                else if (_CurrentStage != _ConnectionStage && _CurrentStage.IsComplete)
                {
                    if (isServer)
                        _CurrentStage = _Stages.Advance();
                }
            }
        }

        [ClientRpc]
        public void RpcToggleAllLoadingScreens(bool active)
        {
            ToggleLoadingScreen(active);
        }

        public void ToggleLoadingScreen(bool active, string text = "Loading")
        {
            if (_LoadingScreen_Instance is null)
            {
                _LoadingScreen_Instance = Instantiate(_LoadingScreen_Prefab, this.transform);
            }

            _LoadingScreen_Instance.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
            _LoadingScreen_Instance.SetActive(active);
        }

    }

}