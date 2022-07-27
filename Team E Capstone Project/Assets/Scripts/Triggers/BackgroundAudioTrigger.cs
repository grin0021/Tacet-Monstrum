// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 17/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles playing of Background Audio
ChangeLog:
17/11/2021: Script created
02/12/2021: Add comments and changelog
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Class that handles playing of Background Audio
ChangeLog:
24/03/2022: Rewrote BackgroundAudioTrigger to use SFX, Music, and Environment Audio Triggers
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudioTrigger : MonoBehaviour
{
    // Enum with different audio triggers
    private enum EAudioTriggers
    {
        SFX,
        Music,
        Environment
    }

    private SceneSoundManager m_sceneSoundManager;                      // Reference to the Scene Sound Mnager

    [Header("Audio Setup")]
    [SerializeField] private AudioClip m_clipToPlay;                    // Audio Clip to Play
    [SerializeField] private EAudioTriggers m_currentAudioTrigger;      // Stores the current audio trigger type

    [Header("Audio Settings")]
    [SerializeField] private bool m_canLoop = false;                    // Bool for can the audio repeat
    [SerializeField] private bool m_canBeReTriggered = false;           // Bool for can it be retriggered

    [Space(10)]

    [Range(0.0F, 1.0F)]
    [SerializeField] private float m_volume = 1.0f;                     // Volume for the audio

    [Space(5)]
    [Range(-3.0F, 3.0F)]
    [SerializeField] private float m_pitch = 1.0f;                      // Pitch for the audio

    [Space(5)]
    [Range(-1.0F, 1.0F)]
    [SerializeField] private float m_stereoPan = 0.0f;                  // Panning for the audio

    [Space(5)]
    [Range(0.0F, 5.0F)]
    [SerializeField] private float m_fadeInTime = 0.0f;                 // Time to fade the audio out

    private void Start()
    {
        // Fetch component in the game
        m_sceneSoundManager = GameObject.Find("PR_SceneSoundManager").GetComponent<SceneSoundManager>();
    }

    // Called when something interacts with the TriggerBox
    private void OnTriggerEnter(Collider other)
    {
        // If other gameobject tag equals "Player"...
        if (other.gameObject.tag == "Player")
        {
            switch (m_currentAudioTrigger)
            {
                // If Audio Trigger is set to Environment...
                case EAudioTriggers.Environment:
                    // If volume, pitch, and stereo pan are set...
                    if (m_volume != 1.0f && m_pitch != 1.0f && m_stereoPan != 0.0f)
                    {
                        // Set BackgroundAudio with audio clip, volume, pitch, stereoPan, canLoop, and fadeInTime
                        m_sceneSoundManager.SetBackgroundAudio(m_clipToPlay, m_volume, m_pitch, m_stereoPan, m_canLoop, m_fadeInTime);
                    }
                    // If volume is set...
                    else if (m_volume != 1.0f)
                    {
                        // Set BackgroundAudio with audio clip, volume,  canLoop, and fadeInTime
                        m_sceneSoundManager.SetBackgroundAudio(m_clipToPlay, m_volume, m_canLoop, m_fadeInTime);
                    }
                    // If volume, pitch, and stereo pan are not set...  
                    else
                    {
                        // Set BackgroundAudio with audio clip, canLoop, and fadeInTime
                        m_sceneSoundManager.SetBackgroundAudio(m_clipToPlay, m_canLoop, m_fadeInTime);
                    }
                    break;

                // If Audio Trigger is set to Music...
                case EAudioTriggers.Music:
                    // If volume, pitch, and stereo pan are set...
                    if (m_volume != 1.0f && m_pitch != 1.0f && m_stereoPan != 0.0f)
                    {
                        // Set MusicAudio with audio clip, volume, pitch, stereoPan, canLoop, and fadeInTime
                        m_sceneSoundManager.SetMusicAudio(m_clipToPlay, m_volume, m_pitch, m_stereoPan, m_canLoop, m_fadeInTime);
                    }
                    // If volume is set...
                    else if (m_volume != 1.0f)
                    {
                        // Set MusicAudio with audio clip, volume,  canLoop, and fadeInTime
                        m_sceneSoundManager.SetMusicAudio(m_clipToPlay, m_volume, m_canLoop, m_fadeInTime);
                    }
                    // If volume, pitch, and stereo pan are not set...  
                    else
                    {
                        // Set MusicAudio with audio clip, canLoop, and fadeInTime
                        m_sceneSoundManager.SetMusicAudio(m_clipToPlay, m_canLoop, m_fadeInTime);
                    }
                    break;

                // If Audio Trigger is set to SFX...
                case EAudioTriggers.SFX:
                    // If volume, pitch, and stereo pan are set...  
                    if (m_volume != 1.0f && m_pitch != 1.0f && m_stereoPan != 0.0f)
                    {
                        // Set SoundEffectAudio with audio clip, volume, pitch, and stereoPan
                        m_sceneSoundManager.SetSoundEffectAudio(m_clipToPlay, m_volume, m_pitch, m_stereoPan);
                    }
                    // If volume is set...
                    else if (m_volume != 1.0f)
                    {
                        // Set SoundEffectAudio with audio clip and volume
                        m_sceneSoundManager.SetSoundEffectAudio(m_clipToPlay, m_volume);
                    }
                    // If volume, pitch, and stereo pan are not set...  
                    else
                    {
                        // Set SoundEffectAudio with only audio clip
                        m_sceneSoundManager.SetSoundEffectAudio(m_clipToPlay);
                    }
                    break;
            }

            // If canBeReTriggered is false...
            if (m_canBeReTriggered == false)
            {
                // Disable gameobject
                this.gameObject.SetActive(false);
            }
        }
    }
}