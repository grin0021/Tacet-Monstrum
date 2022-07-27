// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Holds common properties for all the Interactable Items
ChangeLog:
20/10/2021: Created basic class as the base class for interactables
25/10/2021: Added common p[roperties such as Name and Mass for the Interactables
30/10/2021: Functionality to add a basic tag of Interactable to the GameObject attached
13/11/2021: Added null checks and refined variables names
02/12/2021: Minimum mass of object will be by default 5 now, as it mass below that makes the objects to jitter when interacted with
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Holds common properties for all the Interactable Items
ChangeLog:
14/11/2021: Added null checking for components
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Contains common properties of all the Interactable Objects
public class Interactables : MonoBehaviour
{
    [SerializeField]
    float m_DefaultMass;            // Set the default mass of the object to the given value

    // Called before the Scene is loaded and before the Start() is called
    void Awake()
    {
        // Null check for TagList
        if(GetComponent<TagList>() == null)
        {
            Debug.LogError("Missing Tag List", this);
        }

        // Check if the Object has a TagList component
        if (GetComponent<TagList>() != null)
        {
            // Add a tag "Interactable" tag 
            GetComponent<TagList>().AddTag("Interactable");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // If the Object has a RigidBody component
        if (GetComponent<Rigidbody>() != null)
        {
            // Check if the current set mass is greater than 5 (5 is the limit below which the objects start to jitter)
            if (m_DefaultMass >= 5)
            {   
                // Set the RigidBody mass to the set mass
                GetComponent<Rigidbody>().mass = m_DefaultMass;
            }
            else
            {
                // The default mass will always be 5 now to avoid jitteriness
                GetComponent<Rigidbody>().mass = 5.0f;
            }
        }
    }
}
