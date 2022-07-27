// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 01/11/2021

/*
Author: Bryson Bertuzzi
Date Created: 20/10/2021
Comment/Description: Stores the player's data
ChangeLog:
05/12/2021: Added SettingsData class
07/12/2021: Fixed Issues with Saving and Loading
12/01/2022: Replaced ResolutionIndex logic with Resolution Array logic
02/02/2022: Added ControlllerIndex for SettingsData
02/02/2022: Added function for loading settings in SettingsData
09/02/2022: Added Mouse/Controller Sensitivity in SettingsData
18/03/2022: Fixed issue with playercontroller null (commented out)
22/03/2022: Added DoorData class
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Stores the player's data
ChangeLog:
07/11/2021: Added Comments for explaining the Methods
15/11/2021: Added daste of creation to changelog
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Script holding Player Data
[System.Serializable]
public class PlayerData
{
    public Vector3 Position;    // Position of Player
    public Quaternion Rotation; // Rotation of Player

    // Function that stores default Player data
    public PlayerData()
    {
        Position = Vector3.zero;
        Rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    }

    // Function that stores Player data according to the Playercontroller passed as an argument
    public PlayerData(PlayerController player)
    {
        Position = player.transform.position;
        Rotation = player.transform.rotation;
    }
}

// Script holding Settings Data
[System.Serializable]
public class SettingsData
{
    public bool IsFullscreen;   // Is Screen Fullscreen or Windowed?
    public float Brightness;    // Brightness of Level (Gamma)
    public float[] Volumes;     // Array of Game Volumes
    public int QualityIndex;    // Level of Render Quality
    public int[] Resolution;    // Array of Resolution
    public int ControllerIndex; // Controller Type
    public float[] Sensitivity; // Mouse/Controller Sensitivity

    // Function that stores default Settings data
    public SettingsData()
    {
        // Fullscreen Setting Data
        IsFullscreen = Screen.fullScreen;

        // Brightness Setting Data
        Brightness = 0.0f;

        GameObject player = GameObject.Find("PR_Player");

        //PlayerController playerController = player.GetComponent<PlayerController>();

        // Game Audio Setting Data
        {
            //player = GameObject.Find("PR_Player");
            if (SceneManager.GetActiveScene().name == "LD_MainMenu")
            {
                player = GameObject.Find("MainMenuCanvas");
            }
            else
            {
                player = GameObject.Find("PR_Player");
            }
            AudioMixer gameAudio = player.GetComponent<AudioSource>().outputAudioMixerGroup.audioMixer;

            // Get volume from Audio Mixer
            Volumes = new float[4];
            gameAudio.GetFloat("masterVol", out Volumes[0]);
            gameAudio.GetFloat("musicVol", out Volumes[1]);
            gameAudio.GetFloat("sfxVol", out Volumes[2]);
            gameAudio.GetFloat("environmentVol", out Volumes[3]);
        }

        // Quality Level Setting Data
        QualityIndex = QualitySettings.GetQualityLevel();
        MouseKeyPlayerController controller;
        if (GameObject.Find("SettingsCanvas") != null)
        {
            controller = GameObject.Find("SettingsCanvas").GetComponent<SettingsMenu>().m_playerMovement;
        }
        else
        {
            controller = player.GetComponent<PlayerMovementScript>().Controller;
        }

        // MouseKeyPlayerController controller = GameObject.Find("SettingsCanvas").GetComponent<SettingsMenu>().m_playerMovement;
        switch (controller.CurrInput)
        {
            case MouseKeyPlayerController.EInputType.KeyboardAndMouse:
                ControllerIndex = 0;
                break;
            case MouseKeyPlayerController.EInputType.XboxController:
                ControllerIndex = 1;
                break;
            case MouseKeyPlayerController.EInputType.PS4Controller:
                ControllerIndex = 2;
                break;
        }

        // Resolution Setting Data
        {
            // Initialize Array
            Resolution = new int[3];

            // Set array to screen width, height, and refreshrate 
            {
                Resolution[0] = Screen.width;
                Resolution[1] = Screen.height;
                Resolution[2] = Screen.currentResolution.refreshRate;
            }
        }

        // Sensitivity Setting Data
        {
            Sensitivity = new float[2];
            Sensitivity[0] = controller.GetMouseSensitivity();
            Sensitivity[1] = controller.GetControllerSensitivity();
        }
    }

    // SetSettings is called when loading settings to scene
    public void SetSettings()
    {
        if (SaveSystem.CheckIfFileExists(Application.persistentDataPath + "/settings.data") == true)
        {
            // SettingsData data = new SettingsData();
            SettingsData data = SaveSystem.LoadSettings();

            GameObject player = GameObject.Find("PR_Player");

            // Set Volumes
            {
                AudioMixer gameAudio = player.GetComponent<AudioSource>().outputAudioMixerGroup.audioMixer;
                gameAudio.SetFloat("masterVol", data.Volumes[0]);
                gameAudio.SetFloat("musicVol", data.Volumes[1]);
                gameAudio.SetFloat("sfxVol", data.Volumes[2]);
                gameAudio.SetFloat("environmentVol", data.Volumes[3]);
            }

            // Set Fullscreen
            Screen.fullScreen = data.IsFullscreen;

            // Set Quality Level
            QualitySettings.SetQualityLevel(data.QualityIndex);

            // Set Brightness
            RenderSettings.ambientIntensity = data.Brightness;

            // Set Controller Type
            {
                switch (data.ControllerIndex)
                {
                    case 0:
                        player.GetComponent<PlayerMovementScript>().Controller.CurrInput = MouseKeyPlayerController.EInputType.KeyboardAndMouse;
                        break;

                    case 1:
                        player.GetComponent<PlayerMovementScript>().Controller.CurrInput = MouseKeyPlayerController.EInputType.XboxController;
                        break;
                }
            }

            // Set Mouse/Controller Sensitivity
            {
                player.GetComponent<PlayerMovementScript>().Controller.SetMouseSensitivity(data.Sensitivity[0]);
                player.GetComponent<PlayerMovementScript>().Controller.SetControllerSensitivity(data.Sensitivity[1]);
            }

        }
    }
}

// Script holding Door Data
[System.Serializable]
public class DoorData
{
    public List<Vector3> Position;      // Position of Door
    public List<Quaternion> Rotation;   // Rotation of Door
    public List<bool> b_IsLocked;       // Is Door Locked

    // Function that stores default Door data
    public DoorData()
    {
        Position = new List<Vector3>();
        Rotation = new List<Quaternion>();
        b_IsLocked = new List<bool>();

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Interactable Object");

        foreach (GameObject go in gameObjects)
        {
            TagList temp;
            if (go.TryGetComponent(out temp))
            {
                if (temp.tags.Contains("LockedDoor"))
                {
                    Position.Add(go.transform.position);
                    Rotation.Add(go.transform.rotation);
                    b_IsLocked.Add(go.GetComponent<Door>().bIsDoorLocked);
                }
            }
        }
    }

    public void SetDoors()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Interactable Object");

        int i = 0;
        foreach (GameObject go in gameObjects)
        {
            TagList temp;
            if (go.TryGetComponent(out temp))
            {
                if (temp.tags.Contains("LockedDoor"))
                {
                    go.transform.position = Position[i];
                    go.transform.rotation = Rotation[i];
                    go.GetComponent<Door>().bIsDoorLocked = b_IsLocked[i];
                    ++i;
                }
            }
        }
    }
}