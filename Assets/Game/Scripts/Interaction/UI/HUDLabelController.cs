using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Game.Interaction
{
    public class HUDLabelController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _AngleLabel;
        [SerializeField] private TextMeshProUGUI _RotationLabel;

        private TowerSelector _Selector;

        // Start is called before the first frame update
        void Start()
        {
            _Selector = FindObjectOfType<TowerSelector_Quest>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_Selector.SelectedTower is null) return;

            var controls = _Selector.SelectedTower.GetComponentInChildren<TowerControlsMultiplayerQuest>();

            _AngleLabel.text = controls.TowerTrajectory.Angle.ToString();
            _RotationLabel.text = controls.transform.eulerAngles.y.ToString();
        }
    }
}