using System.Collections;
using UnityEngine;

public class Despawner : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(8f);
        Destroy(this.gameObject);
    }
}
