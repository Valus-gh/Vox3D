using Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Stages
{
    public class StageQueue : CircularQueue<GameStage>
    {
        public StageQueue(int size) : base(size) { }

        private int _CurrentStage;

        public GameStage Advance()
        {
            // interrupt current stage, start next stage, update index
            Stop();

            _CurrentStage = (_CurrentStage == _Back) ? _CurrentStage = _Front : ++_CurrentStage;

            _Items[_CurrentStage].Initialize();
            _Items[_CurrentStage].IsRunning = true;

            return _Items[_CurrentStage];

        }

        public GameStage Start()
        {
            // Interrupt current stage, reset index, start first stage
            Stop();

            _CurrentStage = _Front;
            _Items[_CurrentStage].Initialize();
            _Items[_CurrentStage].IsComplete = false;
            _Items[_CurrentStage].IsRunning = true;

            return _Items[_CurrentStage];
        }

        public GameStage Stop()
        {
            // Interrupt current stage
            _Items[_CurrentStage].IsRunning = false;
            _Items[_CurrentStage].IsComplete = false;

            return _Items[_CurrentStage];
        }

        public void Initialize(List<GameObject> players)
        {
            //Initialize all stages. Not sure if possible yet
            for(int i = 0; i < _Count; i++)
            {
                _Items[i].Players = players;
                //_Items[i].Initialize();
            }
        }

        public void Deinitialize()
        {
            //Initialize all stages. Not sure if possible yet
        }
    }
}