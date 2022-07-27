// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 18/10/2021
Comment/Description: HealthKit Script
ChangeLog:
*/

/*
Author: Aviraj Singh Virk
Comment/Description: HealthKit Script
ChangeLog:
12/11/2021: Added ChangeLog 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Child of Interactables holding values for HealthKit
public class HealthKit : Interactables
{
    public int HealAmount = 10;     // Heal amount

    // Function called when the player uses the HealthKit
    public void UseHealthKit(HealthComponent health)
    {
        health.HealPlayer(HealAmount);
        Destroy(gameObject);
    }
}
