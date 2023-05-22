using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DictionarySound : MonoBehaviour
{
    // Dictionary for sorting through each sound with a name
    // This is used to find all the sounds of a specific type, ie. all cow sounds
    Dictionary<string, ExtendedAudioClip> soundDict = new Dictionary<string, ExtendedAudioClip>();

    // Item to be changed into a dictionary item
    [Serializable] public struct SoundItem{
        public string soundType;

        public string soundName;
        public AudioClip audioClip;
    }

    // Array of the above item, which gets turned into the dictionary soundDict
    [SerializeField] SoundItem[] soundLibrary; 

    // In order to make sure there is no errors, a list is made to sort out sounds that are not needed
    // Possible an additional unneeded check and can be deleted in the future when better code is added.
    [SerializeField] List<string> correctTypes;

    // Dictionary for sorting through the audioSources
    Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

    // Item to be changed into a dictionary item
    [Serializable] public struct AudioSourceItem {
        public string asName;               // Name of the audio source - used for being more in control of the direction
        public AudioSource audioSource;     // The audio source
    }
    
    // Array of the above item, which gets turned into the dictionary audioSources
    [SerializeField] AudioSourceItem[] audioSourcesArray;

    // -- Variables for controlling the audio source -- //
    [SerializeField] AudioSource selectedAudioSource;

    // Variables for keeping the sound going
    private bool continuousPlaying;
    public bool ContinuousPlaying {get{return continuousPlaying;}    set{continuousPlaying = value;}}

    [SerializeField] float timeSince;

    // Start is called before the first frame update
    void Start()
    {
        foreach(SoundItem i in soundLibrary){
            if(correctTypes.Contains(i.soundType)){
                soundDict.Add(i.soundType, new ExtendedAudioClip(i.soundName, i.audioClip));
            } else{
                Debug.LogWarning(i.soundType + " is not a valid type for a sound.");
            }
        }

        Array.Clear(soundLibrary, 0, soundLibrary.Length);

        foreach(AudioSourceItem i in audioSourcesArray){
            audioSources.Add(i.asName, i.audioSource);
        }

        Array.Clear(audioSourcesArray,0, audioSourcesArray.Length);

        #if UNITY_EDITOR
        foreach(KeyValuePair<string, ExtendedAudioClip> entry in soundDict){
            Debug.Log(entry.Key + " has value " + entry.Value);
        }
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(string sound){
        //audioSource.Play(soundDict.ContainsValue(sound));
    }

    void SelectAudioSource(){
        
    }
}
