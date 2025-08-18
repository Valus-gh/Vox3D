using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Networking;
using Game.Resources;

namespace Game.Stages 
{ 

    public class StageManager : NetworkBehaviour
    {

        [SerializeField] public static int TowersPerPlayer = 2;

        [SerializeReference]
        private List<GameObject> _Players = new List<GameObject>();
        public static ProjectileResources ProjectileTemplates;

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

        /**
         * Fetch the Stage components and add them to the StageQueue in order.
         */
        void Start()
        {
            _Stages = new StageQueue(3);
            _Stages.Enqueue(GetComponent<SelectionStage>());
            _Stages.Enqueue(GetComponent<ShootingStage>());
            _Stages.Enqueue(GetComponent<ReportStage>());

            ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");
        }

        void Update()
        {
            // All players have finished entering gameplay scene
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

                // After connecting, the gameplay stagequeue begins playing.
                // The stages exist on all clients, but are only executed on the server, to keep decisions server-side
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
            // Will NOT show up in VR

            if (_LoadingScreen_Instance is null)
            {
                _LoadingScreen_Instance = Instantiate(_LoadingScreen_Prefab, this.transform);
            }

            _LoadingScreen_Instance.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
            _LoadingScreen_Instance.SetActive(active);
        }

        public static Player GetPlayerByID(uint playerID)
        {
            var stageManager = FindObjectOfType<StageManager>();

            var players = stageManager._Players;

            foreach(var p in players)
            {
                if (p.GetComponent<Player>().netId == playerID)
                    return p.GetComponent<Player>();
            }

            return null;
        }

    }

}