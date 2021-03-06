﻿/** 
* Author: Matthew Douglas, Hisham Ata
* Purpose: To handle all the audio logic within the game.
*          Sound object are made to be reused when a sfx is requested
**/

using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, ISwapper
{
#pragma warning disable 0649
    [Header("Music")]
    [SerializeField]
    private AudioSource music;
    [SerializeField]
    private AudioClip[] musicClips;

    [Header("Sound")]
    public GameObject soundPefab;

    [Header("Button Sounds")]
    public AudioClip[] buttonSounds;

    // All the sounds objects in game we can use
    private List<AudioSource> soundObjects;

    private void Start()
    {
        // Initialize sound object lists
        soundObjects = new List<AudioSource>();
        InitSounds();
    }


    public void PlayMusic(int idx)
    {
        if(music.clip != musicClips[idx])
        {
            music.Stop();
            music.clip = musicClips[idx];
            music.Play();
        }      
    }

    // previews music, but does not make current select
    public void PreviewMusic(int idx)
    {
        if (music.isPlaying)
            music.Stop();
        music.clip = musicClips[idx];
        music.Play();
    }

    public void PlaySound(AudioClip clip, float vol = 1.0f)
    {
        // Check for available sound, create one if one is not available to play
        if (!FindSound(clip, vol))
        {
            CreateSound(clip, vol);
        }
    }

    public void PressButton(int buttonSound = 0)
    {
        // Play a specific sound of button depending on the state of button press
        PlaySound(buttonSounds[buttonSound]);
    }

    private bool FindSound(AudioClip clip, float vol)
    {
        foreach (AudioSource aud in soundObjects)
        {
            if (!aud.isPlaying)
            {
                aud.volume = vol;
                aud.clip = clip;
                aud.Play();
                return true;
            }
        }

        return false;
    }

    private void InitSounds()
    {
        // Begin with 5 sound objects to work with
        for(int i = 0; i < 10; ++i)
        {
            GameObject soundObj = Instantiate(soundPefab, this.transform) as GameObject;

            AudioSource aud = soundObj.GetComponent<AudioSource>();
            aud.volume = 0.5f;
            soundObjects.Add(aud);
        }
    }

    private void CreateSound(AudioClip clip, float vol)
    {
        // Created object
        GameObject soundObj = Instantiate(soundPefab, this.transform) as GameObject;

        // Use to play and add to list
        AudioSource aud = soundObj.GetComponent<AudioSource>();
        aud.volume = vol;
        aud.clip = clip;
        aud.Play();
        soundObjects.Add(aud);
    }

    public void SwapIt()
    {
      
         int idx = SwapManager.MusicIdx;
        
        //double check to make sure player has this unlock
        if(SwapManager.MusicUnlocks.Contains(idx))
            PlayMusic(idx);
        else
        {
            SwapManager.MusicIdx = 0;
            PlayMusic(0);
        }
    }
}
