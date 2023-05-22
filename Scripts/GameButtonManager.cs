using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script for buttons in the scene
public class GameButtonManager : MonoBehaviour
{
    [SerializeField] PlayerReset pR; // Reset position script

    // Function for resetting the view - to prepare to play space should it bug out
    public void ResetView(){
        pR.ResetPosition();
        //SceneManager.LoadSceneAsync(1);
    }

    // Function for returning to the main menu
    public void MainMenu(){
        SceneManager.LoadSceneAsync(0);
    }

    // Function for closing the game
    public void QuitGame(){
        Application.Quit();
    }
}
