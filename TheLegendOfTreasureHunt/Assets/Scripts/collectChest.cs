using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectChest : MonoBehaviour
{
    public AudioSource source;
    public AudioClip collect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void collectSound()
    {
        source.clip = collect;
        source.Play();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            collectSound();
        }
    }
}
