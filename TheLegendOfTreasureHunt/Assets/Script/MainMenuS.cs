using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuS : MonoBehaviour
{
 public void MuliaGame(){
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
 }
 
 public void KeluarGame(){
    Application.Quit();
    Debug.Log("keluar");
 }
}
