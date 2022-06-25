using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostSound : MonoBehaviour
{
    public AudioSource source;
    public AudioClip ghost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ghostSoundClip()
    {
        source.clip = ghost;
        source.Play();
    }

    void OnTriggerEnter (Collider col)
    {
        if(col.CompareTag ("Player"))
        {
            ghostSoundClip();
        }
    }
}
