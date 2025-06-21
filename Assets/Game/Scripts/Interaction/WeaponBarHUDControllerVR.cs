using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game.Resources;
using Game.Stages;

namespace Game.Interaction
{
    public class WeaponBarHUDControllerVR : MonoBehaviour
    {
        private List<Button> _WeaponBarButtons;

        public void ToggleDisplay(bool show)
        {
            this.gameObject.SetActive(show);
        }

        public void ToggleClickable(bool clickable)
        {
            foreach (var button in _WeaponBarButtons)
            {
                var nameLabel = button.transform.Find("Label-Ammo").GetComponent<TMPro.TextMeshProUGUI>();

                if (nameLabel.text == "0")
                    button.transform.Find("Background").GetComponent<Image>().color = Color.red;
                else
                    button.transform.Find("Background").GetComponent<Image>().color = (clickable) ? Color.white : Color.red;
            }
        }

        public void ToggleClickable(Button control, bool clickable)
        {
            control.transform.Find("Background").GetComponent<Image>().color = (clickable) ? Color.white : Color.red;
        }

        public void InitializeHUD(ProjectileResources resources)
        {
            _WeaponBarButtons = new List<Button>(GetComponentsInChildren<Button>());

            ToggleClickable(false);
        }

        public void UpdateHUD(Inventory inventory)
        {
            foreach (var button in _WeaponBarButtons)
            {
                var nameLabel = button.transform.Find("Label").GetComponent<TMPro.TextMeshProUGUI>();

                foreach (var item in inventory.Projectiles)
                {
                    if (nameLabel.text == item.Key)
                    {
                        var ammoLabel = button.transform.Find("Label-Ammo").GetComponent<TMPro.TextMeshProUGUI>();

                        // If the player bought ammo for this weapon, register callback
                        // If the player has run out of ammo for this weapon, unregister callback
                        if (item.Value >= 1 && ammoLabel.text == "0")
                        {
                            button.interactable = true;
                        }
                        else if (item.Value == 0 && ammoLabel.text != "0")
                        {
                            button.interactable = false;
                        }

                        ammoLabel.text = item.Value.ToString();
                    }
                }
            }
        }

        public void OnClickEquipWeapon(string name)
        {
            // Equip weapon on given tower
            Debug.Log("Clicked on weapon picker - " + name);

            FindObjectOfType<ShootingStage>().Selectweapon(name);
        }
    }
}