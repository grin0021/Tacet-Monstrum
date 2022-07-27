// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Audio Detecting System
ChangeLog:
15/11/2021: 15/11/2021: Added ChangeLog and function definition comments and null checks
*/

/*
Author: Seth Grinstead
Comment/Description: Class that control box sound behaviour with physics
ChangeLog:
12/11/2021: Added comments
18/11/2021: Updated collision to check if the player was involved in the collision (For monster detection)
18/11/2021: Updated collision so that monster does not trigger itself by walking into a box
07/04/2022: Updated sound emission
*/

/*
Author: Jacob Kaplan-Hall
ChangeLog
14/11/21: changed script to work with new audio system
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoxSound : MonoBehaviour
{
    public AudioClip FirstAudioClip;           // Reference to an AudioClip in scene
    public AudioClip SecondAudioClip;          // Reference to a second AudioClip in scene

    public float MaxAudioRange = 40f;          // Max range audio will reach
    public float MaxForce = 10f;               // Maximum force that will affect sound output
    public float MinForce = .5f;               // Minumum force that will affect sound output
    public float VolumeScale = 1.6f;           // Multiplication factor for volume

    public bool bWasHitByMonster = false;      // Was the box hit by the monster?

    bool m_audioToggle = true;                 // Toggle between first and second AudioClip

    AudioSource m_Source;                      // Reference to object's audio source component
        
    [Tooltip("Time on start where collisions are ignored")]
    public float AStartTimer = 1f;
    bool m_start;

    // Start is called before the first frame update
    void Start()
    {
        //VelocityBuffer = new List<float>();
        //VelocityBuffer.Capacity = VelocityBufferSize;

        m_start = false;

        if (GetComponent<AudioSource>())
        {
            m_Source = GetComponent<AudioSource>();
        }

        StartCoroutine(StartTimer());
    }

    // Called wheenever a collision happens with the GameObject
    void OnCollisionEnter(Collision collision)
    {
        // If applied force to object is greater than MinFOrce and m_start is true
        if (collision.relativeVelocity.magnitude >= MinForce && m_start)
        {
            float force = collision.relativeVelocity.magnitude;

            float volume = force / MaxForce;
            
            // If m_AudioToggle is true
            if (m_audioToggle) 
            {
                m_audioToggle = false;

                if (!bWasHitByMonster)
                {
                    Sound sound = AudioManager.PlaySound(transform, FirstAudioClip, null, 1.0f, volume);
                    sound.SetEffectsInput(AudioManager.GetMasterMixerGroup("SFX"));
                }
                else
                {
                    m_Source.PlayOneShot(FirstAudioClip, volume);
                    bWasHitByMonster = false;
                }
            }
            else
            {
                m_audioToggle = true;

                if (!bWasHitByMonster)
                {
                    Sound sound = AudioManager.PlaySound(transform, SecondAudioClip, null, 1.0f, volume);
                    sound.SetEffectsInput(AudioManager.GetMasterMixerGroup("SFX"));
                }
                else
                {
                    m_Source.PlayOneShot(SecondAudioClip, volume);
                    bWasHitByMonster = false;
                }
            }
        }

    }

    // Enumerator function for a timer
    IEnumerator StartTimer()
    {
        // While AStartTime is greater than zero
        while (AStartTimer > 0f)
        {
            AStartTimer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        m_start = true;

        yield return null;
    }
}