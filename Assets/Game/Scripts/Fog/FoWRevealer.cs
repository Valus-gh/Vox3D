using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Fog
{
    public class FoWRevealer : MonoBehaviour
    {
        [HideInInspector]
        public int ID;

        public int Radius = 5;

        public bool Revealing = false;
    }

}