using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OVR.Input;

namespace Game.Utilities
{
    public class RotatingObject : MonoBehaviour
    {

        [SerializeField] private float _Speed;

        // Start is called before the first frame update
        void Start()
        {
            _Speed = _Speed * Time.deltaTime;
        }

        // Update is called once per frame
        void Update()
        {
            transform.RotateAround(transform.position, transform.up, _Speed);
        }
    }

}