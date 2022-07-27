// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Script for Player's flashlight behaviour
ChangeLog:
10/11/2021: Changed all input to be derived from MouseKeyController
*/

/*
Author: Seth Grinstead
ChangeLog:
11/11/2021: Comments added for readability, fixed errors in audio emission
31/01/2022: MouseKeyPlayerController references player's version instead of creating new object
07/04/2022: Fixed audio emission to work with newest version
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Script for Player's flashlight behaviour
ChangeLog:
14/11/2021: Added null checking for components
*/

/*
Author: Jacob Kaplan-Hall
ChangeLog
14/11/21: changed script to work with new audio system
*/

/*
Author: Matthew Beaurivage
ChangeLog
13/04/22: Added animator to script to have the crank animation play while pressing down the crank button
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    bool m_bIsOn = false;                                      // Is light on?
                                                               
    public GameObject lightObject;                             // Stored reference to flashlight's light GameObject
                                                               
    Light m_light;                                             // Stored reference to lightObject's light component
                                                               
    AudioSource m_audioSource;                                 // Stored reference to flashlights's audio source component

    [SerializeField]
    AudioClip m_FlashlightSound;
    MouseKeyPlayerController m_mouseKeyPlayer { get; set; }    // Player input detection

    float m_chargeTime = 0.0f;                                 // Current charge time on flashlight (Battery charge)
    float m_maxCharge = 20.0f;                                 // Max time flashlight can store (Max battery charge)

    float m_maxIntensity;                                      // Max intensity of flashlight's light
    float m_intensityCharge = 5.0f;                            // Necessary charge for light to have full intensity

    public ItemObject flashlightItem;

    public Animator ArmAnimator;                               // Animator for player arms

    void Start()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (m_mouseKeyPlayer == null)
            {
                // Create new MouseKeyPlayerController Object
                m_mouseKeyPlayer = GameObject.Find("PR_Player").GetComponent<PlayerMovementScript>().Controller;
            }
        }

        // Get Light component from lightObject and store it in m_light
        m_light = lightObject.GetComponent<Light>();

        // Set m_maxIntensity to m_light's intensity
        m_maxIntensity = m_light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        // If Player toggles flashlight's light
        if (m_mouseKeyPlayer.ToggleFlashlight())
        {
            ToggleLight();
        }

        // If Player cranks flashlight
        if (m_mouseKeyPlayer.CrankFlashlight())
        {
            CrankFlashlight();
            // sets the crank animator to be cranking
            ArmAnimator.SetBool("Can_Crank", true);
        }
        else
        {
            // sets the crank animator to be not cranking
            ArmAnimator.SetBool("Can_Crank", false);
        }
        
        // reference to the animation state info object
        AnimatorStateInfo StateInfo = ArmAnimator.GetCurrentAnimatorStateInfo(0);
        // if the animator is still cranking then keep playing the audio
        if (StateInfo.IsName("Crank"))
        {
            if (!m_audioSource || m_audioSource.isPlaying == false)
            {
                Sound sound = AudioManager.PlaySound(transform, m_FlashlightSound, null, 1.0f, 0.75f);
                sound.SetEffectsInput(AudioManager.GetMasterMixerGroup("SFX"));
                
                // Store audio source that sound clip is attached to
                m_audioSource = sound.Source;
            }

            // if not null change pitch
            if (m_audioSource != null)
            {
                // Change sound pitch based on charge percentage
                if (m_chargeTime / m_maxCharge > 0.75f)
                    m_audioSource.pitch = 5.0f;
                else if (m_chargeTime / m_maxCharge < 0.75f)
                    m_audioSource.pitch = 3.0f;
            }
        }
        // else stop playing the audio
        else
        {
            if (m_audioSource)
                m_audioSource.Stop();
        }

        // If flashlight is on, discharge battery
        if (m_bIsOn)
            Discharge();

        // If m_chargeTime is less than necessary charge for max light intensity
        if (m_chargeTime < m_intensityCharge)
        {
            m_light.intensity = m_maxIntensity * (m_chargeTime / m_intensityCharge);
        }
        else if (m_chargeTime >= m_intensityCharge)
        {
            m_light.intensity = m_maxIntensity;
        }
    }

    void CrankFlashlight()
    {
        // If flashlight is not fully charged
        if (m_chargeTime != m_maxCharge)
        {
            // Charge flashlight using deltaTime
            m_chargeTime += Time.deltaTime * 4;

            // If m_chargeTime is equal or larger than m_maxCharge, set to m_maxCharge
            if (m_chargeTime >= m_maxCharge)
                m_chargeTime = m_maxCharge;
        }
    }

    void ToggleLight()
    {
        // Toggle flashlight boolean
        m_bIsOn = !m_bIsOn;

        // If m_bIsOn is false, set lightObject inactive in scene
        if (!m_bIsOn)
            lightObject.SetActive(false);
    }

    void Discharge()
    {
        // If adequate charge (m_chargeTime), set lightObject active in scene
        if (m_chargeTime > 0.2f)
            lightObject.SetActive(true);

        // If inadequate charge (m_chargeTime), set lightObject inactive in scene
        if (m_chargeTime <= 0.1f)
            lightObject.SetActive(false);

        // Subtract deltaTime from m_chargeTime
        m_chargeTime -= Time.deltaTime;

        // If m_chargeTime is equal to or less than, set to zero 
        if (m_chargeTime <= 0.0f)
            m_chargeTime = 0.0f;
    }
}