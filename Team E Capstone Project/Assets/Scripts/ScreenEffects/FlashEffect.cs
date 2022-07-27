// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 21/10/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Script handling various Door Sounds
ChangeLog:
12/11/2021: Added Comments and updated ChangeLog 
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Script handling various Door Sounds
ChangeLog:
14/11/2021: Added null checking for components
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashEffect : MonoBehaviour
{
    Image Image;                    // Stores the reference of the Image
    Coroutine CurrentFlashRoutine;  // Couritine that will be run in the background

    // Gets called when the Scene Loads up, even before Start() is called
    private void Awake()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (!(Image = GetComponent<Image>()))
            {
                Debug.LogError("Missing Image Component", this);
            }
        }
    }

    // Method that functions when the Flash has to appear
    public void FlashScreen(float numSeconds, float alpha, Color color)
    {
        Image.color = color;

        // Make sure alpha isn't greater than max and not less than min
        alpha = Mathf.Clamp(alpha, 0, 1);

        if (CurrentFlashRoutine != null)
        {
            StopCoroutine(CurrentFlashRoutine);
        }

        // Start the couritine
        CurrentFlashRoutine = StartCoroutine(Flash(numSeconds, alpha));
    }

    // The steps to be followed during the Couritine
    IEnumerator Flash(float numSeconds, float alpha)
    {
        float flashDuration = numSeconds / 2;
        for (float i = 0; i <= flashDuration; i += Time.deltaTime)
        {
            Color frameColor = Image.color;
            frameColor.a = Mathf.Lerp(0, alpha, i / flashDuration);
            Image.color = frameColor;

            // wait for next frame
            yield return null;
        }

        float flashOutDuration = numSeconds / 2;
        for (float i = 0; i <= flashOutDuration; i += Time.deltaTime)
        {
            Color frameColor = Image.color;
            frameColor.a = Mathf.Lerp(alpha, 0, i / flashOutDuration);
            Image.color = frameColor;

            // wait for next frame
            yield return null;
        }
        // Hardcoding alpha back to 0 after it's over
        Image.color = new Color32(0, 0, 0, 0);
    }
}
