using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Resources;

namespace Game
{

    public class Player : NetworkBehaviour
    {

        private PlayerTower _Tower;
        public PlayerTower  Tower { get => _Tower; set => _Tower = value; }

        #region PlayerAttributes

        [SyncVar(hook = nameof(OnBaseHitpointsChanged))]
        private uint    _BaseHitpoints;

        [SyncVar(hook = nameof(OnHitpointMultiplierChanged))]
        private float   _HitpointMultiplier;

        [SyncVar(hook = nameof(OnEffectiveHitpointsChanged))]
        private float   _EffectiveHitpoints;

        [SyncVar(hook = nameof(OnCurrentHitpointsChanged))]
        private float   _CurrentHitpoints;

        [SyncVar(hook = nameof(OnBudgetChanged))]
        private uint    _Budget;

        public uint     BaseHitpoints { get => _BaseHitpoints; set => _BaseHitpoints = value; }
        public float    HitpointMultiplier { get => _HitpointMultiplier; set => _HitpointMultiplier = value; }
        public float    EffectiveHitpoints { get => _EffectiveHitpoints; set => _EffectiveHitpoints = value; }
        public float    CurrentHitpoints { get => _CurrentHitpoints; set => _CurrentHitpoints = value; }
        public uint     Budget { get => _Budget; set => _Budget = value; }

        public void Populate(string name, PlayerResources resources)
        {
            foreach(var item in resources.Players)
            {
                if(item.Name == name)
                {
                    BaseHitpoints       = item.BaseHitpoints;
                    HitpointMultiplier  = item.HitpointMultiplier;
                    EffectiveHitpoints  = BaseHitpoints * HitpointMultiplier;
                    CurrentHitpoints    = EffectiveHitpoints;
                }
            }
        }

        void OnBaseHitpointsChanged(uint oldValue, uint newValue)
        {
            Debug.Log("BaseHitpoints: " + BaseHitpoints);
        }
        void OnHitpointMultiplierChanged(float oldValue, float newValue)
        {
            Debug.Log("HitpointMultiplier: " + HitpointMultiplier);
        }
        void OnEffectiveHitpointsChanged(float oldValue, float newValue)
        {
            Debug.Log("EffectiveHitpoints: " + EffectiveHitpoints);
        }
        void OnCurrentHitpointsChanged(float oldValue, float newValue)
        {
            Debug.Log("CurrentHitpoints: " + CurrentHitpoints);
        }
        void OnBudgetChanged(uint oldValue, uint newValue)
        {
            Debug.Log("Budget: " + Budget);
        }

        #endregion

    }

}