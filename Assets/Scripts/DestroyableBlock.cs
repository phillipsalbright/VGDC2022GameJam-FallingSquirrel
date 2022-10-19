using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableBlock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Debug.Log("Bingy " + collision.relativeVelocity.y);
        }
        if (collision.gameObject.layer == 7 && collision.relativeVelocity.y <= -10)
        {
            Destroy(this.gameObject);
        }
    }
}
