using UnityEngine;

using Vox3D.Engine;
using FischlWorks_FogWar;

public class FogManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _InjectorPrefab;

    public void AttachFoW(World world)
    {
        Instantiate(_InjectorPrefab, world.transform);
    }

}
