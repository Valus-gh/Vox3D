using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Interaction;
using Game.Weapons;
using Game.Resources;
using Game.Utilities;
using Game.Stages;

//TODO: extract health into interface

namespace Game
{
    public class PlayerTower : NetworkBehaviour
    {
        private Player  _Player;
        [SyncVar(hook = nameof(OnBaseHitpointsChanged))]
        private float   _BaseHitpoints;
        [SyncVar(hook = nameof(OnCurrentHitpointsChanged))]
        private float   _CurrentHitpoints;

        [SyncVar] public int  TowerID;
        [SyncVar] public bool CanFire;
        [SyncVar] public bool IsDestroyed;

        [SerializeField] private ParticleSystem _SmokeParticles;

        [SerializeField]
        private Projectile                          _EquippedWeapon;
        private ProjectileResources.ProjectileModel _WeaponTemplate;
        private TowerControlsMultiplayer            _TowerControls;

        public Player Player                                        { get => _Player; set => _Player = value; }
        public TowerControlsMultiplayer TowerControls               { get => _TowerControls; private set => _TowerControls = value; }
        public float BaseHitpoints                                  { get => _BaseHitpoints; set => _BaseHitpoints = value; }
        public float CurrentHitpoints                               { get => _CurrentHitpoints; set => _CurrentHitpoints = value; }
        public Projectile EquippedWeapon                            { get => _EquippedWeapon; set => _EquippedWeapon = value; }
        public ProjectileResources.ProjectileModel WeaponTemplate   { get => _WeaponTemplate; set => _WeaponTemplate = value; }

        public void Start()
        {
            CanFire = false;
            IsDestroyed = false;

            TowerControls = GetComponentInChildren<TowerControlsMultiplayer>();
            TowerControls.Tower = this;
        }

        public void Update()
        {
            if (!TowerControls.enabled && CanFire)
                TowerControls.enabled = true;
        }

        void OnBaseHitpointsChanged(float oldValue, float newValue)
        {
            Debug.Log("BaseHitpoints: " + BaseHitpoints);
        }
        void OnCurrentHitpointsChanged(float oldValue, float newValue)
        {
            Debug.Log($"CurrentHitpoints [tower {TowerID}]: " + CurrentHitpoints);

            if (newValue <= 0)
            {
                Destroy();
                Debug.Log("Tower has lost all hitpoints. Send destruction event");
            }
        }

        private void Destroy()
        {
            IsDestroyed = true;
            CanFire = false;
            FindObjectOfType<ShootingStage>().TowerDestroyed();
            _SmokeParticles.Play();
        }
    }

}