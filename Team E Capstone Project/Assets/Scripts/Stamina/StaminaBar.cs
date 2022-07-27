// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 11/02/2022

/*Author: Aviraj Singh Virk
Comment/Description: Main Player/Character Controller Script
ChangeLog:
04/03/2022: Added Example ChangeLog and added comments
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private Image m_imageRef;                               // Reference to the image (stamina slider)
    private float m_currentFill;                            // Our current fill value of the slider image
    private int m_currentStamina;                           // Our current stamina (taken from the player)
    private int m_maxStamina;                               // Our max stamina (taken from the player)
    [SerializeField] private float m_lerpSpeed;             // Lerping speed of the stamina bar

    // Start is called before the first frame update
    void Start()
    {
        m_imageRef = GetComponent<Image>();
    }

    // Method to update the Stamina Bar
    public void SetCurrentStamina(int stamina)
    {
        // If the stamina we want to set is too high, just set it to the max stamina value
        if (stamina > m_maxStamina)
        {
            m_currentStamina = m_maxStamina;
        }
        // If the stamina we want to set is too low, just set it to the minimum value
        else if (stamina < 0)
        {
            m_currentStamina = 0;
        }
        else
        {
            m_currentStamina = stamina;
        }

        // Convert the current and max stamina division to a float so that we can properly lerp the slider fill
        m_currentFill = ((float)m_currentStamina) / ((float)m_maxStamina);
    }


    // Update is called once per frame
    void Update()
    {
        if (m_currentFill != m_imageRef.fillAmount)
        {
            // Lerp the fill amount based on lerp speed (gives that smooth bar filling effect)
            m_imageRef.fillAmount = Mathf.Lerp(m_imageRef.fillAmount, m_currentFill, Time.deltaTime * m_lerpSpeed);
        }
    }

    // Initializing Stamine Component
    public void Initialize(int currentStamina, int maxStamina)
    {
        m_maxStamina = maxStamina;
        m_currentStamina = currentStamina;
    }

}
