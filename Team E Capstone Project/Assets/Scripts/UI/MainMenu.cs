// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 03/11/2021

/*
Author: Bryson Bertuzzi
Comment/Description: Main Menu UI Controller
ChangeLog:
14/11/2021: Added null checking for Sanity Bar
14/11/2021: Added SceneName and comments
15/11/2021: Added Check for when in Unity Editor
15/04/2022: Fix issue with loading scene multiple times
*/

/*
Author: Seth Grinstead
ChangeLog:
01/19/2022: Added controller input for menu
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public GameObject ErrorPanel;                               // Error Panel for loading no save file

    [SerializeField]
    private Canvas m_settingsSubmenuCanvas;                     // Settings Submenu

    public string SceneName;                                    // Name of first scene

    public List<GameObject> MenuButtons;
    int m_menuIndex = 0;

    bool m_bCycleIndex = false;
    float m_cycleTimer = 0.75f;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // If ErrorPanel is empty...
        if (ErrorPanel == null)
        {
            Debug.LogError("Missing Error Panel", this);
        }

        // If SettingsSubmenuCanvas is empty...
        if (m_settingsSubmenuCanvas == null)
        {
            Debug.LogError("Missing Settings Submenu Canvas", this);
        }

        if (SceneName == null)
        {
            Debug.LogError("Missing Scene Name", this);
        }

        //m_uiAppear = GameObject.Find("Player").GetComponent<UIAppear>();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MenuButtons[m_menuIndex]);
    }

    void Update()
    {
        if (m_settingsSubmenuCanvas.gameObject.activeSelf == false)
        {
            //If there is input detected AND has not cycled within arbitrary time (0.5 seconds)
            if (GetVerticalAxis() != 0.0f)
            {
                //If input is positive
                if (GetVerticalAxis() > 0.5f)
                {
                    CycleButtons(-1);
                }
                else if (GetVerticalAxis() < -0.5f)
                {
                    CycleButtons(1);
                }
            }
            //If there is no input detected
            else
            {
                //Player has not cycled, cycle timer reset
                m_bCycleIndex = false;
                m_cycleTimer = 0.5f;
            }

            ResetCycleTimer();
        }
    }

    // NewGameButton is called when New Game Button is pressed
    public void NewGameButton()
    {
        // If Save Files exists...
        if (SaveSystem.CheckIfFileExists(Application.persistentDataPath + "/save.data") == true)
        {
            // Delete all saved files
            {
                File.Delete(Application.persistentDataPath + "/save.data");
                File.Delete(Application.persistentDataPath + "/inventory.data");
                File.Delete(Application.persistentDataPath + "/documents.data");
                File.Delete(Application.persistentDataPath + "/audiotapes.data");
            }
        }

        // Use coroutine to load the Scene in the background
        if (!SceneManager.GetSceneByName(SceneName).isLoaded)
        {
            // Use coroutine to load the Scene in the background
            StartCoroutine(LoadAsyncScene());
        }
    }

    // LoadGameButton is called when Load Game Button is pressed
    public void LoadGameButton()
    {
        if (SaveSystem.CheckIfFileExists(Application.persistentDataPath + "/save.data") == true)
        {
            // If the Scene is already loaded Don't load it again
            if (!SceneManager.GetSceneByName(SceneName).isLoaded)
            {
                // Use coroutine to load the Scene in the background
                StartCoroutine(LoadAsyncScene());
            }
        }
        else
        {
            // Show Error Panel UI
            ErrorPanel.SetActive(true);
        }
    }

    // SettingsButton is called when Settings Button is pressed
    public void SettingsButton()
    {
        // Open Settings Submenu
        m_settingsSubmenuCanvas.gameObject.SetActive(true);
    }

    // ExitButton is called when Exit Button is pressed
    public void ExitButton()
    {

        // If using Unity Editor...
#if UNITY_EDITOR
        // If the Unity Editor is playing...
        if (UnityEditor.EditorApplication.isPlaying == true)
        {
            // Disable the Unity Editor to exit out of the Editor
            UnityEditor.EditorApplication.isPlaying = false;
        }

        Debug.Log("Game Closed");
#endif

        // Quit the player application
        Application.Quit();
    }

    // LoadAsyncScene is called when loading the Scene in the background
    private IEnumerator LoadAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        AsyncOperation aSyncLoad = SceneManager.LoadSceneAsync(SceneName);

        // Wait until the asynchronous scene fully loads
        while (!aSyncLoad.isDone)
        {
            yield return null;
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
                m_cycleTimer = 0.75f;
                m_bCycleIndex = false;
            }
        }
    }

    void CycleButtons(int dir)
    {
        if (!m_bCycleIndex)
        {
            //Adjust index value
            m_menuIndex += dir;

            //If pause index is less than zero, cycle to end of list
            if (m_menuIndex < 0)
                m_menuIndex = MenuButtons.Count - 1;

            //If index is equal to PauseButtons list count, cycle to zero index
            if (m_menuIndex == MenuButtons.Count)
                m_menuIndex = 0;

            //Player has cycled recently
            m_bCycleIndex = true;

            //Set selected game object (button)
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(MenuButtons[m_menuIndex]);
        }
    }
}