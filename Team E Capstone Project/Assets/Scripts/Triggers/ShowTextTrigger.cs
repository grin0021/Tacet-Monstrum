// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 28/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles text showing up for the player on screen
ChangeLog:
28/11/2021: Script created
01/12/2021: Added comments and changelog
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTextTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerController;   // Reference to the PlayerController
    [SerializeField] private string m_textToShow;                   // Text to show on screen
    [SerializeField] private EAlignementType m_alignementType;      // Alignment type
    [SerializeField] private float m_lengthToShow = 1.0f;           // The length for the text box
    [SerializeField] private bool m_canRepeat = false;              // Bool that handles can it be repeated

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Values
        if (m_textToShow == null)
        {
            m_textToShow = "";
        }

        if (m_playerController == null)
        {
            Debug.LogError($"Error in {GetType()}: Missing Player Controller Reference");
        }
    }

    // Called when the TriggerBox collides with something
    private void OnTriggerEnter(Collider other)
    {
        // If the gameObject is a player
        if (other.gameObject.tag == "Player")
        {
            m_playerController.GetUIAppear().ShowText(m_textToShow, m_alignementType, m_lengthToShow);

            if (m_canRepeat == false)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}