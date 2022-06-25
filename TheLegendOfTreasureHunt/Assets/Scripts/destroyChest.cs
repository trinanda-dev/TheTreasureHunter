using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyChest : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            Destroy(gameObject, 1);
        }
    }
}
