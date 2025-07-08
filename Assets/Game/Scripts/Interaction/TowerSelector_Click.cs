using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game.Stages;

namespace Game.Interaction
{
    public class TowerSelector_Click : TowerSelector
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Player")))
                {
                    var hitTransform = hit.transform;

                    Debug.Log("ray hit " + hitTransform.gameObject.name);

                    if (IsTower(hitTransform.gameObject) && IsOwnedByLocalPlayer(hitTransform.gameObject))
                    {
                        HighlightTower(false);

                        var tower = hitTransform.gameObject.GetComponentInParent<PlayerTower>();
                        if (tower.IsDestroyed) return;

                        SelectedTower = tower;
                        FindObjectOfType<WeaponBarHUDController>().ToggleClickable(true);
                        HighlightTower(true);
                    }

                }
                else
                {
                    HighlightTower(false);
                    FindObjectOfType<WeaponBarHUDController>().ToggleClickable(false);
                }
            }
        }

    }
}