using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceBrain : MonoBehaviour
{
    // Outside information it needs to function
    [SerializeField] PlayerController playerController; // Reference to player
    [SerializeField] AudioSource audioSource;           // It's audio source

    // Start is called before the first frame update
    void Start()
    {
        // Find the player controller - speeds up adding future audio spots
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Script which gets run when the object is selected
    public void ObjectSelected(){
        // Runs the ObjectSelected script in the player, with itself and its audio source as the parameters
        playerController.ObjectSelected(this.gameObject, audioSource);
    }

    // Script which gets run when the object is delected
    public void ObjectDeselected(){
        // Runs the ObjectDelected in the player, in order to remove the reference to his object
        playerController.ObjectDeselected();
    }
}
