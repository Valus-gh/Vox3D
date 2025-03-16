using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Vox3D.Networking
{

    public class PlayerMediator : NetworkBehaviour
    {

        #region Singleton and Persistence

        private static PlayerMediator _Instance;

        public static PlayerMediator Instance
        {
            get
            {
                if (_Instance != null)
                    return _Instance;

                var instances = FindObjectsOfType<PlayerMediator>();

                if (instances.Length > 0)
                {
                    Debug.LogWarning($"Multiple instances ({instances.Length}) were found for PlayerMediator Singleton. The first instance will be returned."); ;

                    if (instances.Length > 1)
                    {
                        for (int i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                    }

                    return _Instance = instances[0];

                }

                _Instance = new GameObject("PlayerMediator").AddComponent<PlayerMediator>();
                _Instance.gameObject.AddComponent<NetworkIdentity>();

                return _Instance;

            }

        }

        #endregion


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private List<NetworkRoomPlayer> _Players    = new List<NetworkRoomPlayer>();
        private short _PlayerCount                  = 0;

        private JSON.Vox3DModel _Model;

        internal void AddPlayer(NetworkRoomPlayer player)
        {
            _Players.Add(player);
            _PlayerCount++;
        }

        internal bool LoadModel(string path)
        {
            _Model = JSON.JsonImporter.FromJSON(path);
            if (_Model is not null) return true;

            return false;
        }

        [ClientRpc]
        public void RpcLoadWorldFromModel()
        {
            if(_Model is not null)
                Game.GameManager.Instance.LoadWorldFromModel(_Model);
        }

    }

}