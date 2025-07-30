using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using Game.Utilities;
using Game.Networking;

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
        private static Dictionary<Player, ReportData> PlayerData = new Dictionary<Player, ReportData>();

        public override void Initialize()
        {
            Round++;
            IsInitialized = true;
            IsComplete = true;
        }

        public override void Deinitialize()
        {
        }

        protected override void Run()
        {
        }

        [Command]
        public void EliminatePlayer(Player player)
        {
            player.RpcEliminateSelf(PlayerData[player]);
        }

        public ReportData GetPlayerData(uint id)
        {
            CheckPlayerData();
            foreach (var (player, data) in PlayerData)
            {
                if (player.GetComponent<NetworkRoomPlayerV3D>().netId == id)
                    return data;
            }

            return null;
        }

        public List<ReportData> GetPlayerData()
        {
            CheckPlayerData();
            return new List<ReportData>(PlayerData.Values);
        }

        public ReportData GetPlayerData(Player player)
        {
            CheckPlayerData();
            return PlayerData[player];
        }

        private void CheckPlayerData()
        {
            if (PlayerData.Count == 0)
            {
                foreach (var player in Players)
                {
                    PlayerData.Add(player.GetComponent<Player>(), new ReportData());
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}