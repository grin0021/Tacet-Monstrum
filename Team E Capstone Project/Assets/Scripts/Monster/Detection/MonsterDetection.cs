// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 09/11/2021

/*
Author: Seth Grinstead
Comment/Description: Script Controlling AI Detection through Heard Sounds
ChangeLog:
04/11/2021: Script created
05/11/2021: Added Example ChangeLog
08/11/2021: Separated player-associated sound detection and general sound detection for different behaviours
08/11/2021: Added trigger collision to stun monster in traps
09/11/2021: Removed trigger collision to be placed in a separate collision script
10/02/2022: Modified detection to fit with new AI system
04/07/2022: Moved FOV logic to script and commented code
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Script Controlling AI Detection through Heard Sounds
ChangeLog:
13/11/2021: Added null checks and refined code
15/11/2021: Added creation date and refined code
*/

/*
Author: Jacob Kaplan-Hall
ChangeLog
14/11/21: changed script to work with new audio system
*/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


// Script that controls Monster AI's Detection ability
public class MonsterDetection : MonoBehaviour, IAudioDetectionCallback
{
    AIController m_aiController;            // Reference to AI's AIController
    GameObject m_playerRef;                 // Reference to player object in scene
    Vector3 m_lastHeardPosition;            // Position of the last heard sound
    Vector3 m_storedDestination;            // Reference to AI's last intended destination (for vent state)
    float m_timeToLose = 5.0f;              // Time to lose sight of player
    bool m_bHasSeenPlayer = false;          // Has player been seen?

    [Header("Detection Params")]
    [Tooltip("Maximum \"view\" distance")]
    public float ViewDistance = 20.0f;      // Maximum "view" distance
    public float HearDistance = 10.0f;      // Maximum hearing radius of AI

    // Start is called before the first frame update
    void Start()
    {
        // Get AIController reference from game object
        m_aiController = GetComponent<AIController>();

        // Get player reference from AIController
        m_playerRef = m_aiController.PlayerTransform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // If the AI see the Player, start the Timer 
        LosePlayerTimer();

        // Check if the AI has lost the player
        if (!GetHasSeenPlayer())
        {
            // If AI has a target
            if (m_aiController.Target != null)
            {
                // Set Target to null
                m_aiController.Target = null;
            }
        }
        else
        {
            // If monster is not chasing/attacking/stunned
            if (m_aiController.GetCurrentState().GetName() == "Investigate State" ||
                m_aiController.GetCurrentState().GetName() == "Patrol State")
            {
                // Set Target to Player Object
                m_aiController.Target = m_playerRef;

                // Store AI state if patrol
                if (m_aiController.GetCurrentState().GetName() == "Patrol State")
                {
                    m_aiController.StoreState();
                }

                // Set AI to chase state
                m_aiController.SetState(new ChaseState(m_aiController));
            }
        }
    }

    // Fires raycast towards player
    public void SearchForPlayer()
    {
        // If the Player is within the radius
        if (Vector3.Distance(m_playerRef.transform.position, transform.position) <= ViewDistance)
        {
            // Calculate the direction towards the Player
            Vector3 Dir = (m_playerRef.transform.position - transform.position).normalized;

            // Fire raycast towards player object
            RaycastHit Ray;
            int layerMask = LayerMask.GetMask("Not in Reflection");
            if (Physics.Raycast(transform.position, Dir, out Ray, ViewDistance, layerMask))
            {
                // If ray collided with player object
                if (Ray.collider != null && Ray.collider.gameObject.tag == "Player")
                {
                    // Set has seen player to true
                    m_bHasSeenPlayer = true;

                    // Reset timer to max value
                    m_timeToLose = 5.0f;
                }
            }
            // If player is already seen
            else if (m_bHasSeenPlayer)
            {
                // Reset timer
                m_timeToLose = 5.0f;
            }
        }
    }

    // Handles behaviours when a new sound is heard
    public void OnNewSoundPlayed(ISound newSound, MixerAudioEffect[] liveEffects)
    {
        Sound sound = newSound as Sound;

        if (sound != null)
        {
            // Getting the position where the sound was last heard
            m_lastHeardPosition = sound.OutputSocket.position;

            if ((m_lastHeardPosition - transform.position).magnitude < (HearDistance * sound.GetVolume()))
            {
                // If sound heard is withing hear radius
                if ((m_lastHeardPosition - m_playerRef.transform.position).magnitude < 4.0f)
                {
                    // Search for player
                    SearchForPlayer();

                    // If AI is not in vent state
                    if (m_aiController.GetCurrentState().GetName() != "Vent State" &&
                        m_aiController.GetCurrentState().GetName() != "Stun State")
                    {
                        // If the AI "sees" the player
                        if (GetHasSeenPlayer())
                        {
                            // If monster is not already in chase state
                            if (m_aiController.GetCurrentState().GetName() != "Chase State")
                            {
                                // Set Target to Player Object
                                m_aiController.Target = m_playerRef;

                                // Store AI state if patrol
                                if (m_aiController.GetCurrentState().GetName() == "Patrol State")
                                    m_aiController.StoreState();

                                // Set AI to chase state
                                m_aiController.SetState(new ChaseState(m_aiController));
                            }
                        }
                        else
                        {
                            // Store AI state if patrol
                            if (m_aiController.GetCurrentState().GetName() == "Patrol State")
                            {
                                m_aiController.StoreState();
                            }

                            // Set AI to investigate state
                            m_aiController.SetState(new InvestigateState(m_aiController, m_lastHeardPosition));
                        }
                    }
                }
                else
                {
                    // If not in vent or stun states
                    if (m_aiController.GetCurrentState().GetName() != "Vent State" &&
                        m_aiController.GetCurrentState().GetName() != "Stun State")
                    {
                        // Store AI state if patrol
                        if (m_aiController.GetCurrentState().GetName() == "Patrol State")
                        {
                            m_aiController.StoreState();
                        }

                        // Set AI to investigate state
                        m_aiController.SetState(new InvestigateState(m_aiController, m_lastHeardPosition));
                    }
                }
            }
        }
    }

    // Timer until AI loses player
    public void LosePlayerTimer()
    {
        // If the AI see the Player, start the Timer 
        if (m_bHasSeenPlayer)
        {
            // Decrement timer by deltaTime
            m_timeToLose -= Time.deltaTime;

            // If timer is less reaches zero
            if (m_timeToLose <= 0.0f)
            {
                // Player is no longer visible
                m_bHasSeenPlayer = false;

                // Timer is reset to max value
                m_timeToLose = 5.0f;
            }
        }
    }

    // Returns the value of bool if the Player was spotted
    public bool GetHasSeenPlayer()
    {
        return m_bHasSeenPlayer;
    }

    // Returns the position of the last heard sound
    public Vector3 GetSoundLoc()
    {
        return m_lastHeardPosition;
    }

    // Store last desired destination
    public void SetLastDestination(Vector3 loc)
    {
        m_storedDestination = loc;
    }

    // Retrieve last desired destination
    public Vector3 GetLastDestination()
    {
        return m_storedDestination;
    }
}