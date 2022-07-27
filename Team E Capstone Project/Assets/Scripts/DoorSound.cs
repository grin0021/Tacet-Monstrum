// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 04/10/2021
Comment/Description: Script handling various Door Sounds
ChangeLog:
14/11/2021: Added null checking for components
09/12/2021: Fixed Door making sounds on start
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Script handling various Door Sounds
ChangeLog:
12/11/2021: Added Comments and updated ChangeLog 
*/

/*
Author: Jacob Kaplan-Hall
ChangeLog
14/11/21: changed script to work with new audio system
 */

/*
Author: Seth Grinstead
ChangeLog:
07/04/2022: Updated sound emission
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script storing and handling various sounds of the doors
public class DoorSound : MonoBehaviour
{
    public AudioClip[] m_AudioClip;         // Array storing various audio clips
    public AudioSource m_AudioSource;       // Reference to the AudioSource

    [Header("Physics")]
    public Rigidbody m_Body;                // Reference to the Rigidbody component

    [Header("Creak Control")]
    public float CreakAngle = 15.0f;        // Creak at set angle
    public float PitchControl = 0.1f;       // Control pitch

    private float m_angle = 0.0f;           // Float storing angle of roattion for the door
    private float m_lastAngle = 0.0f;       // Float storing last angle for creaking
    private float m_lastCreak = 0.0f;       // Float storing last creak

    [Header("Misc")]
    public float MaxForce = 5.0f;           // Storing max force to open the door
    public float MaxAudioRange = 10.0f;     // Storing max Audio range til the sound can be heard

    public bool IsEnabled = false;          // Is Door enabled to make sounds?

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (m_AudioClip.Length == 0)
            {
                Debug.LogError("Missing Audio Clips", this);
            }

            if (!(m_AudioSource = GetComponent<AudioSource>()))
            {
                Debug.LogError("Missing Audio Source Component", this);
            }

            if (!(m_Body = GetComponent<Rigidbody>()))
            {
                Debug.LogError("Missing Rigidbody Component", this);
            }
        }
        m_AudioSource = GetComponent<AudioSource>();
        m_Body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsEnabled == true)
        {
            // Call method creak sounds every frame
            CreakSounds();
        }
    }

    // Method handling the functioning of Sound depending on the force used to open the door
    public void CreakSounds()
    {
        // Set Angle to y rotation of the Door Rigidbody
        m_angle = m_Body.rotation.eulerAngles.y;

        // If Door moved more than creakAngle...
        if (Mathf.Abs(m_angle - m_lastAngle) > CreakAngle)
        {
            // Update lastAngle
            m_lastAngle = m_angle;

            // Calculate time from last creak
            float delta = Time.time - m_lastCreak;

            // Set lastCreak to amount of time in seconds that the game has been running for
            m_lastCreak = Time.time;

            // Increase pitch based on speed of door
            m_AudioSource.pitch = Mathf.Clamp((0.5f + PitchControl) / (0.5f + delta), 0.9f, 1.5f);
            m_AudioSource.volume = CalculateVolume();

            // Play sound through audio manager
            Sound sound = AudioManager.PlaySound(transform, m_AudioClip[0], null, m_AudioSource.pitch, m_AudioSource.volume);
            sound.SetEffectsInput(AudioManager.GetMasterMixerGroup("SFX"));
        }
    }

    // Method called for calculating the volume every time the door is opened
    public float CalculateVolume()
    {
        float force = m_Body.angularVelocity.magnitude;

        float volume = 1.0f;

        if (force <= MaxForce)
        {
            volume = force / MaxForce;

            if (volume < 0.5f)
            {
                volume = 0.5f;
            }
        }

        return volume;
    }
}