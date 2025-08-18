using UnityEngine;

using Mirror;

using Game.Networking;
using Game.Utilities;

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
            Debug.Log("Initializing ConnectionStage");         }

        public override void Deinitialize()
        {
            Debug.Log("Deinitializing ConnectionStage");
        }

        protected override void Run()
        {
            // The stage has just begun, tell all clients to load the world and initialize each player's data

            if (!IsComplete && !_Loading)
            {
                foreach(var player in Players)
                {
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcLoadWorld();
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcInitializePlayer();
                }

                _Loading = true;
            }

            // Once we've begun the loading process for each player, we check that everyone has loaded correctly

            if (!IsComplete && _Loading && _WorldsReady < Players.Count)
            {
                foreach (var player in Players)
                {
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcCheckWorldLoading();
                }
            }

            // if all players are done loading, and we have yet to spawn towers, instantate them and spawn them on each client

            if(!IsComplete && _WorldsReady == Players.Count && !_TowersSpawned)
            {
                foreach (var player in Players)
                {
                    for (int i = 0; i < StageManager.TowersPerPlayer; i++)
                    {
                        var tower = Instantiate(NetworkRoomManagerV3D.singleton.spawnPrefabs[1]);

                        // Towers have a TowerID which is preceded by their owner's ID.
                        // Example: Player netID = 1 -> Tower 0 ID = 10, Tower 1 ID = 11

                        tower.GetComponent<OwnedBy>().OwnerID = player.GetComponent<NetworkIdentity>().netId;
                        tower.GetComponentInChildren<PlayerTower>().TowerID = (int)(i + 10 * player.GetComponent<NetworkIdentity>().netId);

                        NetworkServer.Spawn(tower);
                    }
                }

                // Hook the necessary class fields for each tower we spawned, on its respective client. Position the towers.

                foreach (var player in Players)
                {
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcFetchPlayerTowers();
                }

                _TowersSpawned = true;
            }

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