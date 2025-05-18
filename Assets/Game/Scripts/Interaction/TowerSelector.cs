using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game.Utilities;
using Game.Networking;

using UnityFx.Outline;

namespace Game.Interaction
{

    public class TowerSelector : MonoBehaviour
    {
        [SerializeField]
        private OutlineResources _OutlineResources;

        protected PlayerTower selectedTower;

        public PlayerTower SelectedTower { get => selectedTower; protected set => selectedTower = value; }

        protected bool IsTower(GameObject go)
        {
            if (go.CompareTag("Tower")) return true;

            return false;
        }

        protected bool IsOwnedByLocalPlayer(GameObject tower)
        {
            var owner = tower.GetComponentInParent<OwnedBy>();

            if(owner is not null)
                return owner.OwnerID == NetworkRoomManagerV3D.singleton.PlayerID;

            return false;
        }

        protected void HighlightTower(bool activate)
        {
            if(SelectedTower is not null)
            {
                if (activate)
                {
                    if (SelectedTower.GetComponent<OutlineBehaviour>() is not null)
                    {
                        SelectedTower.GetComponent<OutlineBehaviour>().enabled = true;
                        return;
                    }

                    var outliner = SelectedTower.gameObject.AddComponent<OutlineBehaviour>();

                    outliner.OutlineResources = _OutlineResources;

                    outliner.OutlineColor = Color.white;
                    outliner.OutlineWidth = 5;
                }
                else
                {
                    SelectedTower.gameObject.GetComponent<OutlineBehaviour>().enabled = false;
                }
            }
        }

        private void OnDisable()
        {
            HighlightTower(false);
        }
    }

}