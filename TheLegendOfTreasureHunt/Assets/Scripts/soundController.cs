using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundController : MonoBehaviour
{
    public AudioSource source;
    public AudioClip attacking;
    public AudioClip footStep;
    public AudioClip Jump;
    public AudioClip ghost;

    
    void attackSound()
    {
        source.clip = attacking;
        source.Play();
    }

    void footSound()
    {
        source.clip = footStep;
        source.Play();
    }

    void jumpSound()
    {
        source.clip = Jump;
        source.Play();
    }

    void ghostSound()
    {
        source.clip = ghost;
        source.Play();
    }

    

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            ghostSound();
        }
    }
}
