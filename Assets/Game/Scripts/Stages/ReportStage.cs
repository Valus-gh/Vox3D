using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using Game.Utilities;
using Game.Networking;
using Game.Interaction;

namespace Game.Stages
{
    public class ReportStage : GameStage
    {
        public class ReportData
        {
            public float Damage_Dealt;
            public float Damage_Taken;

            public uint[] Data = new uint[7];

            public const uint Players_Eliminated     = 0;
            public const uint Player_RemainingTowers = 1;
            public const uint Budget_Spent           = 2;
            public const uint Budget_Remaining       = 3;
            public const uint Ammo_Purchased         = 4;
            public const uint Ammo_Shot              = 5;
            public const uint Ammo_Hit               = 6;
        }

        public static int Round = 0;
        public static bool GameEnded;
        private static Dictionary<Player, ReportData> _PlayerData = new Dictionary<Player, ReportData>();

        [SerializeField] 
        private GameObject _ReportCardHUD;
        private GameObject _ReportCardHUD_Instance;

        public override void Initialize()
        {
            Round++;
            IsInitialized = true;

            if(!GameEnded) IsComplete = true;

        }

        public override void Deinitialize()
        {
        }

        protected override void Run()
        {

            if (IsComplete) return;

            if (GameEnded)
            {

                foreach (var player in Players)
                {
                    if (player.GetComponent<Player>().Winner)
                    {
                        WinGame(player.GetComponent<Player>());
                        IsComplete = true;
                    }
                }

            }
        }

        [Command]
        public void EliminatePlayer(Player player)
        {
            player.RpcShowReport(_PlayerData[player], false);
        }

        public void WinGame(Player player)
        {
            player.RpcShowReport(_PlayerData[player], true);
        }

        public void LoadHUD(ReportData playerData, bool winner)
        {
            _ReportCardHUD_Instance = Instantiate(_ReportCardHUD, UnityEngine.Camera.main.transform);
            _ReportCardHUD_Instance.GetComponent<ReportCardHUDController>().SetupLabels(playerData, winner);
        }

        public ReportData GetPlayerData(uint id)
        {
            CheckPlayerData();
            foreach (var (player, data) in _PlayerData)
            {
                if (player.GetComponent<NetworkRoomPlayerV3D>().netId == id)
                    return data;
            }

            return null;
        }

        public List<ReportData> GetPlayerData()
        {
            CheckPlayerData();
            return new List<ReportData>(_PlayerData.Values);
        }

        public ReportData GetPlayerData(Player player)
        {
            CheckPlayerData();
            return _PlayerData[player];
        }

        private void CheckPlayerData()
        {
            if (_PlayerData.Count == 0)
            {
                foreach (var player in Players)
                {
                    _PlayerData.Add(player.GetComponent<Player>(), new ReportData());
                }
            }
        }

    }

}