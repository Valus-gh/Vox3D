using System.Collections;
using System.Collections.Generic;
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

                if (instances.Length > 0)
                {
                    Debug.LogWarning($"Multiple instances ({instances.Length}) were found for GameManager Singleton. The first instance will be returned."); ;

                    if (instances.Length > 1)
                    {
                        for (int i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                    }

                    return _Instance = instances[0];

                }

                return _Instance = new GameObject("PlayerMediator").AddComponent<GameManager>();

            }

        }

        #endregion

        [System.NonSerialized]
        public World World;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"GameManager {name} started.");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadWorldFromModel(Vox3DModel model)
        {
            World = Vox3DEngine.FromModel(model);
            World.PopulateWorld();
            World.PopulateChunks();
            PriorityCallStack.Instance().Push(() => World.GenerateGeometry(), 60);
            Debug.Log($"World {World.name} loaded.");
        }

    }

}