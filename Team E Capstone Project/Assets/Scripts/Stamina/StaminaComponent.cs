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

public class StaminaComponent : MonoBehaviour
{
    [SerializeField] private StaminaBar m_staminaBar;               // Reference to the Stamina Bar script
    [SerializeField] private Canvas m_staminaCanvas;                // Reference to the Stamina canvas itself
    private int m_maxStamina = 100;                                 // The player's max Stamina
    private int m_curStamina;                                       // The player's current stamina
    private PlayerMovementScript m_playerMovement;                  // Reference to the player movement script
    private float m_sprintTimerInterval = 0.25f;                    // The amount of time between each sprint decrease/increase 
    private float m_audioTimerInterval = 4f;                        // the amount of time between when the breathing sound effect is allowed to play
    private bool b_audioHasPlayed = false;                          // Bool that tells you if the audio has played or not
    private AudioSource m_audioSource;                              // Reference to the stamina canvas audio source that contains the breathing

    public int StaminaCost = 5;                                     // The cost of sprinting (Modified in the inspector)
    public int StaminaRegenAmount = 5;                              // How much stamina regens per stamina regen tick (Modified in the inspector)

    // Start is called before the first frame update
    void Start()
    {
        m_curStamina = m_maxStamina;
        m_audioSource = m_staminaCanvas.GetComponent<AudioSource>();
        m_playerMovement = GameObject.Find("PR_Player").GetComponent<PlayerMovementScript>();
        // Initialize the stamina bar with values from the player's Stamina component
        m_staminaBar.Initialize(m_curStamina, m_maxStamina);
        m_staminaBar.SetCurrentStamina(m_curStamina);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_sprintTimerInterval <= 0.0f)
        {
            if (m_curStamina > 0)
            {
                // We are above 0 stamina

                if (m_curStamina != m_maxStamina && m_playerMovement.GetState() != PlayerMovementScript.EPlayerState.Sprinting &&
                    m_playerMovement.GetState() != PlayerMovementScript.EPlayerState.Jumping)
                {
                    //We aren't at max stamina, but we want to recharge anyways
                    IncreaseStamina();
                }

                if (m_playerMovement.GetState() == PlayerMovementScript.EPlayerState.Jumping)
                {
                    m_playerMovement.ForceStopSprint();

                    m_staminaCanvas.gameObject.SetActive(true);

                    if (m_curStamina != 0)
                    {
                        m_curStamina -= StaminaCost * 2;
                    }
                    m_staminaBar.SetCurrentStamina(m_curStamina);
                }

                if (m_playerMovement.GetState() == PlayerMovementScript.EPlayerState.Sprinting)
                {
                    // We're sprinting, so decrease stamina
                    DecreaseStamina();
                }
            }
            else
            {
                // No more stamina, so we force the player to stop sprinting
                m_playerMovement.ForceStopSprint();
                StartCoroutine(SprintRegenStart());
            }

            m_sprintTimerInterval = 0.25f;
        }

        m_sprintTimerInterval -= Time.deltaTime;

        if (b_audioHasPlayed == true)
        {
            m_audioTimerInterval -= Time.deltaTime;
        }

        if (m_audioTimerInterval <= 0)
        {
            m_audioTimerInterval = 4f;
            b_audioHasPlayed = false;
        }
    }

    // Method called to decrease stamina
    void DecreaseStamina()
    {
        // We display the stamina bar since the stamina value will no longer be maxed out
        m_staminaCanvas.gameObject.SetActive(true);

        if (m_curStamina != 0)
        {
            m_curStamina -= StaminaCost;
        }
        m_staminaBar.SetCurrentStamina(m_curStamina);
    }

    // Method called to refill stamina
    void IncreaseStamina()
    {
        if (m_curStamina + StaminaRegenAmount >= m_maxStamina)
        {
            m_curStamina = m_maxStamina;
            // Thsi function can only be called if we are hitting max stamina, so we fade out the stamina bar
            StartCoroutine(FadeOutStaminaBar());
        }
        else
        {
            m_curStamina += StaminaRegenAmount;
        }

        m_staminaBar.SetCurrentStamina(m_curStamina);
    }

    // Coroutine to start refilling the stamina
    IEnumerator SprintRegenStart()
    {
        // If the audio source hasn't been played yet, or enough time has passed since it last played, play the sound effect
        if (b_audioHasPlayed != true)
        {
            m_audioSource.Play();
            b_audioHasPlayed = true;
        }
        yield return new WaitForSeconds(3);
        IncreaseStamina();
    }

    // Coroutine to Descrease stamina
    IEnumerator FadeOutStaminaBar()
    {
        yield return new WaitForSeconds(1);
        m_staminaCanvas.gameObject.SetActive(false);
    }

    // Get current value of Stamina
    public int GetCurrentStamina()
    {
        return m_curStamina;
    }
}
