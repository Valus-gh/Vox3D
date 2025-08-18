using UnityEngine;

using Vox3D.Engine;

namespace Game.Fog
{
    /// <summary>
    /// Utility class to attach a FogInjector to the given world.
    /// Originally held the contents of Foginjector
    /// </summary>
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