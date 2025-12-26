using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string cena;
    public string menu;

    public void StartGame()
    {
        SceneManager.LoadScene(cena);
    }
            
            
    public void HowToPlay()
    {
        
        SceneManager.LoadScene(menu);
    
    }
    public void QuitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
