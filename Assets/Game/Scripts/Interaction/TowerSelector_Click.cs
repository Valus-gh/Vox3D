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
                    var transform = hit.transform;

                    Debug.Log("ray hit " + transform.gameObject.name);

                    if (IsTower(transform.gameObject) && IsOwnedByLocalPlayer(transform.gameObject))
                    {
                        HighlightTower(false);
                        SelectedTower = transform.gameObject.GetComponentInParent<PlayerTower>();
                        HighlightTower(true);
                    }
                }
                else
                {
                    HighlightTower(false);
                }
            }
        }
    }
}