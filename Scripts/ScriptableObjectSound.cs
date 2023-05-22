using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScriptableObjectSound : MonoBehaviour
{
    [SerializeField] AudioClip congratulatoryAudioClip;     // Audio clip for the congratulatory audio clip

    // the list of the scriptable objects. List > array due to useability
    [SerializeField] List<AudioClipType> audioClips = new List<AudioClipType>(){
        new AudioClipType("Cow", new List<AudioClipScriptableObject>()),
        new AudioClipType("Sheep", new List<AudioClipScriptableObject>()),
        new AudioClipType("Pig", new List<AudioClipScriptableObject>()),
        new AudioClipType("Horse", new List<AudioClipScriptableObject>()),
        new AudioClipType("Chicken", new List<AudioClipScriptableObject>()),
    };
    // the list of the audio sources.
    [SerializeField] List<AudioSource> AudioSources;
    List<AudioSource> possibleAudioSources;

    // special constructor for the audio clips
    [Serializable] public class AudioClipType{
        public AudioClipType(string _t, List<AudioClipScriptableObject> _ac){
            type = _t;
            audioClip = _ac;
        }
        public string type; // the type of the animal
        public List<AudioClipScriptableObject> audioClip;   // list of audio clips
    }    

    // -- Variables for controlling the audio source -- //
    [SerializeField] AudioSource selectedAudioSource; // Currently affected audio source - for when needing to replay the sound etc.
    public AudioSource SelectedAudioSource{
        get{
            return selectedAudioSource;
        }
        set{
            selectedAudioSource = value;
        }
    }

    public AudioClip selectedAudioClip; // the audio clip that is playing 

    // Less important for non-vr - mostly for when giving visual feedback of which animal it is
    public int currentType;
    [SerializeField] List<int> possibleTypes;

    // Start is called before the first frame update
    void Start()
    {
        // Runs the process new sound function
        ProcessNewSound();
    }

    // Selects and plays a sound
    public void ProcessNewSound(){
        SelectRandomClip(SelectRandomType()); // selects a random clip and gets the type of animal
        // the type does not affect anything about the sound, and all of it is seperate.

        // Selects the audio source and plays the sound.
        SelectAudioSource();
    }

    public void ProcessOnlySound(){
        SelectRandomClip(currentType);
    }

    // Choose a random type
    int SelectRandomType(){
        // Creates a list of the types it can choose between.
        //  - Weird hard-coded => More based on the list of types instead for future dev.
        possibleTypes = new List<int>(){0,1,2,3,4};
        
        // Removes the current type from the list, to not repeat.
        possibleTypes.RemoveAt(currentType);

        // Chooses a random value from the list
        int name = UnityEngine.Random.Range(0,possibleTypes.Count);

        // Saves it and returns it. 
        currentType = possibleTypes[name];        
        return currentType;
    }

    // Function for selecting the audio clip based on the type of animal
    void SelectRandomClip(int _type){
        // Uses the unity random to select a random number based on the size of the list
        int i = UnityEngine.Random.Range(0, audioClips[_type].audioClip.Count); 
        
        // Selects the random audio clip based on the number.   
        selectedAudioClip = audioClips[_type].audioClip[i].AudioClip; 
    }

    // Function for selecting the audio source to play out of.
    void SelectAudioSource(){
        // Creates a list of the possible audio source to choose from
        possibleAudioSources = new List<AudioSource>(AudioSources);

        // Removes the current audio source from the list, to not repeat.
        possibleAudioSources.Remove(selectedAudioSource);

        // Determines the random number based on the size of the list
        int i = UnityEngine.Random.Range(0, possibleAudioSources.Count);
        
        // Selects an audio source
        selectedAudioSource = possibleAudioSources[i];

        // Set the audio clip and applies it to the audio source
        selectedAudioSource.clip = selectedAudioClip;
    }

    // Plays the sound
    public void PlaySound(){
        selectedAudioSource.Play();
    }

    // When the correct sound is found
    public void CorrectSound(){
        selectedAudioSource.clip = congratulatoryAudioClip; // Applies the audio clip to the audio source
        
        PlaySound();    // plays the audio

        StartCoroutine(DelayedProcessNewSound()); // starts a delay to choose a new sound
    }
    // IEnumerator for creating a delayed sound that will run side by side with the program
    IEnumerator DelayedProcessNewSound(){
        yield return new WaitForSeconds(1.0f);  // waits for a second
        ProcessNewSound();                      // processes the new sound
    }
}
