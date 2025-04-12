using UnityEngine;

using Vox3D.Engine;
using Game.Fog.FischlWorks;

namespace Game.Fog
{
    public class FogManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _InjectorPrefab;

        public void AttachFoW(World world)
        {
            Instantiate(_InjectorPrefab, world.transform);
        }

    }

}