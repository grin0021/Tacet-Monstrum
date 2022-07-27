// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 30/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles blinking of Lights attached
ChangeLog:
30/11/2021: Script created
01/12/2021: Added blink logic based on Sin Wave
02/12/2021: Added comments, null checks
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class handling Blinking of Lights in Scene
public class LightBlinkTrigger : MonoBehaviour
{
    public GameObject m_AttachedObject;     // Reference to the attached Object
    public bool m_bIsActive;                // Bool, for checking if trigger is active

    public bool m_bIsWorking;               // Bool, checking if the trigger is in use currently

    public float m_TimeInterval;            // Timer between max and min intensities
    public float m_IntensityOffset;         // Difference between max and min intensities

    public float m_FrameValue;              // Keep track of time passed

    Light m_AttachedComponent;              // Reference to the Light Component on GameObject

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Values
        if (m_AttachedObject == null)
        {
            Debug.Log("Attached object not set");
        }
        else
        {
            m_AttachedComponent = m_AttachedObject.GetComponent<Light>();
        }

        if (m_IntensityOffset <= 1.0f)
        {
            m_IntensityOffset = 100.0f;
        }

        m_bIsActive = true;
    }

    // Called when the Trigger collides with something
    void OnTriggerEnter(Collider other)
    {
        // If the Player is the one activating it...
        if (other.gameObject.tag == "Player")
        {
            if (m_bIsActive)
            {
                m_bIsActive = false;
                m_bIsWorking = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the trigger is in use, follow the sin wave logic to increase and decrease intensities
        if (m_AttachedComponent && m_bIsWorking)
        {
            m_FrameValue += Time.deltaTime;
            m_AttachedComponent.intensity = Mathf.Abs(m_IntensityOffset * (Mathf.Sin(m_TimeInterval * m_FrameValue)));
        }
    }
};
