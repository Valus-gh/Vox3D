using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using Game.Resources;
using Game.Stages;

namespace Game.Interaction
{
    public class WeaponBarHUDController : MonoBehaviour
    {
        private VisualTreeAsset _WeaponBarButtonTemplate;

        private VisualElement _WeaponBar;
        private List<VisualElement> _WeaponBarButtons;

        public void ToggleDisplay(bool show)
        {
            GetComponent<UIDocument>().rootVisualElement.Q("root-container").visible = show;

            List<Label> labels = GetComponent<UIDocument>().rootVisualElement.Query<Label>().ToList();
            labels.ForEach(l => l.visible = show);
        }

        public void ToggleClickable(bool clickable)
        {
            foreach(var button in _WeaponBarButtons)
            {
                var nameLabel = button.Q("ammo-count-label") as Label;

                if(nameLabel.text == "0")
                    button.style.opacity = 0.5f;
                else
                    button.style.opacity = (clickable) ? 1 : 0.5f;
            }
        }

        public void ToggleClickable(VisualElement control, bool clickable)
        {
            control.style.opacity = (clickable) ? 1 : 0.5f;
        }

        public void InitializeHUD(ProjectileResources resources)
        {
            _WeaponBarButtonTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("UI/weapon-picker-button");
            _WeaponBar = GetComponent<UIDocument>().rootVisualElement.Q("weapon-picker-bar");

            _WeaponBarButtons = new List<VisualElement>();

            foreach (var item in resources.Projectiles)
            {
                var weaponButton = _WeaponBarButtonTemplate.Instantiate();

                var nameLabel = weaponButton.Q("weapon-name-label") as Label;
                var ammoLabel = weaponButton.Q("ammo-count-label") as Label;
                nameLabel.text = item.Name;
                ammoLabel.text = "0";

                _WeaponBarButtons.Add(weaponButton);
                _WeaponBar.Add(weaponButton);
            }
            ToggleClickable(false);
        }

        public void UpdateHUD(Inventory inventory)
        {
            foreach(var button in _WeaponBarButtons)
            {
                var nameLabel = button.Q("weapon-name-label") as Label;

                foreach (var item in inventory.Projectiles)
                {
                    if(nameLabel.text == item.Key)
                    {
                        var ammoLabel = button.Q("ammo-count-label") as Label;

                        // If the player bought ammo for this weapon, register callback
                        // If the player has run out of ammo for this weapon, unregister callback
                        if(item.Value >= 1 && ammoLabel.text == "0")
                        {
                            button.RegisterCallback<ClickEvent>(OnClickEquipWeapon);
                        }else if (item.Value == 0 && ammoLabel.text != "0")
                        {
                            button.UnregisterCallback<ClickEvent>(OnClickEquipWeapon);
                        }

                        ammoLabel.text = item.Value.ToString();
                    }
                }
            }
        }

        private void OnClickEquipWeapon(ClickEvent evt)
        {
            // Equip weapon on given tower
            Debug.Log("Clicked on weapon picker - " + (evt.target as Button).name);

            var nameLabel = (evt.target as Button).Q("weapon-name-label") as Label;

            FindObjectOfType<ShootingStage>().Selectweapon(nameLabel.text);
        }
    }
}