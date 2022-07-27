// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 17/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles the sound effect to be played
ChangeLog:
17/11/2021: Script created
01/12/2021: Added comments and changelog
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectTrigger : MonoBehaviour
{
    [SerializeField] private SceneSoundManager m_sceneSoundManager;     // Reference to the Scene Sound Mnager
    [SerializeField] private AudioClip m_clipToPlay;                    // Audio Clip to Play
    [SerializeField] private float m_volume = 1.0f;                     // Volume for the audio
    [SerializeField] private float m_pitch = 1.0f;                      // Pitch for the audio
    [SerializeField] private float m_stereoPan = 0.0f;                  // 
    [SerializeField] private bool m_canRepeat = false;                  // Bool for can the audio repeat

    // Called when something enters the TriggerBox
    private void OnTriggerEnter(Collider other)
    {
        // if the GameObject is the Player
        if (other.gameObject.tag == "Player")
        {
            if (m_volume != 1.0f && m_pitch != 1.0f && m_stereoPan != 0.0f)
            {
                m_sceneSoundManager.SetSoundEffectAudio(m_clipToPlay, m_volume, m_pitch, m_stereoPan);
            }
            else if (m_volume != 1.0f)
            {
                m_sceneSoundManager.SetSoundEffectAudio(m_clipToPlay, m_volume);
            }
            else
            {
                m_sceneSoundManager.SetSoundEffectAudio(m_clipToPlay);
            }

            if (m_canRepeat == false)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
