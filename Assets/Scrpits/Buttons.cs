using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;



public class Buttons : MonoBehaviour
{

    public GameObject menuPausa;
    public GameObject panelLibroRecetas;

    public bool isPaused = false;

    
    public void startGame_Btn()
    { 
        SceneManager.LoadScene("Game");
    }

    public void titleScreen_Btn()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void ResumeGame()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void PauseGame()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0; //Esto pausaria luego el juego (tipo, el tiempo) cuando tengamos algo xd osea pone la velocidad al 0
        isPaused = true;
    }

    public void AbrirLibroRecetas()
    {
        panelLibroRecetas.SetActive(true);
    }

    public void CerrarLibroRecetas()
    {
        panelLibroRecetas.SetActive(false);
    }
}
