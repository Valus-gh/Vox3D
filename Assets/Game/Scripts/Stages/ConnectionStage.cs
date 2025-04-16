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
            foreach (var player in Players)
            {
                player.GetComponent<NetworkRoomPlayerV3D>().RpcLoadWorld();

                var playerTower = Instantiate(NetworkRoomManagerV3D.singleton.spawnPrefabs[1]);
                playerTower.GetComponent<OwnedBy>().OwnerID = player.GetComponent<NetworkIdentity>().netId;
                NetworkServer.Spawn(playerTower);

                player.GetComponent<NetworkRoomPlayerV3D>().CmdInitializePlayer();
                player.GetComponent<NetworkRoomPlayerV3D>().RpcFetchPlayerTower();
            }

            IsRunning = false;
            IsComplete = true;
        }

    }

}