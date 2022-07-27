// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 04/10/2021
Comment/Description: Sanity Component Script
ChangeLog:
10/10/2021: Seperated Health & Sanity from Player
13/10/2021: Fixed Naming Convention from SanitySystem to SanityComponent
13/11/2021: Added null checking for Sanity Bar
*/

/*
Author: Aviraj Singh Virk
Comment/Description: HealthKit Script
ChangeLog:
12/11/2021: Added ChangeLog
12/11/2021: Added comments to vairables and methods
*/

/*
Author: Seth Grinstead
ChangeLog:
07/04/2022: Added sanity loss and effect when monster is in player's view. Commented.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

// Base Script for HealthComponent of the Player
public class SanityComponent : MonoBehaviour
{
    public int MaxSanity = 20;                          // Max Sanity of Player
    public float CurrentSanity { get; private set; }    // Current Sanity of Player
    public SanityBar SanityBar;                         // UI Sanity Bar

    public Renderer MonsterRenderer;

    public Volume Volume;
    public GameObject Monster;

    // Start is called before the first frame update
    void Start()
    {
        if (SanityBar == null)
        {
            Debug.LogError("Missing Sanity Bar", this);
        }

        // Player Sanity
        CurrentSanity = MaxSanity;
        CurrentSanity = Mathf.Clamp(CurrentSanity, 0, MaxSanity);
        SanityBar.UpdateSanityStatus(CurrentSanity);

        Volume.enabled = false;
    }

    // Update is called once per frame
    public void Update()
    {
        SanityInput();

        CurrentSanity = Mathf.Clamp(CurrentSanity, 0, MaxSanity);

        // If the monster's renderer is visible
        if (MonsterRenderer.isVisible)
        {
            // Get direction to monster as vector
            Vector3 dir = Monster.transform.position - transform.position;

            // Raycast towards monster
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Default") | LayerMask.GetMask("Environment");
            if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, layerMask))
            {
                // If raycast hits monster
                if (hit.collider.gameObject == Monster)
                {
                    // Slowly drain sanity
                    LoseSanity(0.5f * Time.deltaTime);

                    // If sanity is below threshold
                    if (CurrentSanity / MaxSanity < 0.33f)
                    {
                        // If post-processing volume is not enabled
                        if (!Volume.enabled)
                        {
                            // Enable volume and set weight to zero
                            Volume.enabled = true;
                            Volume.weight = 0.0f;
                        }

                        // Lerp volume weight to 1.0f
                        Volume.weight = Mathf.Lerp(Volume.weight, 1.0f, 0.75f * Time.deltaTime);
                    }
                }
                else
                {
                    // If volume is enabled
                    if (Volume.enabled)
                    {
                        // Lerp volume weight to 0.0f
                        Volume.weight = Mathf.Lerp(Volume.weight, 0.0f, 2f * Time.deltaTime);

                        // If volume weight reaches zero
                        if (Volume.weight <= 0.0f)
                        {
                            // Disable volume
                            Volume.enabled = false;
                        }
                    }
                }
            }
        }
    }

    // Method called whenever the player uses the health
    public void SanityInput()
    {
        // If T is pressed, reset Current Sanity to 20
        if (Input.GetKeyDown(KeyCode.T))
        {
            CurrentSanity = 0;
            GainSanity(20);
        }

        // If Z is pressed, decrease Current Sanity by 5
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (CurrentSanity > 0)
            {
                LoseSanity(5);
            }
        }
    }

    // LoseSanity is called when player losses sanity
    public void LoseSanity(float damage)
    {
        CurrentSanity -= damage;
        SanityBar.UpdateSanityStatus(CurrentSanity);
    }

    // GainSanity is called when player gains sanity
    public void GainSanity(int sanityGained)
    {
        CurrentSanity += sanityGained;
        SanityBar.UpdateSanityStatus(CurrentSanity);
    }
}