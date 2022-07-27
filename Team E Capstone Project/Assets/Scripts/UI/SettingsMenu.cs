// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

/*
Author: Bryson Bertuzzi
Date Created: 04/11/2021
Comment/Description: Settings Menu UI Controller
ChangeLog:
14/11/2021: Added null checking for components
16/11/2021: Added SetControllerType function
29/11/2021: Added Brightness function and fixed bug with duplicating dropdown for resolutions
29/11/2021: Fixed null checking
01/12/2021: Added mutiple audio functions and removed SetVolume function
01/12/2021: Hooked up Settings Canvas to work with no setup required
05/12/2021: Added Saving and Loading for Settings
07/12/2021: Fixed Issues with Saving and Loading
12/01/2022: Reworked Resolution logic to use hard-coded resolutions rather than Unity resoultions
20/01/2022: Fixed naming of player
24/01/2022: Fixed bug for resolution dropdown options not working on build
02/02/2022: Added loading for controller type
02/02/2022: Added limiter for audio sliders
09/02/2022: Added Mouse/Controller Sensitivity functions
09/03/2022: Added tab menu functions for Graphics, Audio, and Controls
29/03/2022: Removed controller input for settings menu
31/03/2022: Fixed brightness settings to work with Volume Profile
04/04/2022: Added Brightness button functions for BrightnessSubmenu
04/04/2022: Added TestAudio button functions for AudioSubmenu
05/04/2022: Replaced TestAudio with TestMusic, TestSFX, and TestEnvironment Buttons
15/04/2022: Removed search for player for SettingsMenuEnabled
 */

/*
Author: Seth Grinstead
ChangeLog:
01/19/2022: Added controller input for settings menu
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;


public class SettingsMenu : MonoBehaviour
{
    private AudioMixer m_gameAudio;
    private TMPro.TMP_Dropdown m_resolutionDropdown;
    private Resolutions[] m_resolutions;
    private Canvas m_settingsSubmenuCanvas;
    public MouseKeyPlayerController m_playerMovement;
    public PlayerController PlayerController;
    private Image[] m_LogoImages;
    public AudioSource[] m_audioSources;

    [SerializeField] private Canvas m_graphicsSubmenuCanvas;      // Reference to the Graphics Canvas
    [SerializeField] private Canvas m_audioSubmenuCanvas;         // Reference to the Audio Canvas
    [SerializeField] private Canvas m_controlsSubmenuCanvas;      // Reference to the Controls Canvas
    [SerializeField] private Canvas m_brightnessSubmenuCanvas;    // Reference to the Brightness Canvas
    [SerializeField] private Image m_controllerLayout;            // Reference to the controller Layout Image
    [SerializeField] private Image m_keyboardLayout;              // Reference to the controller Layout Image
    [SerializeField] private VolumeProfile m_volumeProfile;       // Reference to the global volume profile

    // OnEnable is called when the object becomes enabled and active
    private void OnEnable()
    {
        // Open Graphics Submenu
        m_graphicsSubmenuCanvas.gameObject.SetActive(true);

    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Open Graphics Submenu
        m_graphicsSubmenuCanvas.gameObject.SetActive(true);
        // Open Audio Submenu
        m_audioSubmenuCanvas.gameObject.SetActive(true);
        // Open Controls Submenu
        m_controlsSubmenuCanvas.gameObject.SetActive(true);
        // Open Brightness Submenu
        m_brightnessSubmenuCanvas.gameObject.SetActive(true);

        // Fetch components on the same gameObject and null check them
        {
            if (m_audioSources == null)
            {
                Debug.LogError("Missing Audio Source Component", this);
            }

            if (m_LogoImages == null)
            {
                m_LogoImages = new Image[3];
                m_LogoImages[0] = GameObject.Find("Logo1").GetComponent<Image>();
                m_LogoImages[1] = GameObject.Find("Logo2").GetComponent<Image>();
                m_LogoImages[2] = GameObject.Find("Logo3").GetComponent<Image>();
            }

            if (m_volumeProfile == null)
            {
                Debug.LogError("Missing PP_Lighting Volume Profile", this);
            }

            if (m_graphicsSubmenuCanvas == null)
            {
                Debug.LogError("Missing Graphics Submenu Canvas", this);
            }

            if (m_audioSubmenuCanvas == null)
            {
                Debug.LogError("Missing Audio Submenu Canvas", this);
            }

            if (m_controlsSubmenuCanvas == null)
            {
                Debug.LogError("Missing Controls Submenu Canvas", this);
            }

            var settingsCanvas = gameObject;
            if (settingsCanvas == null)
            {
                Debug.LogError($"Error in {GetType()}: Missing Settings Submenu Canvas");
            }
            else
            {
                m_settingsSubmenuCanvas = settingsCanvas.GetComponent<Canvas>();
                GameObject.Find("GraphicsDropdown").GetComponent<TMPro.TMP_Dropdown>().onValueChanged.AddListener(SetQuality);
                GameObject.Find("ControllerTypeDropdown").GetComponent<TMPro.TMP_Dropdown>().onValueChanged.AddListener(SetControllerType);
                GameObject.Find("FullscreenToggle").GetComponent<Toggle>().onValueChanged.AddListener(SetFullscreen);
                GameObject.Find("BackButton").GetComponent<Button>().onClick.AddListener(BackButton);
                GameObject.Find("BackButton2").GetComponent<Button>().onClick.AddListener(BrightnessButton);
                GameObject.Find("BrightnessButton").GetComponent<Button>().onClick.AddListener(BrightnessButton);
                GameObject.Find("TestMusicButton").GetComponent<Button>().onClick.AddListener(TestMusicButton);
                GameObject.Find("TestSFXButton").GetComponent<Button>().onClick.AddListener(TestSFXButton);
                GameObject.Find("TestEnvironmentButton").GetComponent<Button>().onClick.AddListener(TestEnvironmentButton);
                GameObject.Find("BrightnessSlider").GetComponent<Slider>().onValueChanged.AddListener(SetBrightness);
                GameObject.Find("MouseSensitivitySlider").GetComponent<Slider>().onValueChanged.AddListener(SetMouseSensitivity);
                GameObject.Find("ControllerSensitivitySlider").GetComponent<Slider>().onValueChanged.AddListener(SetControllerSensitivity);
            }

            GameObject gameAudio;
            if (SceneManager.GetActiveScene().name == "LD_MainMenu")
            {
                gameAudio = GameObject.Find("MainMenuCanvas");
            }
            else
            {
                gameAudio = GameObject.Find("PR_Player");
            }

            if (gameObject == null)
            {
                Debug.LogError($"Error in {GetType()}: Missing Audio Mixer");
            }
            else
            {
                m_gameAudio = gameAudio.GetComponent<AudioSource>().outputAudioMixerGroup.audioMixer;
                GameObject.Find("MasterVolumeSlider").GetComponent<Slider>().onValueChanged.AddListener(SetMasterVolume);
                GameObject.Find("MusicVolumeSlider").GetComponent<Slider>().onValueChanged.AddListener(SetMusicVolume);
                GameObject.Find("SFXVolumeSlider").GetComponent<Slider>().onValueChanged.AddListener(SetSFXVolume);
                GameObject.Find("EnvironmentVolumeSlider").GetComponent<Slider>().onValueChanged.AddListener(SetEnvironmentVolume);
            }

            var resDrop = GameObject.Find("ResolutionDropdown");
            if (resDrop == null)
            {
                Debug.LogError($"Error in {GetType()}: Missing Resolution Dropdown");
            }
            else
            {
                m_resolutionDropdown = resDrop.GetComponent<TMPro.TMP_Dropdown>();
                m_resolutionDropdown.onValueChanged.AddListener(SetResolution);
                ResolutionSetup();
            }

            GameObject player = null;
            if (SceneManager.GetActiveScene().name != "LD_MainMenu")
            {
                player = GameObject.Find("PR_Player");
            }

            if (player == null)
            {
                Debug.LogWarning($"Warning in {GetType()}: Missing Player Movement Script, creating one for you");

                m_playerMovement = new MouseKeyPlayerController();
            }
            else
            {
                m_playerMovement = player.GetComponent<PlayerMovementScript>().Controller;
            }
        }

        //   if (Application.isEditor == false)
        {
            // Load Settings if file exists
            if (SaveSystem.CheckIfFileExists(Application.persistentDataPath + "/settings.data") == true)
            {
                SettingsData data = new SettingsData();
                data = SaveSystem.LoadSettings();

                SetMasterVolume(data.Volumes[0]);
                SetMusicVolume(data.Volumes[1]);
                SetSFXVolume(data.Volumes[2]);
                SetEnvironmentVolume(data.Volumes[3]);
                SetFullscreen(data.IsFullscreen);
                SetQuality(data.QualityIndex);
                SetBrightness(data.Brightness);
                SetControllerType(data.ControllerIndex);
                LoadResolution(data.Resolution);
                SetMouseSensitivity(data.Sensitivity[0]);
                SetControllerSensitivity(data.Sensitivity[1]);

                GameObject.Find("MasterVolumeSlider").GetComponent<Slider>().value = data.Volumes[0];
                GameObject.Find("MusicVolumeSlider").GetComponent<Slider>().value = data.Volumes[1];
                GameObject.Find("SFXVolumeSlider").GetComponent<Slider>().value = data.Volumes[2];
                GameObject.Find("EnvironmentVolumeSlider").GetComponent<Slider>().value = data.Volumes[3];
                GameObject.Find("GraphicsDropdown").GetComponent<TMPro.TMP_Dropdown>().value = data.QualityIndex;
                GameObject.Find("FullscreenToggle").GetComponent<Toggle>().isOn = data.IsFullscreen;
                GameObject.Find("BrightnessSlider").GetComponent<Slider>().value = data.Brightness;
                GameObject.Find("ControllerTypeDropdown").GetComponent<TMPro.TMP_Dropdown>().value = data.ControllerIndex;
                GameObject.Find("MouseSensitivitySlider").GetComponent<Slider>().value = data.Sensitivity[0];
                GameObject.Find("ControllerSensitivitySlider").GetComponent<Slider>().value = data.Sensitivity[1];

                ResolutionSetup();
            }
        }
        //else
        //{
        //    ResolutionSetup();
        //}
        // SetBrightness(0);

        // Close Audio Submenu
        m_audioSubmenuCanvas.gameObject.SetActive(false);
        // Close Controls Submenu
        m_controlsSubmenuCanvas.gameObject.SetActive(false);
        // Close Brightness Submenu
        m_brightnessSubmenuCanvas.gameObject.SetActive(false);

        //PlayerController = GameObject.Find("PR_Player").gameObject.GetComponent<PlayerController>();
        
        //PlayerController.SettingsMenuEnabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton4))
        {
            TabLeft();
        }
        else if (Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            TabRight();
        }
    }

    // BackButton is called when Back Button is pressed
    public void BackButton()
    {
        //  if (Application.isEditor == false)
        {
            // Save Settings
            SettingsData data = new SettingsData();
            SaveSystem.SaveSettings(data);
        }

        // If Graphics Submenu is open...
        if (m_graphicsSubmenuCanvas.gameObject.activeSelf == true && m_graphicsSubmenuCanvas != null)
        {
            // Close Graphics Submenu
            m_graphicsSubmenuCanvas.gameObject.SetActive(false);
        }

        // If Audio Submenu is open...
        if (m_audioSubmenuCanvas.gameObject.activeSelf == true && m_audioSubmenuCanvas != null)
        {
            // Close Audio Submenu
            m_audioSubmenuCanvas.gameObject.SetActive(false);
        }

        // If Controls Submenu is open...
        if (m_controlsSubmenuCanvas.gameObject.activeSelf == true && m_controlsSubmenuCanvas != null)
        {
            // Close Controls Submenu
            m_controlsSubmenuCanvas.gameObject.SetActive(false);
        }

        // If Brightness Submenu is open...
        if (m_brightnessSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Brightness Submenu
            m_brightnessSubmenuCanvas.gameObject.SetActive(false);
        }

        //PlayerController.SettingsMenuEnabled = false;
        m_settingsSubmenuCanvas.gameObject.SetActive(false);
    }

    // SetMasterVolume is called when master volume slider is moved
    public void SetMasterVolume(float volume)
    {
        // If volume equals -20dB...
        if (volume == -20)
        {
            // Set volume to -80db (mute)
            volume = -80;
        }

        // Adjust Master Volume
        m_gameAudio.SetFloat("masterVol", volume);
    }

    // SetMusicVolume is called when music volume slider is moved
    public void SetMusicVolume(float volume)
    {
        // If volume equals -20dB...
        if (volume == -20)
        {
            // Set volume to -80db (mute)
            volume = -80;
        }

        // Adjust Music Volume
        m_gameAudio.SetFloat("musicVol", volume);
    }

    // SetSFXVolume is called when sfx volume slider is moved
    public void SetSFXVolume(float volume)
    {
        // If volume equals -20dB...
        if (volume == -20)
        {
            // Set volume to -80db (mute)
            volume = -80;
        }

        // Adjust SFX Volume
        m_gameAudio.SetFloat("sfxVol", volume);
    }

    // SetEnvironmentVolume is called when environment volume slider is moved
    public void SetEnvironmentVolume(float volume)
    {
        // If volume equals -20dB...
        if (volume == -20)
        {
            // Set volume to -80db (mute)
            volume = -80;
        }

        // Adjust Environment Volume
        m_gameAudio.SetFloat("environmentVol", volume);
    }

    // SetQuality is called when object in dropdown is selected
    public void SetQuality(int qualityIndex)
    {
        // Set Quality Level
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // SetFullscreen is called when fullscreen toggle is pressed
    public void SetFullscreen(bool isFullscreen)
    {
        // Toggle Fullscreen
        Screen.fullScreen = isFullscreen;
    }

    // SetResolution is called when object in dropdown is selected
    public void SetResolution(int resolutionIndex)
    {
        Resolutions res = m_resolutions[resolutionIndex];
        Screen.SetResolution(res.Width, res.Height, Screen.fullScreen, res.RefreshRate);
    }

    // ResolutionSetup is called when setting up resolution dropdown
    private void ResolutionSetup()
    {
        // Initialize Resolutions Array
        m_resolutions = new Resolutions[5];

        for (int i = 0; i < m_resolutions.Length; i++)
        {
            // Initialize Resolutions
            m_resolutions[i] = new Resolutions();
        }

        // qHD 960px x 540px @ 60fps 16:9 aspect
        {
            m_resolutions[0].Width = 960;
            m_resolutions[0].Height = 540;
            m_resolutions[0].RefreshRate = 60;
        }

        // HD 1280px x 720px @ 60fps 16:9 aspect
        {
            m_resolutions[1].Width = 1280;
            m_resolutions[1].Height = 720;
            m_resolutions[1].RefreshRate = 60;
        }

        // HD+ 1600px x 900px @ 60fps 16:9 aspect
        {
            m_resolutions[2].Width = 1600;
            m_resolutions[2].Height = 900;
            m_resolutions[2].RefreshRate = 60;
        }

        // FHD 1920px x 1080px @ 60fps 16:9 aspect
        {
            m_resolutions[3].Width = 1920;
            m_resolutions[3].Height = 1080;
            m_resolutions[3].RefreshRate = 60;
        }

        // Clear all options in resoulution dropdown
        m_resolutionDropdown.ClearOptions();

        // Create a list of option strings
        List<string> options = new List<string>();

        // Create an integer called currentResIndex
        int currentResIndex = 0;

        for (int i = 0; i < m_resolutions.Length; i++)
        {
            // Set option string to width x height of resolution
            string option = m_resolutions[i].Width.ToString() + " x " + m_resolutions[i].Height.ToString();

            // Add option to options list
            options.Add(option);

            // If the resolution matchs the current screen resolution...
            if (m_resolutions[i].Width == Screen.width &&
                m_resolutions[i].Height == Screen.height)
            {
                // Set i to currentResIndex;
                currentResIndex = i;
            }
        }

        // Add options to resolution dropdown
        m_resolutionDropdown.AddOptions(options);

        // Set the currently displayed option
        m_resolutionDropdown.value = currentResIndex;

        // Refresh the currently selected option
        m_resolutionDropdown.RefreshShownValue();

        // Clear options list
        options.Clear();
    }

    // SetControllerType is called when object in dropdown is selected
    public void SetControllerType(int controllerIndex)
    {
        StandaloneInputModule mod = GameObject.Find("EventSystem").GetComponent<StandaloneInputModule>();

        switch (controllerIndex)
        {
            case 0:
                m_playerMovement.CurrInput = MouseKeyPlayerController.EInputType.KeyboardAndMouse;
                m_controllerLayout.gameObject.SetActive(false);
                m_keyboardLayout.gameObject.SetActive(true);
                break;

            case 1:
                m_playerMovement.CurrInput = MouseKeyPlayerController.EInputType.XboxController;
                m_controllerLayout.gameObject.SetActive(true);
                m_keyboardLayout.gameObject.SetActive(false);
                break;

            case 2:
                m_playerMovement.CurrInput = MouseKeyPlayerController.EInputType.PS4Controller;
                mod.submitButton = "PSSubmit";
                mod.cancelButton = "PSCancel";
                break;
        }
    }

    // BrightnessButton is called when brightness button is selected
    public void BrightnessButton()
    {
        if (m_brightnessSubmenuCanvas.gameObject.activeSelf == false)
        {
            m_brightnessSubmenuCanvas.gameObject.SetActive(true);
        }
        else
        {
            m_brightnessSubmenuCanvas.gameObject.SetActive(false);
        }
    }


    // SetBrighness is called when brightness slider is moved
    public void SetBrightness(float brightness)
    {
        m_LogoImages[0].color = new Color(m_LogoImages[0].color.r, m_LogoImages[0].color.g, m_LogoImages[0].color.b, brightness - 0.25f);
        m_LogoImages[1].color = new Color(m_LogoImages[0].color.r, m_LogoImages[0].color.g, m_LogoImages[0].color.b, brightness);
        m_LogoImages[2].color = new Color(m_LogoImages[0].color.r, m_LogoImages[0].color.g, m_LogoImages[0].color.b, brightness + 0.25f);

        if (m_volumeProfile.TryGet<LiftGammaGain>(out var gamma))
        {
            gamma.gamma.Override(new Vector4(0, 0, 0, brightness));
        }
    }

    // SetMouseSensitivity is called when mouse sensitivity slider is moved
    public void SetMouseSensitivity(float sensitivity)
    {
        m_playerMovement.SetMouseSensitivity(sensitivity);
    }

    // SetControllerSensitivity is called when controller sensitivity slider is moved
    public void SetControllerSensitivity(float sensitivity)
    {
        m_playerMovement.SetControllerSensitivity(sensitivity);
    }

    // LoadResolution is called when loading resoulution
    private void LoadResolution(int[] resArray)
    {
        Screen.SetResolution(resArray[0], resArray[1], Screen.fullScreen, resArray[2]);
    }

    void TabLeft()
    {
        if (m_graphicsSubmenuCanvas.gameObject.activeSelf == true)
        {
            ControlsButton();
        }
        else if (m_audioSubmenuCanvas.gameObject.activeSelf == true)
        {
            GraphicsButton();
        }
        else if (m_controlsSubmenuCanvas.gameObject.activeSelf == true)
        {
            AudioButton();
        }
    }

    void TabRight()
    {
        if (m_graphicsSubmenuCanvas.gameObject.activeSelf == true)
        {
            AudioButton();
        }
        else if (m_audioSubmenuCanvas.gameObject.activeSelf == true)
        {
            ControlsButton();
        }
        else if (m_controlsSubmenuCanvas.gameObject.activeSelf == true)
        {
            GraphicsButton();
        }
    }

    // GraphicsButton is called when Graphics Button is selected
    public void GraphicsButton()
    {
        // Open Graphics Submenu
        m_graphicsSubmenuCanvas.gameObject.SetActive(true);

        // If Audio Submenu is open...
        if (m_audioSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Audio Submenu
            m_audioSubmenuCanvas.gameObject.SetActive(false);
        }

        // If Controls Submenu is open...
        if (m_controlsSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Controls Tapes Submenu
            m_controlsSubmenuCanvas.gameObject.SetActive(false);
        }
    }

    // AudioButton is called when Audio Button is selected
    public void AudioButton()
    {
        // Open Audio Submenu
        m_audioSubmenuCanvas.gameObject.SetActive(true);

        // If Graphics Submenu is open...
        if (m_graphicsSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Graphics Submenu
            m_graphicsSubmenuCanvas.gameObject.SetActive(false);
        }

        // If Controls Submenu is open...
        if (m_controlsSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Controls Submenu
            m_controlsSubmenuCanvas.gameObject.SetActive(false);
        }
    }

    // ControlsButton is called when Controls Button is selected
    public void ControlsButton()
    {
        // Open Controls Submenu
        m_controlsSubmenuCanvas.gameObject.SetActive(true);
        m_playerMovement.CurrInput = MouseKeyPlayerController.EInputType.KeyboardAndMouse;

        if (m_playerMovement.CurrInput == MouseKeyPlayerController.EInputType.XboxController)
        {
            m_controllerLayout.gameObject.SetActive(true);
            m_keyboardLayout.gameObject.SetActive(false);
        }
        else
        {
            m_controllerLayout.gameObject.SetActive(false);
            m_keyboardLayout.gameObject.SetActive(true);
        }

        // If Graphics Submenu is open...
        if (m_graphicsSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Graphics Submenu
            m_graphicsSubmenuCanvas.gameObject.SetActive(false);
        }

        // If Audio Submenu is open...
        if (m_audioSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Audio Submenu
            m_audioSubmenuCanvas.gameObject.SetActive(false);
        }
    }

    // TestMusicButton is called when TestMusicButton is selected
    public void TestMusicButton()
    {
        // Play Music Clip
        m_audioSources[0].Play();
    }

    // TestSFXButton is called when TestSFXButton is selected
    public void TestSFXButton()
    {
        // Play SFX Clip
        m_audioSources[1].Play();
    }

    // TestEnvironmentButton is called when TestEnvironmentButton is selected
    public void TestEnvironmentButton()
    {
        // Play Environment Clip
        m_audioSources[2].Play();
    }

    //private void PlayAudioClip(AudioClip audioClip, AudioSource audioSource)
    //{
    //    // If both audio source and audio clip are not null...
    //    if (audioSource != null && audioClip != null)
    //    {
    //        // If audio source is not playing anything...
    //        if (audioSource.isPlaying == false)
    //        {
    //            // Play one shot of audio clip
    //            audioSource.Play();
    //        }
    //    }
    //}
}

public class Resolutions
{
    public int Width = 0;
    public int Height = 0;
    public int RefreshRate = 0;
}