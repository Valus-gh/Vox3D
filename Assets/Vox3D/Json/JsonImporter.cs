using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vox3D.JSON
{
    public class JsonImporter : MonoBehaviour
    {
        public static Vox3DModel FromJSON(string path)
        {
            TextAsset json = Resources.Load(path, typeof(TextAsset)) as TextAsset;
            return JsonUtility.FromJson<Vox3DModel>(json.text);
        }
        public static string JSONToString(Vox3DModel model)
        {
            return JSONToString(model);
        }
    }
}