using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class welcomeSounds : MonoBehaviour
{
    public AudioSource source;
    public AudioClip welcome;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void welcomeSound ()
    {
        source.clip = welcome;
        source.Play();
    }

    

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            welcomeSound();
        }
    }
}
