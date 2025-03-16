using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Vox3D;
using Vox3D.Engine;
using Vox3D.JSON;

//TODO Extract singleton pattern into generic Singleton interface

namespace Game
{

    public class GameManager : MonoBehaviour
    {
        #region Singleton and Persistence

        private static GameManager _Instance;

        public static GameManager Instance
        {
            get
            {
                if (_Instance != null)
                    return _Instance;

                var instances = FindObjectsOfType<GameManager>();

                if (instances.Length == 0) return null;
                else _Instance = instances[0];

                return _Instance;
                
            }

        }

        #endregion

        [System.NonSerialized]
        public World            World;
        public List<GameObject> Towers;
        public Vox3DModel       Model;
        public string           ConfigPath;

        public bool             HasLoaded;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"GameManager {name} started.");
        }

        // Update is called once per frame
        void Update()
        {
            if(World is not null) HasLoaded = World.ChunksReady;
        }

        public void LoadWorldFromModel()
        {
            Model = JsonImporter.FromJSON(ConfigPath);
            World = Vox3DEngine.FromModel(Model);
            World.PopulateWorld();
            World.PopulateChunks();
            PriorityCallStack.Instance().Push(() => World.GenerateGeometry(), 60);
        }


    }

}