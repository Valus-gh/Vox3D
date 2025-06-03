using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Resources;
using Game.Utilities;
using Game.Stages;

namespace Game
{

    public class Player : NetworkBehaviour
    {

        private List<PlayerTower> _Towers;
        public List<PlayerTower> Towers { get => _Towers; set => _Towers = value; }

        [SerializeField]
        private Inventory _Inventory;
        public Inventory Inventory { get => _Inventory; set => _Inventory = value; }

        #region PlayerAttributes

        [SyncVar(hook = nameof(OnBaseHitpointsChanged))]
        private float   _BaseHitpoints;

        [SyncVar(hook = nameof(OnHitpointMultiplierChanged))]
        private float   _HitpointMultiplier;

        [SyncVar(hook = nameof(OnEffectiveHitpointsChanged))]
        private float   _EffectiveHitpoints;

        [SyncVar(hook = nameof(OnCurrentHitpointsChanged))]
        private float   _CurrentHitpoints;

        [SyncVar(hook = nameof(OnBudgetChanged))]
        private int     _Budget;

        public float BaseHitpoints      { get => _BaseHitpoints; set => _BaseHitpoints = value; }
        public float HitpointMultiplier { get => _HitpointMultiplier; set => _HitpointMultiplier = value; }
        public float EffectiveHitpoints { get => _EffectiveHitpoints; set => _EffectiveHitpoints = value; }
        public float CurrentHitpoints   { get => _CurrentHitpoints; set => _CurrentHitpoints = value; }
        public int Budget               { get => _Budget; set => _Budget = value; }

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

        void OnBaseHitpointsChanged(float oldValue, float newValue)
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

            if (newValue <= 0)
                Debug.Log("Player has lost all hitpoints. Send defeat event");
        }
        void OnBudgetChanged(int oldValue, int newValue)
        {
            Debug.Log("Budget: " + Budget);
        }

        public void InitializeTowerHitpoints()
        {
            foreach(var tower in _Towers)
            {
                tower.BaseHitpoints     = BaseHitpoints / StageManager.TowersPerPlayer;
                tower.CurrentHitpoints  = tower.BaseHitpoints;
            }
        }

        public void DamageTowerWithId(int towerID, float damage)
        {
            _Towers.Find((t) => t.TowerID == towerID).CurrentHitpoints -= damage;
        }

        [TargetRpc]
        public void RpcPreserveVoxels(bool preserve)
        {
            foreach (var tower in _Towers)
            {
                tower.GetComponentInChildren<VoxelPreserver>().PreserveVoxels(preserve);
            }
        }

        #endregion

    }

}