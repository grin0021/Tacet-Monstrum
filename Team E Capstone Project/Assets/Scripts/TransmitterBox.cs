// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

/*
Author: Seth Grinstead
Comment/Description: Script handling radio sound output based on volume knob
ChangeLog:
20/10/2021: Created script
11/11/2021: Added comments for readability
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Script handling radio sound output based on volume knob
ChangeLog:
14/11/2021: Added null checking for Sanity Bar
*/

/*
Author: Jacob Kaplan-Hall
ChangeLog
14/11/21: changed script to work with new audio system
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmitterBox : MonoBehaviour
{
    AudioSource m_radio;              // Reference to object's AudioSource
    AudioSource m_Source;
    TransmitterKnob m_volumeControl;  // Reference to object's TransmitterKnob

    public GameObject Knob;           // Reference to volume knob in scene

    bool m_bReadyToPlay = true;       // Is audio ready to play?
    public bool bLoop = false;        // Is audio meant to loop?

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (Knob == null)
            {
                Debug.LogError("Missing Knob", this);
            }

            if (!(m_radio = GetComponent<AudioSource>()))
            {
                Debug.LogError("Missing Audio Source Component", this);
            }


            if (!(m_volumeControl = Knob.GetComponent<TransmitterKnob>()))
            {
                Debug.LogError("Missing Transmitter Knob Script", this);
            }
        }
        // Variable initialization
        m_radio = GetComponent<AudioSource>();
        m_volumeControl = Knob.GetComponent<TransmitterKnob>();
    }

    // Update is called once per frame
    void Update()
    {
        // Play sound
        PlaySound();
    }

    void PlaySound()
    {
        // If volume knob exists
        if (m_volumeControl)
        {
            // If volume knob returns volume greater than zero
            if (m_volumeControl.GetVolume() > 0.0f)
            {
                m_Source.volume = m_volumeControl.GetVolume();

                // If audio is ready to play
                if (m_bReadyToPlay == true)
                {
                    AudioManager.PlaySound(transform,m_radio.clip);
                    m_bReadyToPlay = false;
                }
            }
            else
            {
                m_radio.Stop();
                m_bReadyToPlay = true;
            }

            // If audio is not playing and is meant to loop
            if (m_radio.isPlaying == false && bLoop == true)
            {
                m_bReadyToPlay = true;
            }
        }
    }
}
