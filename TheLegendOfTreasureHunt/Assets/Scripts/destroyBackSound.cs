using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyBackSound : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag ("soundDestroy"))
        {
            Destroy(gameObject);
        }
    }
}
