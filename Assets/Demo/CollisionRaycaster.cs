using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    public class CollisionRaycaster : MonoBehaviour
    {

        private RaycastHit _hit;
        public float radius;
        private float _offset;

        // Update is called once per frame
        void Update()
        {
            // Move this object to the position clicked by the mouse.
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 500.0f, LayerMask.GetMask("Default")))
                {
                    Debug.Log($"Ray hit at distance {hit.distance}");
                    Debug.Log($"Ray hit chunk {hit.collider.GetComponentInParent<Vox3D.Chunk>().name}");

                    float offset = Vox3D.Vox3DManager.Instance().Properties.VoxelSize;
                    Vox3D.ChunkCollisionHandler.Instance().CollisionSphere(hit.point, radius, offset);

                    _hit = hit;
                    _offset = offset;

                }
                else Debug.Log("Ray missed");
                
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(_hit.point, radius);

            Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(_hit.point, radius + _offset);
        }

    }

}