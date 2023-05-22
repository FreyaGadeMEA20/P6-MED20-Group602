using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : FSM
{
    // The different states of the gameplay
    public enum PlayerState{
        WAIT,
        PRECUTSCENE,
        ONGOING,
        EVALUATE_STATE,
        FINISHED,
    }
    public PlayerState playerState; // current state of the game
    [SerializeField] ScriptableObjectSound audioManager; // reference to the audio manager

    // Reference to the different parabol guns in the scene - needs to have a reference to the right and left gun
    [SerializeField] GameObject rightGun;
    [SerializeField] GameObject leftGun;
    GameObject activeGun;   // the one the player sees

    // Reference to the different ray cast interactors in the scene - needs to have a reference to the right and left hand
    [SerializeField] GameObject rightInteractor;
    [SerializeField] GameObject leftInteractor;
    GameObject activeInteractor; // the one that the player will interact with
    
    // InputDevices for keeping track of whether the player is right or left handed. Active is the one the programs reads from during the gameplay
    InputDevice activeDevice;
    InputDevice rightHandDevice;
    InputDevice leftHandDevice;

    // booleans for controller the states of the game
    bool preCutsceneOver;
    bool finished;

    // checks if the player presses the trigger, to prevent it from running each frame while the player is pressing it.
    bool triggerPressed;

    /// different variables for the info of what the player is hovering over
    bool hovering = false;
    GameObject objectHovering;
    AudioSource selectedAudioSource;

    // delay between each sound - starts at 4f and changes depending on the sound that is getting played.
    public float timeBetweenPlayingSound = 4f;

    // Point for the gameplay loop to end
    [SerializeField] int maxPoints;
    int currentPoint;

    // Initialize is called before the first frame update
    protected override void Initialize()
    {
        // Set the current state of the program to be the wait state.
        playerState = PlayerState.WAIT;

        // Get the audio manager in the scene
        audioManager = GameObject.FindGameObjectWithTag("audioManager").GetComponent<ScriptableObjectSound>();

        // Characteristics for the right and left controller. Both share the controller characteristics.
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;

        // Create a temporary list to hold the devices it gets from looking for characteristics.
        List<InputDevice> tempRDevices = new List<InputDevice>();
        List<InputDevice> tempLDevices = new List<InputDevice>();

        // Gets the devices with the given characteristics
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, tempRDevices);
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, tempLDevices); 

        // Applies the devices to the variables that are being saved.
        rightHandDevice = tempRDevices[0];
        leftHandDevice = tempLDevices[0];
    }

    // Update is called once per frame
    protected override void FSMUpdate()
    {

        // Runs the function depending on which state the player is in
        switch (playerState){
            case PlayerState.WAIT:
                UpdateWaitState();
                break;
            case PlayerState.PRECUTSCENE:
                UpdatePreCutsceneState();
                break;
            case PlayerState.ONGOING:
                UpdateGameplayLoopState();
                break;
            case PlayerState.EVALUATE_STATE:
                EvaluateStateState();
                break;
            case PlayerState.FINISHED:
                UpdateFinishedState();
                break;
        }
    }

    protected override void FSMFixedUpdate(){}

    /// <summary>
    /// Wait state for when the program is waiting for the user to start.
    /// </summary>
    protected void UpdateWaitState(){
        // Switch to the next state
        // Change to get the touch of the user, instead of checking which controller they are using
        rightHandDevice.TryGetFeatureValue(CommonUsages.grip, out float primaryRButtonValue);
        if(primaryRButtonValue > 0.9f){
            SelectController(rightHandDevice, rightGun, rightInteractor);
        }
        
        leftHandDevice.TryGetFeatureValue(CommonUsages.grip, out float primaryLButtonValue);
        if(primaryLButtonValue > 0.9f){
            SelectController(leftHandDevice, leftGun, leftInteractor);
        }

    }

    /// <summary>
    /// Setting the controller for the program
    /// </summary>
    /// <param name="_controller">The controller that should be set</param>
    private void SelectController(InputDevice _controller, GameObject _gun, GameObject _interactor){
        activeDevice = _controller;

        activeGun = _gun;
        activeGun.SetActive(true);

        activeInteractor = _interactor;
        activeInteractor.SetActive(true);


        // Replace this with another way to determine when to change from PC to GL
        IEnumerator cor = StartPrecutscene(1f);
        StartCoroutine(cor);

        playerState = PlayerState.PRECUTSCENE;
    }

    /// <summary>
    /// A cutscene state to play when the game has been started.
    /// </summary>
    protected void UpdatePreCutsceneState(){
        // Play a cutscene to showcase the animals escaping from the farm
        // Give instructions to the player here as well
        // - most likely same element (Unity Timeline package), but we'll see

        // Switch to the next state (if(false) -> when cutscene is finished. Timeline marker?)
        if(preCutsceneOver){
            SetupNewSound();
            timeBetweenPlayingSound = audioManager.selectedAudioClip.length + Random.Range(1.5f, 2.5f);
            PlaySound();
            playerState = PlayerState.ONGOING;
        }
    }

    // IEnumerator for the pre cutscene state for it to go from idle to gameplay loop state.
    // Waits a specified time an then says it is over
    private IEnumerator StartPrecutscene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        preCutsceneOver = true;
    }
    
    /// <summary>
    /// Game state which handles the gameplay loop.
    /// </summary>
    protected void UpdateGameplayLoopState(){
        activeDevice.TryGetFeatureValue(CommonUsages.grip, out float primaryTrigger);
        if(primaryTrigger > 0.9f && hovering && DetermineCorrectSource() && !triggerPressed){
            // Clause to not trigger each frame the trigger is pressed. Could be removed.
            triggerPressed = true;

            // Starts a coroutine for the correct IEnumerator as the player is correct
            StartCoroutine(Correct());
            // Goes into the evaluate_state state, as it is evaluating the players score
            playerState = PlayerState.EVALUATE_STATE;
        } else if (primaryTrigger < 0.9f){
            triggerPressed = false; // once the player releases the trigger, it allows them to press again.
        }

        // Gameplay loop here
        if(timeBetweenPlayingSound > 0){
            timeBetweenPlayingSound -= Time.deltaTime; // Timer to next audio player
        } else {
            // If the time between sound is less to or equal 0, it will process a new sound, set the new timer and then play the sound.
            audioManager.ProcessOnlySound();
            timeBetweenPlayingSound = audioManager.selectedAudioClip.length + Random.Range(1.5f, 2.5f);
            PlaySound();
        }
        
        // -- Non VR Debugging -- //
        if(Input.GetKeyDown(KeyCode.Space)){
            StartCoroutine(Correct());
            playerState = PlayerState.EVALUATE_STATE;
        }
    }

    // Checks whether the two gameobjects are the same
    //  - should be changed so the gameobjects are included in the function and not hardcoded.
    private bool DetermineCorrectSource(){
        return GameObject.ReferenceEquals(selectedAudioSource.gameObject, audioManager.SelectedAudioSource.gameObject);
    }

    // Evaluate state state, for when it needs to evaluate the players score.
    protected void EvaluateStateState(){
        triggerPressed = false; // resets the trigger
        ObjectDeselected();     // deselects the object

        // checks the players points to determine if they are finished or not.
        //  - prob should be changed so it does currentPoint++ before the check, but does not really matter for this prototype.
        if(currentPoint >= maxPoints){
            finished = true;
        } else {
            currentPoint++;
        }

        // Switch to the next state (if(false) -> )
        if(finished){
            playerState = PlayerState.FINISHED;
        }
    }
    
    // IEnumerator for when the player has found the correct sound
    //  - plays the sound for the player and a delay before it goes back to the ongoing state.
    IEnumerator Correct(){
        audioManager.CorrectSound();
        
        yield return new WaitForSeconds(1.5f);
        
        playerState = PlayerState.ONGOING;
        timeBetweenPlayingSound = 1f;
    }

    /// <summary>
    /// Finished state which happens when the game is finished.
    /// </summary>
    protected void UpdateFinishedState(){
        Debug.Log("Game is over");
        // Whatever the game is supposed to do when it is finished.
        // - Play a cutscene
    }

    // Setup new sound. Easier to call.
    protected void SetupNewSound(){
        audioManager.ProcessNewSound();
    }

    // Play the sound. Easier to call.
    protected void PlaySound(){
        audioManager.PlaySound();
    }

    /// <summary>
    /// Telling the player which object is getting selected.
    /// </summary>
    public void ObjectSelected(GameObject _go, AudioSource _as){
        hovering = true;            // Tells the program it is hovering an object
        objectHovering = _go;       // Tells the program which object is getting hovered
        selectedAudioSource = _as;  // Tells the program which audio source is hovered

        // Visual feedback to the user by giving the object an outline
        objectHovering.GetComponent<Outline>().OutlineWidth = 10;
    }

    /// <summary>
    /// Telling the player which object is getting deselected.
    /// </summary>
    public void ObjectDeselected(){
        // Removes the outline before removing the object from the memory
        objectHovering.GetComponent<Outline>().OutlineWidth = 0;

        hovering = false;           // Tells the program it is no longer hovering an object
        objectHovering = null;      // Removes the object stored
        selectedAudioSource = null; // Removes the audio source stored
    }
}
