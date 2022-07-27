// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 18/10/2021
Comment/Description: Sanity Med Script
ChangeLog:
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Sanity Med Script
ChangeLog:
12/11/2021: Added ChangeLog
12/11/2021: Added comments to vairables and methods
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Child of Interactables holding values for SanityMed
public class SanityMed : Interactables
{
    public int HealAmount = 10;     // Heal amount

    // Function called when the player uses the HealthKit
    public void UseSanityMed(SanityComponent sanity)
    {
        sanity.GainSanity(HealAmount);
        Destroy(gameObject);
    }
}