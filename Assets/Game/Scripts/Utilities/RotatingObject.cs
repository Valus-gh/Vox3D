using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OVR.Input;

namespace Game.Utilities
{
    public class RotatingObject : MonoBehaviour
    {

        [SerializeField] private float _Speed;

        private Vector3 _Axis;
        private Vector3 _AxisUI;

        private bool forUI = true;

        public Vector3 Axis { get => _Axis; set => _Axis = value; }
        public float Speed { get => _Speed; set => _Speed = value; }
        public bool ForUI { get => forUI; set => forUI = value; }

        // Start is called before the first frame update
        void Start()
        {
            Speed = Speed * Time.deltaTime;
            _AxisUI = transform.up;
        }

        // Update is called once per frame
        void Update()
        {
            if(ForUI) transform.RotateAround(transform.position, _AxisUI, Speed);
            else transform.RotateAround(transform.position, Axis, Speed);
        }

    }

}