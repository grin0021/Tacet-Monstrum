// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 19/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles stopping the music playing currently in the scene
ChangeLog:
19/11/2021: Script created
08/12/2021: Added comments, null checks
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicTrigger : MonoBehaviour
{
    [SerializeField] private SceneSoundManager m_sceneSoundManager;     // Reference for the Scene Sound Manager
    [SerializeField] private AudioClip m_musicToPlay;                   // Audio to play
    [SerializeField] private float m_volume = 1.0f;                     // Volume for the audio
    [SerializeField] private float m_pitch = 1.0f;                      // Pitch for the audio
    [SerializeField] private float m_stereoPan = 0.0f;                  //
    [SerializeField] private bool m_canLoop = false;                    // Bool can the audio loop
    [SerializeField] private bool m_canBeReTriggered = false;           // Bool can it be retriggered
    [SerializeField] private float m_fadeInTime = 0.0f;                 // Fade out time for the audio

    private void Start()
    {
        m_sceneSoundManager = GameObject.Find("PR_SceneSoundManager").GetComponent<SceneSoundManager>();
    }

    // Called when something interacts with TriggerBox
    private void OnTriggerEnter(Collider other)
    {
        // If the GameObject is the Player
        if (other.gameObject.tag == "Player")
        {
            if (m_volume != 1.0f && m_pitch != 1.0f && m_stereoPan != 0.0f)
            {
                m_sceneSoundManager.SetMusicAudio(m_musicToPlay, m_volume, m_pitch, m_stereoPan, m_canLoop, m_fadeInTime);
            }
            else if (m_volume != 1.0f)
            {
                m_sceneSoundManager.SetMusicAudio(m_musicToPlay, m_volume, m_canLoop, m_fadeInTime);
            }
            else
            {
                m_sceneSoundManager.SetMusicAudio(m_musicToPlay, m_canLoop, m_fadeInTime);
            }

            if (m_canBeReTriggered == false)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
