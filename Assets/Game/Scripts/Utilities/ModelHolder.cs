using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Vox3D.Engine;

namespace Game.Utilities
{
    public class ModelHolder : NetworkBehaviour
    {
        public Vox3D.JSON.Vox3DModel Model;

        public void Awake()
        {
            DontDestroyOnLoad(this);
        }


    }

}