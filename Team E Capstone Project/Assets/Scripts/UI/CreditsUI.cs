// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 17/11/2021

/*
Author: Bryson Bertuzzi
Date Created: 25/03/2022
Comment/Description: Credits Canvas UI Controller
ChangeLog:
14/11/2021: Added null checking for components

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    private Animation m_animation;
    public Canvas MainMenuCanvas;

    // Start is called before the first frame update
    void Awake()
    {
        m_animation = gameObject.GetComponent<Animation>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!m_animation.isPlaying)
        {
            m_animation.Stop();
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        m_animation.Play();
    }

    private void OnDisable()
    {
        MainMenuCanvas.gameObject.SetActive(true);
    }
}