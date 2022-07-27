// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 04/10/2021
Comment/Description: Health Component Script
ChangeLog:
10/10/2021: Seperated Health & Sanity from Player
13/10/2021: Fixed Naming Convention from HealthSystem to HealthComponent
14/11/2021: Added null checking for Health Bar
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Main Player/Character Input Script
ChangeLog:
12/11/2021: Added ChangeLog
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base Script for HealthComponent of the Player
public class HealthComponent : MonoBehaviour
{
    public int MaxHealth = 100;                         // Max Health of Player
    public float CurrentHealth { get; private set; }    // Current Health of Player
    public HealthBar HealthBar;                         // UI Health Bar
    private PlayerController m_playerController;
    [SerializeField] private AudioClip m_damageClip;

    // Start is called before the first frame update
    void Start()
    {
        m_playerController = this.GetComponentInParent<PlayerController>();

        if (HealthBar == null)
        {
            Debug.LogError("Missing Health Bar", this);
        }

        // Player Health
        CurrentHealth = MaxHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        HealthBar.UpdateHealthStatus(CurrentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    // Method called whenever the player uses the health
    void HealthInput()
    {
        // If X is pressed, Current Health takes 10 damage
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(10);
        }

        // If C is pressed, increase Current Health to 20
        if (Input.GetKeyDown(KeyCode.C))
        {
            HealPlayer(20);
        }

        // If M is pressed, reset Current Health to 100
        if (Input.GetKeyDown(KeyCode.M))
        {
            CurrentHealth = 0;
            HealPlayer(100);
        }
    }

    // TakeDamage is called when player takes damage
    public void TakeDamage(float damage)
    {
        if (m_playerController.GetPlayerMovement().bPlayerIsDead == false)
        {
            m_playerController.slashEffect.FlashScreen(1.0f, 0.90f, Color.red);
            m_playerController.flashEffect.FlashScreen(0.5f, 0.1f, Color.red);
            m_playerController.GetAudioSource().clip = m_damageClip;
            m_playerController.GetAudioSource().Play();
            CurrentHealth -= damage;
            HealthBar.UpdateHealthStatus(CurrentHealth);

            if (CurrentHealth <= 0)
            {
                Debug.Log("You are dead.");
                m_playerController.OnDeath();
            }
        }

    }

    // HealPlayer is called when player heals
    public void HealPlayer(int healAmount)
    {
        CurrentHealth += healAmount;
        HealthBar.UpdateHealthStatus(CurrentHealth);
    }
}