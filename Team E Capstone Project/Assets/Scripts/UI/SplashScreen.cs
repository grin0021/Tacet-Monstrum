// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 05/04/2022
Comment/Description: Splash Screen Script
ChangeLog:
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public Sprite[] SplashScreenImages;        // Reference to Splash Screen Sprites

    private int m_indexOfArray = 0;            // Index of the array
    private bool m_bfinishedFading = false;    // Is the Fade Image State complete
    private GameObject m_imageObject;          // Reference to child gameobject

    // Start is called before the first frame update
    void Start()
    {
        m_imageObject = transform.GetChild(0).gameObject;

        // Set Image to the first splashscreen
        m_imageObject.GetComponent<Image>().sprite = SplashScreenImages[0];
        // Set Image width and height based on sprite
        m_imageObject.GetComponent<RectTransform>().sizeDelta = SplashScreenImages[0].rect.size;

        // Start coroutine for FadeImage
        StartCoroutine(FadeImage());
    }

    // FadeImage is called when SplashScreen requires fading
    private IEnumerator FadeImage()
    {
        // Get the Canvas Group
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        // While the canvas group alpha is less than 1...
        while (canvasGroup.alpha < 1)
        {
            // Increase canvas group alpha by deltaTime
            canvasGroup.alpha += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2);

        // While the canvas group alpha is greater than 0...
        while (canvasGroup.alpha > 0)
        {
            // Decrease canvas group alpha by deltaTime
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }

        // Return m_bfinishedFading as true when done function
        yield return m_bfinishedFading = true;
    }

    // LateUpdate is called every frame
    private void LateUpdate()
    {
        // If m_bfinishedFading is true...
        if (m_bfinishedFading == true)
        {
            // Set m_bfinishedFading to false
            m_bfinishedFading = false;

            // Stop Coroutine for FadeImage
            StopCoroutine(FadeImage());

            // Increase index of array
            m_indexOfArray++;

            // If index of array is greater than or equal to the length of the SplashScreenImages...
            if (m_indexOfArray >= SplashScreenImages.Length)
            {
                // Load LD_MainMenu asynchronously in the background
                SceneManager.LoadSceneAsync("LD_MainMenu", LoadSceneMode.Single);
            }
            else
            {
                // Set Image to the next splashscreen
                m_imageObject.GetComponent<Image>().sprite = SplashScreenImages[m_indexOfArray];
                // Set Image width and height based on next sprite
                m_imageObject.GetComponent<RectTransform>().sizeDelta = SplashScreenImages[m_indexOfArray].rect.size;
            }

            // Start coroutine for FadeImage
            StartCoroutine(FadeImage());
        }
    }
}