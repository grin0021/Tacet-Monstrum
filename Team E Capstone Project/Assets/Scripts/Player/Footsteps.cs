// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 08/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Script Controlling AI Detection through Heard Sounds
ChangeLog:
05/11/2021: Added Example ChangeLog
13/11/2021: Added null checks and refined variables names
*/

/*
Author: Seth Grinstead
ChangeLog:
08/11/2021: Footsteps now uses emitter to allow for monster to detect
24/11/24: Added frequencies and volumes based on player movement state
07/04/2022: Updated audio emission
*/

/*
Author: Bryson Bertuzzi
ChangeLog:
13/11/2021: Added null checking for components
30/03/2022: Added impact sound for landing after jump
11/04/2022: Fixed missing lines of code for landing
*/

/*
Author: Jacob Kaplan-Hall
ChangeLog
14/11/21: changed script to work with new audio system
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script which holds the properties of the sound of footsteps for the Player
public class Footsteps : MonoBehaviour
{
    [SerializeField]
    private PlayerMovementScript m_moveScript;                    // Reference to the Player's Movement Script

    private MouseKeyPlayerController Controller { get; set; }     // Getter and Setter for the Controller

    float m_CurrentPitch = 1f;                                    // References to the values used to modulate the footsteps
    float m_CurrentVolume = 1f;

    [SerializeField]
    private AudioClip m_defaultClip;                              // Audio clip to be played by default

    [SerializeField]
    private AudioClip m_landingClip;                              // Audio clip to be played on jump landing

    [SerializeField]
    private PlayerController m_playerController;                  // Reference to the PlayerController

    private float m_stepTime = 0.15f;
    float m_maxStepTime;

    private bool m_bisLanding = false;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (!(m_playerController = GetComponent<PlayerController>()))
            {
                Debug.LogError("Missing Player Controller Component", this);
            }

            if (!(m_moveScript = GetComponent<PlayerMovementScript>()))
            {
                Debug.LogError("Missing Player Controller Component", this);
            }

            if (Controller == null)
            {
                Controller = m_moveScript.Controller;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (m_moveScript.GetState() == PlayerMovementScript.EPlayerState.Walking)
        {
            m_maxStepTime = 0.7f;
            m_CurrentVolume = 0.5f;
        }
        else if (m_moveScript.GetState() == PlayerMovementScript.EPlayerState.Sprinting)
        {
            m_maxStepTime = 0.35f;
            m_CurrentVolume = 0.75f;
        }
        else if (m_moveScript.GetState() == PlayerMovementScript.EPlayerState.Crouching)
        {
            m_maxStepTime = 1.0f;
            m_CurrentVolume = 0.25f;
        }
        else if (m_moveScript.GetState() == PlayerMovementScript.EPlayerState.Crawling)
        {
            m_maxStepTime = 1.5f;
            m_CurrentVolume = 0.1f;
        }
        else if (m_moveScript.GetState() == PlayerMovementScript.EPlayerState.Jumping && m_moveScript.bIsGrounded == true)
        {
            m_moveScript.EnterState(PlayerMovementScript.EPlayerState.Walking);
            m_bisLanding = true;
        }

        // If the player is on ground, is moving, and the audio clip is not playing
        if (m_moveScript.bIsGrounded == true && Controller.GetMoveInput() != Vector3.zero)
        {
            m_stepTime -= Time.deltaTime;

            //m_playerController.GetComponent<AudioSource>().volume = Random.Range(0.8f, 1);
            m_CurrentPitch = Random.Range(0.8f, 1.1f);

            if (m_stepTime <= 0.0f)
            {
                Sound sound = AudioManager.PlaySound(transform, m_defaultClip, null, Mathf.Clamp(m_CurrentPitch, .75f, 1.5f), m_CurrentVolume);
                sound.SetEffectsInput(AudioManager.GetMasterMixerGroup("SFX"));

                m_stepTime = m_maxStepTime;
            }
        }
        else
        {
            m_stepTime = m_maxStepTime;
        }

        // If the player is landing from a jump
        if (m_bisLanding == true)
        {
            Sound sound = AudioManager.PlaySound(transform, m_landingClip, null, 1.0f, 0.15f);
            sound.SetEffectsInput(AudioManager.GetMasterMixerGroup("SFX"));

            m_bisLanding = false;
        }
    }
}
