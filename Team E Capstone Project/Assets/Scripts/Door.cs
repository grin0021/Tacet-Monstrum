// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 03/10/2021
Comment/Description: Script handling the Door's Animator
ChangeLog:
14/11/2021: Added null checking for Sanity Bar
22/11/2021: Removed Animator from door and using Hinge Joints instead
24/11/2021: Door script now has functions for locked and unlocked doors
26/11/2021: Added partner door functions for double door scenarios
02/11/2021: Fixed audio not disabling alarm
09/12/2021: Fixed Door making sounds on start
24/02/2022: Added MaterialSwap trigger reference to door
22/03/2022: Added Open Partner door function for double door scenarios
06/04/2022: Optimized script and added updater for partner door
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Script handling the Door's Animator
ChangeLog:
11/11/2021: Updated Changelog and added comments
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class handling simple door with Animator component
public class Door : MonoBehaviour
{
    [Header("Door Setup")]
    public int doorUnlockID = -1;	                        // Item ID
    [HideInInspector] public int NumLeversPulled = 0;       // Current number of levers pulled
    public int NumOfLeversReq = 1;                          // Number of levers required to open door

    private HingeJoint m_hingeJoint;                        // Reference to Hinge Joint Component
    private bool m_bisMotorOn = false;                      // Is the Hinge Joint Motor On

    private DoorSound m_doorSound;                          // Door Sound Script

    [Header("Door Type")]
    public bool bIsDoorLocked = false;                      // Is the door a locked door
    public bool bIsMirrored = false;                        // Is the door a opposite door
    public bool bIsDualLocked = false;                      // Is the door a two-step door

    [Header("Audio")]
    [SerializeField] private AudioClip OpeningDoorSound;    // Opening Door Sound Clip
    [SerializeField] private AudioClip LockedDoorSound;     // Locked Door Sound Clip 
    private AudioSource m_audioSource;                      // Reference to Audio Source Component

    [Header("Partner Door Setup")]
    public Door PartnerDoor = null;                         // Reference to partner Door script

    public bool bIsKeyCardDone = false;                     // Is the keycard lock done
    public bool bIsKeypadDone = false;                      // Is the keypad lock done   

    [Header("Misc")]
    public MaterialSwap MaterialSwap;                       // Reference to trigger MaterialSwap    

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (!(m_hingeJoint = GetComponent<HingeJoint>()))
            {
                Debug.LogError($"Error in {GetType()}: Missing Hinge Joint");
            }

            if (!(m_doorSound = GetComponent<DoorSound>()))
            {
                Debug.LogError($"Error in {GetType()}: Missing Door Sound Script");
            }

            if (!(gameObject.GetComponent<Rigidbody>()))
            {
                Debug.LogError($"Error in {GetType()}: Missing Rigidbody");
            }

            if (!(m_audioSource = GetComponent<AudioSource>()))
            {
                Debug.LogError($"Error in {GetType()}: Missing Audio Source");
            }

            if (OpeningDoorSound == null)
            {
                Debug.LogError($"Error in {GetType()}: Missing Opening Door Sound");
            }

            if (LockedDoorSound == null)
            {
                Debug.LogError($"Error in {GetType()}: Missing Locked Door Sound");
            }
        }

        // If the door is a locked door...
        if (bIsDoorLocked == true)
        {
            // Setup Locked Door
            LockedDoorSetup();
        }
        else
        {
            // Set Door Sound to true
            m_doorSound.enabled = true;

            // Setup Unlocked Door
            UnlockedDoorSetup();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // If PartnerDoor is not null and its motor is on...
        if (PartnerDoor != null && PartnerDoor.m_bisMotorOn == true)
        {
            // Open the door
            OpenDoor();

            // Set PartnerDoor to null
            PartnerDoor = null;
        }

        // If the Hinge Joint exists and the motor is on...
        if (m_hingeJoint != null && m_bisMotorOn == true)
        {
            // If the Hinge Joint angle is less than or equal to the Hinge Joint min limit...
            if (m_hingeJoint.angle <= m_hingeJoint.limits.min)
            {
                // Set Hinge Joint Motor to false
                SetJointMotor(false, 0, 0);

                // Disable isMotorOn
                m_bisMotorOn = false;

                StartCoroutine(WaitTime());
            }
        }

        // If duel lock, keycard done, and keypad done are all true... 
        if (bIsDualLocked == true && bIsKeyCardDone == true && bIsKeypadDone == true)
        {
            // Set dual locked to false
            bIsDualLocked = false;

            // Open the door
            OpenDoor();
        }
    }

    // UnlockedDoorSetup is called when setting up the unlocked door
    void UnlockedDoorSetup()
    {
        // Joint Limits Setup
        SetJointLimits(-90.0f, 90.0f);

        // Spring Setup
        SetJointSpring(false, 0);

        // Set Door Sound to false
        m_doorSound.IsEnabled = false;
    }

    // LockedDoorSetup is called when setting up the locked door
    public void LockedDoorSetup()
    {
        // Disable Door Sound Script
        m_doorSound.enabled = false;

        // Joint Limits Setup
        {
            if (bIsMirrored == true)
            {
                // Set Hinge Joint limits to new limits
                SetJointLimits(10.0f, 0.0f);
            }
            else
            {
                // Set Hinge Joint limits to new limits
                SetJointLimits(-10.0f, 0.0f);
            }
        }

        // Spring Setup
        SetJointSpring(true, 100.0f);
    }

    // OpenDoor is called when locked door has been unlocked
    public void OpenDoor()
    {
        UnlockedDoorSetup();

        // Joint Motor Setup
        {
            // Set Hinge Joint motor to new motor
            SetJointMotor(true, 100, 90);

            // Toggle bisMotorOn to true
            m_bisMotorOn = true;
        }

        // Audio Setup
        {
            // Set Audio Source Clip to OpeningDoorSound 
            m_audioSource.clip = OpeningDoorSound;

            // Play Audio Clip
            m_audioSource.Play();
        }

        // Misc
        {
            // Toggle bIsDoorLocked to false
            bIsDoorLocked = false;
        }

        if (NumLeversPulled == NumOfLeversReq)
        {
            GameObject.Find("PR_SceneSoundManager").GetComponent<SceneSoundManager>().StopAudio(0.0f);
        }

        // If MaterialSwap is available...
        if (MaterialSwap != null)
        {
            // Reset trigger objects material
            MaterialSwap.ResetMaterials();
        }
    }

    // PlayLockedDoorSound is called when jiggling locked door
    public void PlayLockedDoorSound()
    {
        m_audioSource.clip = LockedDoorSound;
        m_audioSource.Play();
    }

    // SetJointLimits is called when setting up Hinge Joint Limits
    private void SetJointLimits(float newMin, float newMax)
    {
        // Initialize a Joint Limits from Hinge Joint 
        JointLimits newLimits = m_hingeJoint.limits;

        // Set limit minimum to new minimum
        newLimits.min = newMin;

        // Set limit maximum to new maximum
        newLimits.max = newMax;

        // Set Hinge Joint limits to new limits
        m_hingeJoint.limits = newLimits;
    }

    // SetJointSpring is called when setting up Hinge Joint Spring
    private void SetJointSpring(bool isEnabled, float springForce)
    {
        JointSpring newSpring = m_hingeJoint.spring;

        // Make spring apply a force of 100
        newSpring.spring = springForce;

        // Set Hinge Joint spring to new spring
        m_hingeJoint.spring = newSpring;

        // Diable Hinge Joint Spring
        m_hingeJoint.useSpring = isEnabled;
    }

    // SetJointMotor is called when setting up Hinge Joint Motor
    private void SetJointMotor(bool isEnabled, float force, float targetVelocity)
    {
        // Initialize a Joint Motor from Hinge Joint 
        JointMotor newMotor = m_hingeJoint.motor;

        // Make motor apply a force of 100
        newMotor.force = force;

        if (bIsMirrored == true)
        {
            // Make motor apply a force up to a target velocity of 90
            newMotor.targetVelocity = targetVelocity;
        }
        else
        {
            // Make motor apply a force up to a target velocity of -90
            newMotor.targetVelocity = -targetVelocity;
        }

        // Disable Free Spin on motor to slow down after acceleration
        newMotor.freeSpin = false;

        // Set Hinge Joint motor to new motor
        m_hingeJoint.motor = newMotor;

        // Enable Hinge Joint motor
        m_hingeJoint.useMotor = isEnabled;
    }

    // UpdatePartnerDoor is called when partner door needs syncing
    public void UpdatePartnerDoor()
    {
        // If partner door is not null...
        if (PartnerDoor != null)
        {
            // Resync Partner Door
            PartnerDoor.bIsDualLocked = bIsDualLocked;
            PartnerDoor.bIsKeyCardDone = bIsKeyCardDone;
            PartnerDoor.bIsKeypadDone = bIsKeypadDone;
            PartnerDoor.bIsDoorLocked = bIsDoorLocked;
        }
    }

    // WaitTime is called when functions require waiting before executing
    IEnumerator WaitTime()
    {
        // Wait 2 Seconds before executing next line
        yield return new WaitForSecondsRealtime(2);

        // Enable Door Sound script
        m_doorSound.enabled = true;
        m_doorSound.IsEnabled = false;
    }
}