using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    private float timeToLive = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(timeToLive));
    }

   IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
    }
}
