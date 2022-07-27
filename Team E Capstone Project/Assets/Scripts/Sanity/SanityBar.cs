// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Comment/Description: Sanity Bar UI Script
ChangeLog:
14/11/2021: Fixed naming convention of variables
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Sanity Bar UI Script
ChangeLog:
12/11/2021: Added Example ChangeLog
12/11/2021: Added comments for methods and variables
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Main Script handling UI of Sanity in the Game
public class SanityBar : MonoBehaviour
{
    public Text StatusText;         // Text to display status in the UI

    public void UpdateSanityStatus(float sanity)
    {

        if (sanity > 15.0f)
        {
            StatusText.text = "Sane";
        }
        else if (sanity > 10.0f)
        {
            StatusText.text = "Headaches";
        }
        else if (sanity > 5.0f)
        {
            StatusText.text = "Migraines";
        }
        else if (sanity > 0.0f)
        {
            StatusText.text = "On the brink of insanity";
        }

    }
}
