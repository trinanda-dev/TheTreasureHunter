using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class killPlayer : MonoBehaviour
{
    public GameObject Player;
    public Transform spawnPoint;
    public int respawn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            Destroy(col.gameObject);
            StartCoroutine("Respawn", 2f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    IEnumerator Respawn(float spawnDelay)
    {
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(Player, spawnPoint.position, spawnPoint.rotation);
    }
}
