using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class bacKScene : MonoBehaviour
{
    public void backScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -2 );
    }

    public void keluarGame(){
        Application.Quit();
        Debug.Log("keluar");
    }
 
}
