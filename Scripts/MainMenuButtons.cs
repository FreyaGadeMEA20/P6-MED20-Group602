using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script for the main menu buttons
public class MainMenuButtons : MonoBehaviour
{
    public GameObject buttons; // canvas for the buttons
    public GameObject loading; // canvas for the loading text

    // Function for starting the game
    //  - deactivates the buttons and activates the loading when the player presses the Start Game button.
    public void StartGame(){
        buttons.SetActive(false);
        loading.SetActive(true);
        SceneManager.LoadSceneAsync(1);
    }

    // Function for quitting the game
    public void QuitGame(){
        Application.Quit();
    }
}
