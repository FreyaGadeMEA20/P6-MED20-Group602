using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedAudioClip
{
    public ExtendedAudioClip(string _name, AudioClip _audioClip){
        soundName = _name;
        audioClip = _audioClip;
    }

    public string soundName {get; set;}

    AudioClip audioClip;
    public AudioClip p_audioClip {
        get{
            Debug.Log("Audio clip: <color=blue>" + soundName + "</color> has been retrived");
            return audioClip;
        }
        set{
            Debug.LogWarning("Audio clip: <color=blue>" + soundName + "</color> has been altered");
            audioClip = value;
        }
    }
}
