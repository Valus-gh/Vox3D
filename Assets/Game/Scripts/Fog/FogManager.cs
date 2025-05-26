using UnityEngine;

using Vox3D.Engine;

namespace Game.Fog
{
    public class FogManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _InjectorPrefab;

        private bool _Attached;
        public void AttachFoW(World world)
        {
            if (_Attached) return;
            Instantiate(_InjectorPrefab, world.transform);
        }

    }

}