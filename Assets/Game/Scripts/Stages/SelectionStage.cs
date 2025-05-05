using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using Game.Resources;
using Game.Interaction;
using Game.Utilities;

namespace Game.Stages
{
    public class SelectionStage : GameStage
    {

        [SerializeField]
        private GameObject _SelectionStageHUD;
        private GameObject _SelectionStageHUDInstance;

        private ProjectileResources _ProjectileTemplates;
        private Dictionary<uint, PlayerTower> _Towers;

        public override void Initialize()
        {
            if (!IsInitialized)
            {
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

                _Towers = new Dictionary<uint, PlayerTower>();
                var towers = FindObjectsOfType<PlayerTower>();

                foreach (var tower in towers)
                {
                    var ownerID = tower.GetComponent<OwnedBy>().OwnerID;
                    _Towers.Add(ownerID, tower);
                }

                IsInitialized = true;

                RpcLoadHUD();
            }

        }

        public override void Deinitialize()
        {
            throw new System.NotImplementedException();
        }
        protected override void Run()
        {
        }

        [ClientRpc]
        private void RpcLoadHUD() 
        {
            if(_ProjectileTemplates is null)
                _ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

            _SelectionStageHUDInstance = Instantiate(_SelectionStageHUD, this.transform);
            _SelectionStageHUDInstance.GetComponent<SelectionHUDController>().InitializeHUD(_ProjectileTemplates);
        }
    }

}