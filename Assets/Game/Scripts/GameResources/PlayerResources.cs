using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Resources
{
    public class PlayerResources
    {

        [System.Serializable]
        public class PlayerModel
        {
            public string   Name;
            public uint     BaseHitpoints;
            public float    HitpointMultiplier;

            public PlayerModel() { }
            public PlayerModel(string name, uint baseHitpoints, float hitpointMultiplier)
            {
                Name = name;
                BaseHitpoints = baseHitpoints;
                HitpointMultiplier = hitpointMultiplier;
            }
        }

        public PlayerModel[] Players;

    }

}