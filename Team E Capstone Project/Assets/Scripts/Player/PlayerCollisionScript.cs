// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: __/__/2021

/*
Author: Bryson Bertuzzi
Date Created: 04/10/2021
Comment/Description: Main Player/Character Collision script
ChangeLog:
10/10/2021: Seperated PlayerCollision from PlayerController
13/11/2021: Added null checking for components
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Main Player/Character Collision script
ChangeLog:
05/11/2021: Added Example ChangeLog
04/03/2022: Added comments and refined code
*/

/*
Author: Seth Grinstead
ChangeLog:
12/11/2021: Added functionality in OnControllerColliderHit to prevent player from riding certain objects. Added Comments.
16/11/2021: Shuffled current logic to allow for rigidbody implementation in player. Added code to detect if player is grounded (for jumping)
07/04/2022: Updated any code referencing MonsterFOV (deleted script)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour
{
    private PlayerController m_playerController;    // Reference to the PlayerController

    public float m_pushPower = 1.0f;                // Pushing Power

    private AudioSource m_playerAudioSource;        // Reference to the player's audio source component

    GameObject m_camera;                            // Reference to the player's camera

    float m_damageTimer = 0.75f;
    bool m_bisDamaged = false;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (!(m_playerController = GetComponent<PlayerController>()))
            {
                Debug.LogError("Missing Player Controller Component", this);
            }

            if (!(m_playerAudioSource = m_playerController.GetAudioSource()))
            {
                Debug.LogError("Missing Audio Source Component", this);
            }

            m_camera = GameObject.Find("MainCamera");
        }
    }

    private void Update()
    {
        if (m_bisDamaged)
        {
            m_damageTimer -= Time.deltaTime;

            if (m_damageTimer <= 0.0f)
            {
                m_bisDamaged = false;
                m_damageTimer = 0.75f;
            }
        }
    }

    // Called when something enters the Object's triggerbox
    void OnTriggerEnter(Collider other)
    {
        // If trigger object is a Audio Zone
        if (other.tag == "Audio Zone")
        {
            m_playerAudioSource.enabled = true;
        }

        // If trigger object is a hiding place
        if (other.tag == "Hiding Place")
        {
            m_playerController.SetIsHidden(true);
        }

        if (other.tag == "Sanity Loss (test)")
        {
            m_camera.GetComponent<SanityLossEvent>().SetBegin(true);
            GetComponent<SanityComponent>().LoseSanity(5.0f);
        } 
          
        if (other.tag == "Damage")
        {
            if (!m_bisDamaged)
            {
                GetComponent<HealthComponent>().TakeDamage(10.0f);

                other.GetComponentInParent<MonsterDetection>().SearchForPlayer();

                m_bisDamaged = true;
            }
        }
    }

    // Called when something exits the Object's triggerbox
    void OnTriggerExit(Collider other)
    {
        // If trigger object is a Audio Zone
        if (other.tag == "Audio Zone")
        {
            m_playerAudioSource.enabled = false;
        }

        // If trigger object is a hiding place
        if (other.tag == "Hiding Place")
        {
            m_playerController.SetIsHidden(false);
        }
    }

    // Called when something collides with the object
    void OnCollisionEnter(Collision Col)
    {
        // If the player is not grounded
        if (GetComponent<PlayerMovementScript>().bIsGrounded == false)
        {
            PlayerMovementScript move = GetComponent<PlayerMovementScript>();

            RaycastHit ray;
            // Fire raycast downwards from player
            if (Physics.SphereCast(transform.position, 0.5f, Vector3.down, out ray, 3.0f, 9))
            {
                // If detected object is the same as colliding object, and is not an object held by player
                if (ray.collider.gameObject == Col.gameObject && Col.gameObject != m_playerController.playerHand.GetHeldObject())
                     move.bIsGrounded = true;
            }
        }

        // If other object is monster
        if (Col.gameObject.layer == 6)
        {
            Col.gameObject.GetComponent<MonsterDetection>().SearchForPlayer();

            RaycastHit ray;
            // SphereCast downwards from player
            if (Physics.SphereCast(transform.position, 0.5f, Vector3.down, out ray, 2.0f))
            {
                // If ray's detected object is same as colliding object
                if (ray.collider.gameObject == Col.gameObject)
                {
                    Vector3 dir = transform.position - Col.gameObject.transform.position;
                    dir.Normalize();
                    dir.y = 0.0f;

                    // Move player off of object
                    transform.position += 7.5f * dir * Time.deltaTime;
                }
            }
        }
        else
        {
            Rigidbody rigidbody = Col.collider.attachedRigidbody;

            // If hit object is not above player
            if (rigidbody && rigidbody.transform.position.y < transform.position.y + 0.5f)
            {
                // If player is moving
                if (GetComponent<Rigidbody>().velocity != Vector3.zero)
                {
                    // Apply the push
                    rigidbody.velocity = -Col.relativeVelocity * m_pushPower;
                }
            }
        }
    }
}
