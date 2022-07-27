// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 01/11/2021

/*
Author: Bryson Bertuzzi
Date Created: 24/10/2021
Comment/Description: Script handling the Debug Menu for Debug purposes in the Game
ChangeLog:
14/11/2021: Removed Settings Window from Debug Menu
16/11/2021: Limited script to Editor only
19/11/2021: Added support for Xbox and PS4 Controllers
19/11/2021: Added FPS Counter to menu
30/03/2022: Updated to reflect new controls
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Script handling the Debug Menu for Debug purposes in the Game
ChangeLog:
12/11/2021: Added comments and updated Changelog
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR

// Class handling the UI showing in the Game for debugging purposes
public class DebugMenu : MonoBehaviour
{
    private float[] m_axes;		        // Stores number of keys on controller used
    private bool[] m_buttons;           // Stores number of keys used for various actions

    private string[] m_toolbarStrings = { "None", "Debug" }; // "Settings"
    private int m_toolbar = 0;          // Integer storing the toolbar index
    //private int m_selGrid = 0;		// Integer

    //public AudioMixer GameAudio;	    // Reference to game's AudioMixer
    //private float m_volume;			// Float containing the Volume settings
    //private int m_qualityLevel;		// Integer containing the quality levels


    public PlayerMovementScript PlayerMovement;

    private MouseKeyPlayerController.EInputType m_inputType;

    private float m_frameRate = 0;

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize Values
        m_axes = new float[4];
        m_buttons = new bool[10];

        //GameAudio.GetFloat("volume", out m_volume);
        //m_qualityLevel = QualitySettings.GetQualityLevel();
    }

    // Update is called once per frame
    private void Update()
    {
        SetControllerType();

        // Checking for Input every frame
        if (m_toolbar == 1)
        {
            m_frameRate = (int)(1.0f / Time.unscaledDeltaTime);

            if (m_inputType == MouseKeyPlayerController.EInputType.KeyboardAndMouse)
            {
                m_axes[0] = Input.GetAxis("KMHorizontal");
                m_axes[1] = Input.GetAxis("KMVertical");
                m_axes[2] = Input.GetAxis("KMX");
                m_axes[3] = Input.GetAxis("KMY");

                m_buttons[0] = Input.GetButton("KMJump");
                m_buttons[1] = Input.GetButton("KMInteract");
                m_buttons[2] = Input.GetButton("KMSprint");
                m_buttons[3] = PlayerMovement.GetCrouched();
                m_buttons[4] = PlayerMovement.GetCrawling();
                m_buttons[5] = Input.GetButton("KMThrow");
                m_buttons[6] = Input.GetKey(KeyCode.G);
                m_buttons[7] = Input.GetKey(KeyCode.F);
                m_buttons[8] = Input.GetKey(KeyCode.Q);
                m_buttons[9] = Input.GetKey(KeyCode.E);
            }

            if (m_inputType == MouseKeyPlayerController.EInputType.XboxController)
            {
                m_axes[0] = Input.GetAxis("XBHorizontal");
                m_axes[1] = Input.GetAxis("XBVertical");
                m_axes[2] = Input.GetAxis("XBX");
                m_axes[3] = Input.GetAxis("XBY");

                m_buttons[0] = Input.GetButton("XBJump");
                m_buttons[1] = Input.GetAxis("XBInteract") > 0.0f;
                m_buttons[2] = Input.GetButton("XBSprint");
                m_buttons[3] = PlayerMovement.GetCrouched();
                m_buttons[4] = PlayerMovement.GetCrawling();
                m_buttons[5] = Input.GetAxis("XBThrow") > 0.0f;
                m_buttons[6] = Input.GetKey(KeyCode.JoystickButton4);
                m_buttons[7] = Input.GetKey(KeyCode.JoystickButton5);
                m_buttons[8] = Input.GetAxis("XBHorizontalDPad") < 0.0f;
                m_buttons[9] = Input.GetAxis("XBHorizontalDPad") > 0.0f;
            }

            if (m_inputType == MouseKeyPlayerController.EInputType.PS4Controller)
            {
                m_axes[0] = Input.GetAxis("PSHorizontal");
                m_axes[1] = Input.GetAxis("PSVertical");
                m_axes[2] = Input.GetAxis("PSX");
                m_axes[3] = Input.GetAxis("PSY");

                m_buttons[0] = Input.GetButton("PSJump");
                m_buttons[1] = Input.GetButton("PSInteract");
                m_buttons[2] = Input.GetButton("PSSprint");
                m_buttons[3] = PlayerMovement.GetCrouched();
                m_buttons[4] = PlayerMovement.GetCrawling();
                m_buttons[5] = Input.GetAxis("PSThrow") > 0.0f;
                m_buttons[6] = Input.GetKey(KeyCode.JoystickButton10);
                m_buttons[7] = Input.GetKey(KeyCode.JoystickButton11);
                m_buttons[8] = Input.GetAxis("PSHorizontalDPad") < 0.0f;
                m_buttons[9] = Input.GetAxis("PSHorizontalDPad") > 0.0f;
            }
        }
    }

    // Called whenever the GUI is requested on screen
    private void OnGUI()
    {
        m_toolbar = GUI.Toolbar(new Rect(Screen.width - 300.0f, 0.0f, 300.0f, 30.0f), m_toolbar, m_toolbarStrings);
        if (m_toolbar == 1)
        {
            GUI.Window(0, new Rect(Screen.width - 200.0f, 30.0f, 200, 250.0f), DebugWindow, "DebugWindow");
        }
        //else if (m_toolbar == 1)
        //{
        //    GUI.Window(0, new Rect(Screen.width - 200.0f, 30.0f, 200, 200), SettingsWindow, "SettingsWindow");
        //}
        else
        {
        }
    }

    // Method called whenever the Debug window is in use/function
    private void DebugWindow(int windowID)
    {
        GUILayout.Label("FPS: " + m_frameRate);

        GUILayout.BeginHorizontal();
        GUILayout.Label("X: " + Mathf.Round(m_axes[0]));
        GUILayout.Label("Y: " + Mathf.Round(m_axes[1]));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Look X: " + Mathf.Round(m_axes[2]));
        GUILayout.Label("Look Y: " + Mathf.Round(m_axes[3]));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Jump: " + m_buttons[0]);
        GUILayout.Label("Sprint: " + m_buttons[2]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Pickup: " + m_buttons[1]);
        GUILayout.Label("Throw: " + m_buttons[5]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Crouch: " + m_buttons[3]);
        GUILayout.Label("Crawl: " + m_buttons[4]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Crank: " + m_buttons[6]);
        GUILayout.Label("Flash: " + m_buttons[7]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Lean L: " + m_buttons[8]);
        GUILayout.Label("Lean R: " + m_buttons[9]);
        GUILayout.EndHorizontal();
    }

    // Method for laying out and functioning of the settings menu
    private void SettingsWindow(int windowID)
    {
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Volume:");
        //m_volume = GUILayout.HorizontalSlider(m_volume, -80.0f, 20.0f);
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Graphics:");
        //m_qualityLevel = GUILayout.SelectionGrid(m_selGrid, m_selStrings, 1);
        //GUILayout.EndHorizontal();

        //if (GUI.changed)
        //{
        //    SetVolume();
        //    SetQuality();
        //}
    }

    // Called whenever the volume slider is used
    private void SetVolume()
    {
        //GameAudio.SetFloat("volume", m_volume);
    }

    // Called whenever the Quality is changed
    private void SetQuality()
    {
        //QualitySettings.SetQualityLevel(m_qualityLevel);
    }

    public void SetControllerType()
    {
        switch (PlayerMovement.Controller.CurrInput)
        {
            case MouseKeyPlayerController.EInputType.KeyboardAndMouse:
                m_inputType = MouseKeyPlayerController.EInputType.KeyboardAndMouse;
                break;

            case MouseKeyPlayerController.EInputType.XboxController:
                m_inputType = MouseKeyPlayerController.EInputType.XboxController;
                break;

            case MouseKeyPlayerController.EInputType.PS4Controller:
                m_inputType = MouseKeyPlayerController.EInputType.PS4Controller;
                break;
        }
    }
}

#endif