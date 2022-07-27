// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 17/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles Stopping of the Audio currently playing
ChangeLog:
17/11/2021: Script created
08/12/2021: Added comments, null checks
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGAudioStopTrigger : MonoBehaviour
{
    [SerializeField]
    private SceneSoundManager m_sceneSoundManager;     // Reference to the Scene Sound Manager
    
    [SerializeField]
    private bool m_canBeReTriggered = false;           // Bool can it retriggered
    
    [SerializeField]
    private float m_fadeOutTime = 0.0f;                // Time for audio to fade out
    
    // Called when something interacts with the TriggerBox
    private void OnTriggerEnter(Collider other)
    {
        // If the GameObject is the Player
        if (other.gameObject.tag == "Player")
        {
            m_sceneSoundManager.StopAudio(m_fadeOutTime);

            if (m_canBeReTriggered == false)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
