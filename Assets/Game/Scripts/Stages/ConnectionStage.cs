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
        private bool _Loading = false;

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

            if(_Loading is false && IsComplete is false)
            {
                _Loading = true;

                foreach (var player in Players)
                {
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcLoadWorld();

                    // TODO spawn multiple towers for each player

                    var playerTower = Instantiate(NetworkRoomManagerV3D.singleton.spawnPrefabs[1]);
                    playerTower.GetComponent<OwnedBy>().OwnerID = player.GetComponent<NetworkIdentity>().netId;
                    NetworkServer.Spawn(playerTower);

                    player.GetComponent<NetworkRoomPlayerV3D>().CmdInitializePlayer();
                    player.GetComponent<NetworkRoomPlayerV3D>().RpcFetchPlayerTower();
                }
            }

            if(NetworkRoomManagerV3D.singleton.TowerPositions is not null)
            {
                IsRunning = false;
                IsComplete = true;
            }
        }

    }

}