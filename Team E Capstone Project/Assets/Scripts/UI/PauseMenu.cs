// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 08/11/2021

/*
Author: Bryson Bertuzzi
Date Created: 02/11/2021
Comment/Description: Pause Menu UI Controller
ChangeLog:
08/11/2021: SettingsButton() function addded
14/11/2021: Added null checking for components
09/12/2021: Fixed scene loading on exit to main menu
*/

/*
Author: Aviraj Singh Virk
Description: Pause Menu UI Controller
ChangeLog:
10/12/2021: Added comments and refined the script
*/

/*
Author: Seth Grinstead
ChangeLog:
01/19/2022: Moved UI controller input for menu from UIAppear script
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Canvas m_settingsSubmenuCanvas;         // Reference to the Setting Canvas

    public PlayerMovementScript PlayerMovement;     // Reference to player movement script
    public PlayerController PlayerController;
    MouseKeyPlayerController m_controller;          // Reference to player input manager

    public List<GameObject> PauseButtons;           // List of button objects in pause menu

    int m_pauseIndex = 0;                           // Current index of button object list
    float m_cycleTimer = 0.5f;                      // Time between button cycling

    bool m_bCycleIndex = false;                     // Has player cycled recently

    UIAppear m_uiAppear;

    // Called when the Game loads up, Before Start is called
    private void Awake()
    {
        // If SettingsSubmenuCanvas is empty...
        if (m_settingsSubmenuCanvas == null)
        {
            Debug.LogError("Missing Settings Submenu Canvas", this);
        }

        //Get input manager reference from player script
        m_controller = PlayerMovement.Controller;
        PlayerController = PlayerMovement.gameObject.GetComponent<PlayerController>();

        m_uiAppear = GameObject.Find("PR_Player").GetComponent<UIAppear>();

        //Set first selected object
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(PauseButtons[m_pauseIndex]);
    }

    void Update()
    {
        //If settings canvas is not drawn on top
        if (m_settingsSubmenuCanvas.gameObject.activeSelf == false)
        {   
            //If there is input detected
            if (GetVerticalAxis() != 0.0f)
            {
                //If input is positive
                if (GetVerticalAxis() > 0.0f)
                    CycleButtons(-1);
                else
                    CycleButtons(1);
            }
            else
            {
                //Player not holding input, reset cycle timer
                m_bCycleIndex = false;
                m_cycleTimer = 0.5f;
            }

            //If controller cancel button is pressed
            if (Input.GetButtonDown("Cancel"))
            {
                //Close pause menu
                m_uiAppear.HidePauseMenu();
            }
        }

        //Timer for holding input scrolling menu items
        ResetCycleTimer();
    }

    // Exiting the settings menu
    public void ExitSettings()
    {
        PlayerController.SettingsMenuEnabled = false;

        //Close settings menu
        m_settingsSubmenuCanvas.gameObject.SetActive(false);
    }

    // SettingsButton is called when Settings Button is pressed
    public void SettingsButton()
    {
        PlayerController.SettingsMenuEnabled = true;

        // Open Settings Submenu
        m_settingsSubmenuCanvas.gameObject.SetActive(true);
    }

    // ExitToMainMenu is called when Exit to Main Menu Button is pressed
    public void ExitToMainMenu()
    {
        // Use coroutine to load the Scene in the background
        StartCoroutine(LoadAsyncScene());
    }

    // LoadAsyncScene is called when loading the Scene in the background
    IEnumerator LoadAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        AsyncOperation aSyncLoad = SceneManager.LoadSceneAsync("LD_MainMenu");

        // Wait until the asynchronous scene fully loads
        while (!aSyncLoad.isDone)
        {
            yield return null;
        }
    }

    //Function holding menu navigation logic for controller
    void CycleButtons(int dir)
    {
        if (!m_bCycleIndex)
        {
            //Adjust index value
            m_pauseIndex += dir;

            //If pause index is less than zero, cycle to end of list
            if (m_pauseIndex < 0)
                m_pauseIndex = PauseButtons.Count - 1;

            //If index is equal to PauseButtons list count, cycle to zero index
            if (m_pauseIndex == PauseButtons.Count)
                m_pauseIndex = 0;

            //Player has cycled recently
            m_bCycleIndex = true;

            //Set selected game object (button)
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(PauseButtons[m_pauseIndex]);
        }
    }

    float GetVerticalAxis()
    {
        if (Input.GetAxisRaw("KMVertical") != 0.0f)
            return Input.GetAxisRaw("KMVertical");
        else if (Input.GetAxisRaw("XBVerticalDPad") != 0.0f)
            return Input.GetAxisRaw("XBVerticalDPad");

        return 0.0f;
    }

    void ResetCycleTimer()
    {
        if (m_bCycleIndex)
        {
            m_cycleTimer -= Time.fixedDeltaTime;

            if (m_cycleTimer <= 0.0f)
            {
                m_cycleTimer = 0.5f;
                m_bCycleIndex = false;
            }
        }
    }
}