using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Mirror;

using Game.Resources;
using Game.Stages;
using Game.Networking;

namespace Game.Interaction
{

    [RequireComponent(typeof(UIDocument))]
    public class SelectionHUDController : MonoBehaviour
    {
        private VisualTreeAsset _WeaponSelectionButtonTemplate;

        private GroupBox _ButtonContainer;

        private void OnEnable()
        {
        }
        
        public void ToggleDisplay(bool show)
        {
            GetComponent<UIDocument>().rootVisualElement.Q("root-container").visible = show;
        }

        public void InitializeHUD(ProjectileResources resources)
        {
            _WeaponSelectionButtonTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("UI/weapon-selection-button");
            _ButtonContainer = GetComponent<UIDocument>().rootVisualElement.Q("button-container") as GroupBox;

            foreach(var item in resources.Projectiles)
            {
                var weaponButton    = _WeaponSelectionButtonTemplate.Instantiate();
                
                var nameLabel       = weaponButton.Q("name-label") as Label;
                var costLabel       = weaponButton.Q("cost-label") as Label;
                var damageLabel     = weaponButton?.Q("damage-label") as Label;

                nameLabel.text      = item.Name;
                costLabel.text      = "0";
                damageLabel.text    = "Damage: " + item.Blast.Damage + ((item.Blast.Scatter) ? " x " + item.Blast.ScatterAmount : "");

                if (item.Blast.Scatter)
                {
                    var scatteredProjectile = item.Blast.Child;
                    for(int i = 0; i < resources.Projectiles.Length; i++)
                    {
                        if (resources.Projectiles[i].Name.Equals(scatteredProjectile))
                        {
                            damageLabel.text = resources.Projectiles[i].Blast.Damage + " x " + item.Blast.ScatterAmount;
                            break;
                        }
                    }
                }
                else
                {
                    damageLabel.text = "Damage: " + item.Blast.Damage;
                }

                weaponButton.RegisterCallback<ClickEvent>(ButtonPressed);

                _ButtonContainer.Add(weaponButton);

            }
        }

        private void ButtonPressed(ClickEvent evt)
        {
            var button = evt.target as Button;
            var weaponName = (button.Q("name-label") as Label).text;
            var weaponCost = (button.Q("cost-label") as Label).text;
            FindObjectOfType<SelectionStage>().CmdConfirmPurchase(NetworkRoomManagerV3D.singleton.PlayerID, weaponName, uint.Parse(weaponCost));
        }

    }

}