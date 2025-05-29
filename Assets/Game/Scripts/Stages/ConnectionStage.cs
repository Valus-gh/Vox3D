using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Networking;
using Game.Utilities;
using Game.Resources;

namespace Game.Stages
{
    public class ConnectionStage : GameStage
    {
        private bool _Loading;
        private uint _PlayerObjectsReady = 0;
        private uint _WorldsReady = 0;
        private bool _TowersSpawned;

        public  override void Initialize()
        {
            //throw new System.NotImplementedException();
        }

        public override void Deinitialize()
        {
            throw new System.NotImplementedException();
        }

        protected override void Run()
        {
            if (!IsComplete && !_Loading)
            {
                foreach(var player in Players)
                {
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcLoadWorld();
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcInitializePlayer();
                }

                _Loading = true;
            }

            if (!IsComplete && _Loading && _WorldsReady < Players.Count)
            {
                foreach (var player in Players)
                {
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcCheckWorldLoading();
                }
            }

            if(!IsComplete && _WorldsReady == Players.Count && !_TowersSpawned)
            {
                foreach (var player in Players)
                {
                    for (int i = 0; i < StageManager.TowersPerPlayer; i++)
                    {
                        var tower = Instantiate(NetworkRoomManagerV3D.singleton.spawnPrefabs[1]);

                        tower.GetComponent<OwnedBy>().OwnerID = player.GetComponent<NetworkIdentity>().netId;

                        tower.GetComponentInChildren<PlayerTower>().TowerID = (int)(i + 10 * player.GetComponent<NetworkIdentity>().netId);

                        NetworkServer.Spawn(tower);
                    }
                }

                foreach (var player in Players)
                {
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcFetchPlayerTowers();
                }

                _TowersSpawned = true;
            }

            /*
            if (_Loading is false && IsComplete is false)
            {
                _Loading = true;

                foreach (var player in Players)
                {
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcLoadWorld();

                    for (int i = 0; i < StageManager.TowersPerPlayer; i++)
                    {
                        var tower = Instantiate(NetworkRoomManagerV3D.singleton.spawnPrefabs[1]);
                        tower.GetComponent<OwnedBy>().OwnerID = player.GetComponent<NetworkIdentity>().netId;
                        tower.GetComponentInChildren<PlayerTower>().TowerID = (int)(i + 10 * player.GetComponent<NetworkIdentity>().netId);

                        NetworkServer.Spawn(tower);
                    }
                }

                FindObjectOfType<NetworkRoomPlayerV3D>().CmdInitializePlayers();
                FindObjectOfType<NetworkRoomPlayerV3D>().RpcFetchPlayerTowers();
            }*/

            if (NetworkRoomManagerV3D.singleton.TowerPositions is not null && _PlayerObjectsReady == Players.Count)
            {
                IsRunning = false;
                IsComplete = true;
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdConfirmPlayerReady()
        {
            _PlayerObjectsReady++;
        }

        [Command(requiresAuthority = false)]
        public void CmdConfirmWorldLoaded()
        {
            _WorldsReady++;
        }
    }

}