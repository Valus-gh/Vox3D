using System.Collections;
using UnityEngine;

public class DestroyAfterInterval : MonoBehaviour
{

    public float Interval = 10.0f;

    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(Interval);
        Destroy(gameObject);
    }
}
