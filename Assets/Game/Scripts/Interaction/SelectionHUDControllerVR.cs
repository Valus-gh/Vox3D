using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

using Game.Stages;
using Game.Networking;
using Game.Resources;

namespace Game.Interaction
{ 

    public class SelectionHUDControllerVR : MonoBehaviour
    {

        public void ToggleDisplay(bool show)
        {
            this.gameObject.SetActive(show);
        }

        public void PurchaseAmmo(string ammo)
        {
            Debug.Log("Ammo Purchased");

            var ProjectileTemplates = Vox3D.JSON.JsonImporter<ProjectileResources>.FromJSON("projectiles");

            foreach (var template in ProjectileTemplates.Projectiles)
            {
                if(template.Name == ammo)
                    FindObjectOfType<SelectionStage>().CmdConfirmPurchase(NetworkRoomManagerV3D.singleton.PlayerID, ammo, template.Cost);
            }

        }

        public void FinishPurchasing()
        {
            Debug.Log("Finished Purchasing");

            // Notify server that client is done buying
            FindObjectOfType<SelectionStage>().CmdReadyToProceed();
            ToggleDisplay(false);
        }

    }

}