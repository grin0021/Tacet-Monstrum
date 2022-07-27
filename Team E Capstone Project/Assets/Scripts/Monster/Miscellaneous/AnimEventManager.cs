using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Description: Script handling collider de/activation during attack animations
ChangeLog:
07/04/2022: Comments added
 */

public class AnimEventManager : MonoBehaviour
{
    public Collider JawCollider;                // Reference to collider on monster's jaw
    public Collider HandCollider;               // Reference to collider on monster's hand 

    PlayerController m_playerController;        // Reference to PlayerController component on player object

    public bool bCanBite = true;                // Can the monster use its bite animation
    float m_biteResetTimer = 5.0f;

    private void Start()
    {
        // Set colliders inactive
        JawCollider.enabled = false;
        HandCollider.enabled = false;

        // Find player object and PlayerController component from object
        m_playerController = GameObject.Find("PR_Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        // If bite is not ready
        if (!bCanBite)
        {
            // Decrement bite cooldown timer
            m_biteResetTimer -= Time.deltaTime;

            // If bite reset timer reaches zero
            if (m_biteResetTimer <= 0.0f)
            {
                // Reset bite timer
                m_biteResetTimer = 5.0f;

                // Set bite bool to true, bite is ready
                bCanBite = true;
            }
        }
    }

    // Enables jaw collider
    public void EnableJawCollider()
    {
        JawCollider.enabled = true;
    }

    // Disables jaw collider
    public void DisableJawCollider()
    {
        JawCollider.enabled = false;
    }

    // Enables hand collider
    public void EnableHandCollider()
    {
        HandCollider.enabled = true;
    }

    // Disables hand collider
    public void DisableHandCollider()
    {
        HandCollider.enabled = false;
    }

    // Sets necessary bools for bite animation grab effect
    public void GrabPlayer()
    {
        // If bite attack is available
        if (bCanBite)
        {
            // Set bIsBitten to true (player is grabbed)
            m_playerController.bIsBitten = true;

            // Set player velocity to zero
            m_playerController.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // Set bite bool to false, bite has cooldown
            bCanBite = false;
        }
    }

    // Releases the player after bite animation
    public void ReleasePlayer()
    {
        m_playerController.bIsBitten = false;
    }

    public bool GetIsBitten()
    {
        return m_playerController.bIsBitten;
    }
}
