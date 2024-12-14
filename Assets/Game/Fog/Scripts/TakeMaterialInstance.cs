using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeMaterialInstance : MonoBehaviour
{
    Material material;
    public Projector projector;

    // Update is called once per frame
    void Update()
    {
        if (material == null)
        {
            var o = GameObject.Find("[RUNTIME] Fog_Plane");
            material = o.GetComponent<MeshRenderer>().material;
            o.SetActive(false);
        }

        projector.material = material;
    }
}
