using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BTNManager : MonoBehaviour
{
    public void BackScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1 );
    }

    public void KeluarGame(){
        Application.Quit();
        Debug.Log("keluar");
    }
 
}
