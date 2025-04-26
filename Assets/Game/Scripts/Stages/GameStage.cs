using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace Game.Stages
{
    public abstract class GameStage : NetworkBehaviour
    {

        private List<GameObject> _Players;

        public bool IsInitialized   = false;
        public bool IsRunning       = false;
        public bool IsComplete      = false;

        public List<GameObject> Players { get => _Players; set => _Players = value; }

        public abstract void Initialize();
        public abstract void Deinitialize();
        protected abstract void Run();

        // Update is called once per frame
        void Update()
        {
            if (IsRunning) Run();
        }
    }

}