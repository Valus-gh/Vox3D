using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Game.Interaction
{
    public class TowerSelector_Quest : TowerSelector
    {
        private List<GrabRotationController> _Grabbables;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(_Grabbables is null || _Grabbables.Count == 0)
            {
                _Grabbables = Enumerable.ToList(FindObjectsOfType<GrabRotationController>());
                _Grabbables.ForEach(grabbable => grabbable.ExtraEventStart.AddListener(HighlightOnGrab));
            }
        }

        private void HighlightOnGrab(GrabRotationController grabbable)
        {
            if (enabled == false) return;

            if(grabbable.Tower.IsDestroyed) return;

            HighlightTower(false);
            SelectedTower = grabbable.Tower;
            var wbar = FindObjectOfType<WeaponBarHUDControllerVR>();
            if(wbar != null) wbar.ToggleClickable(true);
            HighlightTower(true);
        }

    }

}