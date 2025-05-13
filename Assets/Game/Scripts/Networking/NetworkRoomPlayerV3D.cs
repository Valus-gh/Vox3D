using UnityEngine;
using Mirror;
using System.Collections.Generic;

using Vox3D;
using Game.Fog;
using Game.Utilities;
using Game.Resources;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-room-player
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkRoomPlayer.html
*/

namespace Game.Networking
{

    /// <summary>
    /// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
    /// The RoomPrefab object of the NetworkRoomManager must have this component on it.
    /// This component holds basic room player data required for the room to function.
    /// Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.
    /// </summary>
    [RequireComponent(typeof(Player))]
    public class NetworkRoomPlayerV3D : NetworkRoomPlayer
    {

        public Vox3D.JSON.Vox3DModel    Model;
        public Vox3D.Engine.World       World;

        private Player                  _Player;

        public Player Player
        {
            get
            {
                if (_Player is null)
                    _Player = gameObject.GetComponent<Player>();
                
                return _Player;  
            }
        }

        [TargetRpc]
        public void RpcLoadWorld()
        {
            NetworkRoomManagerV3D.singleton.PlayerID = netId;

            Model = FindObjectOfType<ModelHolder>().Model;
            World = Vox3DEngine.FromModel(Model);

            World.PopulateWorld();
            World.PopulateChunks();
            Vox3D.Engine.PriorityCallStack.Instance().Push(() => World.GenerateGeometry(), 60);

        }

        [Command]
        public void CmdInitializePlayer()
        {
            //hardcoded to be the basic type of player
            Player.Populate("Basic", ResourceImporter<PlayerResources>.FromJSON("player"));
        }

        [TargetRpc]
        public void RpcFetchPlayerTower()
        {
            var towers = FindObjectsOfType<PlayerTower>();

            //TODO fetch multiple towers for each player

            foreach (var tower in towers)
            {

                tower.transform.parent = World.transform;

                if(tower.GetComponent<OwnedBy>().OwnerID == NetworkRoomManagerV3D.singleton.PlayerID)
                {

                    Player.Tower = tower;
                    tower.Player = Player;

                    Vox3D.Engine.PriorityCallStack.Instance().Push(() =>
                    {
                        tower.CanFire = true;

                        var manager = NetworkRoomManagerV3D.singleton;
                        if (manager.TowerPositions is null)
                            manager.TowerPositions = TowerLocator.GenerateTowerLocations(World, manager.minPlayers);

                        CmdGenerateTowerLocations(manager.TowerPositions, World.Properties.VoxelSize);

                        // Deactivate FogVisibilityAgent on tower
                        Player.Tower.gameObject.AddComponent<FoWRevealer>().Radius = 7;

                        // Instantiate FogInjector for current world
                        GameObject.Find("FogManager").GetComponent<FogManager>().AttachFoW(World);

                    }, 60);

                }
            }
        }

        [Command]
        public void CmdGenerateTowerLocations(List<Vector3> towerPositions, int voxelSize)
        {
            if(NetworkRoomManagerV3D.singleton.TowerPositions is null)
                NetworkRoomManagerV3D.singleton.TowerPositions = towerPositions;

            var towers = FindObjectsOfType<PlayerTower>();
            if (towers.Length == NetworkRoomManagerV3D.singleton.minPlayers)
            {
                for (int i = 0; i < towers.Length; i++)
                {
                    towers[i].transform.localPosition   = NetworkRoomManagerV3D.singleton.TowerPositions[i];
                    towers[i].transform.localScale      = Vector3.one * (0.5f * voxelSize);
                }
            }
        }

        #region Start & Stop Callbacks

        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer() { }

        /// <summary>
        /// Invoked on the server when the object is unspawned
        /// <para>Useful for saving object data in persistent storage</para>
        /// </summary>
        public override void OnStopServer() { }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
        /// </summary>
        public override void OnStartClient() { }

        /// <summary>
        /// This is invoked on clients when the server has caused this object to be destroyed.
        /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
        /// </summary>
        public override void OnStopClient() { }

        /// <summary>
        /// Called when the local player object has been set up.
        /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
        /// </summary>
        public override void OnStartLocalPlayer() { }

        /// <summary>
        /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
        /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
        /// <para>When <see cref="NetworkIdentity.AssignClientAuthority"/> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnectionToClient parameter included, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStartAuthority() { }

        /// <summary>
        /// This is invoked on behaviours when authority is removed.
        /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStopAuthority() { }

        #endregion

        #region Room Client Callbacks

        /// <summary>
        /// This is a hook that is invoked on all player objects when entering the room.
        /// <para>Note: isLocalPlayer is not guaranteed to be set until OnStartLocalPlayer is called.</para>
        /// </summary>
        public override void OnClientEnterRoom() { }

        /// <summary>
        /// This is a hook that is invoked on all player objects when exiting the room.
        /// </summary>
        public override void OnClientExitRoom() { }

        #endregion

        #region SyncVar Hooks

        /// <summary>
        /// This is a hook that is invoked on clients when the index changes.
        /// </summary>
        /// <param name="oldIndex">The old index value</param>
        /// <param name="newIndex">The new index value</param>
        public override void IndexChanged(int oldIndex, int newIndex) { }

        /// <summary>
        /// This is a hook that is invoked on clients when a RoomPlayer switches between ready or not ready.
        /// <para>This function is called when the a client player calls SendReadyToBeginMessage() or SendNotReadyToBeginMessage().</para>
        /// </summary>
        /// <param name="oldReadyState">The old readyState value</param>
        /// <param name="newReadyState">The new readyState value</param>
        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState) { }

        #endregion

        #region Optional UI

        public override void OnGUI()
        {
            base.OnGUI();
        }

        #endregion
    }

}