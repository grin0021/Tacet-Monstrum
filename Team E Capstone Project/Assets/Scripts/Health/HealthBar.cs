// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Comment/Description: Script controls the UI for the Health in Scene
ChangeLog:
14/11/2021: Fixed naming convention of variables
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Script controls the UI for the Health in Scene
ChangeLog:
11/11/2021: Added ChangeLog
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script used for UI of Health in Game
public class HealthBar : MonoBehaviour
{
    public Text StatusText;         // Text to display status in the UI

    public void UpdateHealthStatus(float health)
    {
        if (health > 75.0f)
        {
            StatusText.text = "Healthy";
        }
        else if (health > 50.0f)
        {
            StatusText.text = "Injured";
        }
        else if (health > 25.0f)
        {
            StatusText.text = "Critically Injured";
        }
        else if (health > 0.0f)
        {
            StatusText.text = "On Death's Door";
        }
    }
}
