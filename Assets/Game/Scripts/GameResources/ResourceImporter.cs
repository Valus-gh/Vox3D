using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Resources
{

    public class ResourceImporter<T>
    {
        public static T FromJSON(string path)
        {
            return Vox3D.JSON.JsonImporter<T>.FromJSON(path);
        }
    }

}