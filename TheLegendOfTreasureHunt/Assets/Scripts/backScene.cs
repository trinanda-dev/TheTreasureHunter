using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backScene : MonoBehaviour
{
    public void bacKScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -3 );
    }

    public void keluarGame(){
        Application.Quit();
        Debug.Log("keluar");
    }
 
}
