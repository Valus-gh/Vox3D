using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vox3D.JSON
{
    public class JsonImporter<T>
    {
        public static T FromJSON(string path)
        {
            TextAsset json = Resources.Load(path, typeof(TextAsset)) as TextAsset;
            return JsonUtility.FromJson<T>(json.text);
        }
        public static string JSONToString(T model)
        {
            return JSONToString(model);
        }
    }
}