// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles the working of the Doors attached to a Trigger
ChangeLog:
20/10/2021: Created basic trigger and added reference for the Door attached
25/10/2021: Added the access to Door's animator function to open the door
05/10/2021: Added Math to calculate mass on Pressure Plate
13/11/2021: Added null checks and refined variables names
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Class that handles the working of the Doors attached to a Trigger
ChangeLog:
22/03/2022: Added feature to reveal hidden object after pressure plate objective is complete
12/04/2022: Added Sound Cue for Pressure Plate when objects are on and off
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Door m_Door;             // Provide the connected Door

    public GameObject RewardObject;

    [SerializeField]
    private float m_reqMass;        // Set the minmum required mass to activate the Trigger

    bool m_IsActive;                // Checks if the Pressure Plate is active or not

    [SerializeField]
    private float m_curMass;        // Keeps track of the MASS currerntly on the pressure plate

    public AudioSource AudioFeedback;   // Reference for Audio Source

    public AudioClip[] AudioClips;      // Reference for Audio Clips

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        m_IsActive = true;
        m_curMass = 0.0f;
    }

    // Called when something enters the TriggerBox of the this GameObject
    private void OnTriggerEnter(Collider other)
    {
        // Check if the Trigger is still active
        if (m_IsActive)
        {
            // Check if the other object is Interactable
            if (other.gameObject.tag == "Interactable Object")
            {
                // Store the RigidBody component of the collided object in a variable
                Rigidbody objectRB = other.gameObject.GetComponent<Rigidbody>();

                // Increment the current mass according to the mass of the objects put on the pressure plate
                m_curMass += objectRB.mass;

                if (AudioFeedback != null && AudioClips != null)
                {
                    AudioFeedback.PlayOneShot(AudioClips[0]);
                }

                // Check if the current mass succeeds or equals the required mass, ACTIVATE the trigger for the door
                if (m_curMass >= m_reqMass)
                {
                    m_IsActive = false;

                    // Call OpenDoor function on the attached door
                    if (m_Door != null)
                    {
                        m_Door.OpenDoor();
                    }
                    else if (RewardObject != null)
                    {
                        RewardObject.SetActive(true);

                        if (AudioFeedback != null && AudioClips != null)
                        {
                            AudioFeedback.PlayOneShot(AudioClips[2]);
                        }
                    }
                }
            }
        }
    }

    // Called when something exits the TriggerBox of the this GameObject
    private void OnTriggerExit(Collider other)
    {
        // Check if the Pressure plate is still active
        if (m_IsActive)
        {
            // Check if the object that just left the collider was an Interactable
            if (other.gameObject.tag == "Interactable Object")
            {
                // Store the RigidBody component of the collided object in a variable
                Rigidbody objectRB = other.gameObject.GetComponent<Rigidbody>();

                // If the current mass is GREATER THAN 0, subtract the mass of the GameObject removed from the Pressure Plate
                if (m_curMass > 0)
                {
                    m_curMass -= objectRB.mass;
                    if (AudioFeedback != null && AudioClips != null)
                    {
                        AudioFeedback.PlayOneShot(AudioClips[1]);
                    }
                }
            }
        }
    }

}