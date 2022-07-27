// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 10/11/2021

/*
Author: Aviraj Singh
Class Description: Class that handles the Text showing up on screen
10/12/2021: Added comments and refined the script
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Enum used for where to show the text on screen
public enum EAlignementType
{
    Bottom,
    Middle,
    Top
}

public class ShowTextUI : MonoBehaviour
{
    // References to the Bottom Panel and its text
    private Transform m_bottomPanel;
    private TMP_Text m_bottomText;

    // References to the Middle Panel and its text
    private Transform m_middlePanel;
    private TMP_Text m_middleText;

    // References to the Top Panel and its text
    private Transform m_topPanel;
    private TMP_Text m_topText;

    // Timers for all three Panels
    private float m_bottomTimer;
    private float m_middleTimer;
    private float m_topTimer;

    // Bools to check if a panel is being shown
    private bool b_isBottomTextShowing;
    private bool b_isMiddleTextShowing;
    private bool b_isTopTextShowing;


    // Start is called before the first frame update
    void Start()
    {
        Transform TempTransform;

        // Sets the references for all three panels and their text
        m_bottomPanel = this.transform.GetChild(0);
        TempTransform = m_bottomPanel.transform.GetChild(0);
        m_bottomText = TempTransform.GetComponent<TMP_Text>();

        m_middlePanel = this.transform.GetChild(1);
        TempTransform = m_middlePanel.transform.GetChild(0);
        m_middleText = TempTransform.GetComponent<TMP_Text>();

        m_topPanel = this.transform.GetChild(2);
        TempTransform = m_topPanel.transform.GetChild(0);
        m_topText = TempTransform.GetComponent<TMP_Text>();

        // Sets all the timers to 0.0f
        m_bottomTimer = 0.0f;
        m_middleTimer = 0.0f;
        m_topTimer = 0.0f;
    }

    // Update is called every frame
    void Update()
    {
        if (b_isBottomTextShowing == true)
        {
            if (m_bottomTimer <= 0.0f)
            {
                // Hide the text and panel that is being shown
                StartCoroutine(FadeOutText(EAlignementType.Bottom));
            }
            else
            {
                m_bottomTimer -= Time.deltaTime;
            }
        }

        if (b_isMiddleTextShowing == true)
        {
            if (m_middleTimer <= 0.0f)
            {
                // Hide the text and panel that is being shown
                StartCoroutine(FadeOutText(EAlignementType.Middle));
            }
            else
            {
                m_middleTimer -= Time.deltaTime;
            }
        }

        if (b_isTopTextShowing == true)
        {
            if (m_topTimer <= 0.0f)
            {
                // Hide the text and panel that is being shown
                StartCoroutine(FadeOutText(EAlignementType.Top));
            }
            else
            {
                m_topTimer -= Time.deltaTime;
            }
        }
    }

    // Main method to show text on screen
    public void ShowText(string text, EAlignementType alignement, float lengthToShow)
    {
        if (alignement == EAlignementType.Bottom && b_isBottomTextShowing == false)
        {
            m_bottomTimer = lengthToShow;                                           // Set the timer of the panel to the passed in length
            m_bottomPanel.gameObject.SetActive(true);                               // Sets the panel to be active
            m_bottomPanel.localScale = Vector3.one;                                 // Sets the scale of the Panel back to normal, in-case this is overwriting an in-progress panel animation
            m_bottomText.text = text;                                               // Sets the text to the passed in value
            b_isBottomTextShowing = true;                                           // Sets that the panel is being shown to true
        }
        else if (alignement == EAlignementType.Middle && b_isMiddleTextShowing == false)
        {
            m_middleTimer = lengthToShow;
            m_middlePanel.gameObject.SetActive(true);
            m_middlePanel.localScale = Vector3.one;                                 // Same as above
            m_middleText.text = text;
            b_isMiddleTextShowing = true;
        }
        else if (alignement == EAlignementType.Top && b_isTopTextShowing == false)
        {
            m_topTimer = lengthToShow;
            m_topPanel.gameObject.SetActive(true);
            m_topPanel.localScale = Vector3.one;                                    // Same as above
            m_topText.text = text;
            b_isTopTextShowing = true;
        }
        else
        {
            Debug.Log(alignement + " Was not available and the text was not displayed!");
        }
    }

    // Coroutine for text fading out
    IEnumerator FadeOutText(EAlignementType alignement)
    {
        // Based on the passed in alignement, lower our x and y scale to 0 to simulate fading out
        if (alignement == EAlignementType.Bottom)
        {
            while (m_bottomPanel.localScale.x > 0)
            {
                float temp = m_bottomPanel.localScale.x;
                temp -= Time.deltaTime / 2;

                m_bottomPanel.localScale = new Vector3(temp, temp, 1.0f);
                yield return null;
            }
            m_bottomTimer = 0.0f;
            m_bottomPanel.gameObject.SetActive(false);
            b_isBottomTextShowing = false;
            yield return null;
        }
        else if (alignement == EAlignementType.Middle)
        {
            while (m_middlePanel.localScale.x > 0)
            {
                float temp = m_middlePanel.localScale.x;
                temp -= Time.deltaTime / 2;

                m_middlePanel.localScale = new Vector3(temp, temp, 1.0f);
                yield return null;
            }
            m_middleTimer = 0.0f;
            m_middlePanel.gameObject.SetActive(false);
            b_isMiddleTextShowing = false;
            yield return null;
        }
        else if (alignement == EAlignementType.Top)
        {
            while (m_topPanel.localScale.x > 0)
            {
                float temp = m_topPanel.localScale.x;
                temp -= Time.deltaTime / 2;

                m_topPanel.localScale = new Vector3(temp, temp, 1.0f);
                yield return null;
            }
            m_topTimer = 0.0f;
            m_topPanel.gameObject.SetActive(false);
            b_isTopTextShowing = false;
            yield return null;
        }
    }

    // Clear the text from the screen
    public void ClearShowTextUI()
    {
        // Set everything back to its defaults and makes all the panels active, used incase menus are opened
        m_bottomTimer = 0.0f;
        m_bottomPanel.gameObject.SetActive(false);
        b_isBottomTextShowing = false;
        m_middleTimer = 0.0f;
        m_middlePanel.gameObject.SetActive(false);
        b_isMiddleTextShowing = false;
        m_topTimer = 0.0f;
        m_topPanel.gameObject.SetActive(false);
        b_isTopTextShowing = false;
    }
}
