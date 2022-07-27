// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 03/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Keypad UI Controller
ChangeLog:
10/12/2021: Added comments and refined Script
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Keypad UI Controller
ChangeLog:
23/03/2022: Added duel lock function for doors that use both keypad and keycard
06/04/2022: Added function to update partner door in duel lock function
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeypadUI : MonoBehaviour
{
    [SerializeField] private Canvas m_keypadCanvas;                     // How the keypad is displayed
    [SerializeField] private PlayerMovementScript m_playerMovement;     // Simple reference to the player's player movement script
    [SerializeField] private PlayerController m_playerController;       // Simple reference to the player's player controller script
    [SerializeField] private Door m_connectedDoor;                      // The door that this keypad opens
    [SerializeField] private AudioClip m_buttonClip;                    // Audio clip to be used when clicking the buttons on the keypad
    [SerializeField] private AudioClip m_correctBuzzer;                 // Audio that will be used when you get the correct input
    [SerializeField] private AudioClip m_incorrectBuzzer;               // Audio that will be used when you get the wrong input
    [SerializeField] private Sprite m_buzzerImage;
    [SerializeField] private Sprite m_offImage;

    private bool b_isKeypadShowing = false;
    private AudioSource m_keypadAudioSource;                            // This keypad's audio source component
    private int m_totalInput;                                           // The amount of inputs the player has put in
    private float m_timer;                                              // Simple timer to delay the keypad dissapearing

    [Range(0, 9)] [SerializeField] private int m_keypadNum1 = 0;
    [Range(0, 9)] [SerializeField] private int m_keypadNum2 = 0;        // The 4 ints that contain the keypads unlock sequence, they are locked between 0 and 9
    [Range(0, 9)] [SerializeField] private int m_keypadNum3 = 0;
    [Range(0, 9)] [SerializeField] private int m_keypadNum4 = 0;

    private int m_inputNum1;
    private int m_inputNum2;
    private int m_inputNum3;                // The 4 ints that contain the player's inputs onto the keypad
    private int m_inputNum4;


    [HideInInspector] public bool IsKeypadActive = true;                // Bool that tracks if this keypad is still active

    public List<GameObject> Buttons;
    int m_buttonsIndex;
    bool m_bCycleIndex = false;
    float m_cycleTimer = 0.35f;
    bool m_bControllerInput = true;

    // Start is called before the first frame update
    void Start()
    {
        m_playerController = GameObject.Find("PR_Player").GetComponent<PlayerController>();
        m_playerMovement = GameObject.Find("PR_Player").GetComponent<PlayerMovementScript>();

        // Null checks
        if (m_connectedDoor == null)
        {
            Debug.LogError("Keypad has not been assigned a door!");
        }

        if (m_playerController == null)
        {
            Debug.LogError("Keypad needs player controller!");
        }

        if (m_playerMovement == null)
        {
            Debug.LogError("Keypad needs player movement!");
        }

        if (this.GetComponent<AudioSource>() != null)
        {
            m_keypadAudioSource = this.GetComponent<AudioSource>();
        }

        // Setting default values then updating the keypad text to have these default values
        m_timer = 3.0f;
        m_totalInput = 0;
        m_inputNum1 = 0;
        m_inputNum2 = 0;
        m_inputNum3 = 0;
        m_inputNum4 = 0;
        UpdateKeypadText();
    }

    // Update is called once per frame
    void Update()
    {

        if (m_bControllerInput)
        {
            if (!m_bCycleIndex)
            {
                if (GetHorizontalInput() != 0.0f)
                {
                    CycleHorizontal();
                }
                else if (GetVerticalInput() != 0.0f)
                {
                    CycleVertical();
                }

                if (Input.GetButtonDown("Cancel"))
                {
                    HideKeypadMenu();
                }
            }

            ResetCycleTimer();
        }
    }

    public void SubmitInput()
    {
        // Player has used all 4 of their inputs
        if (m_totalInput == 4)
        {
            // If the player's input was correct
            if (IsInputCorrect())
            {
                // Play the correct buzzer sound
                m_keypadAudioSource.clip = m_correctBuzzer;
                m_keypadAudioSource.enabled = true;
                m_keypadAudioSource.volume = 0.5f;
                m_keypadAudioSource.Play();

                // Open the door and set ths keypad to not be active
                m_playerController.flashEffect.FlashScreen(0.25f, 0.15f, Color.cyan);
                IsKeypadActive = false;
                if (m_connectedDoor.bIsDualLocked == true)
                {
                    m_connectedDoor.bIsKeypadDone = true;
                    m_connectedDoor.UpdatePartnerDoor();
                }
                else
                {
                    m_connectedDoor.OpenDoor();
                }
                HideKeypadMenu();
            }
            else
            {
                // Play the incorrect buzzer sound
                m_keypadAudioSource.clip = m_incorrectBuzzer;
                m_keypadAudioSource.enabled = true;
                m_keypadAudioSource.volume = 0.5f;
                m_keypadAudioSource.Play();

                m_keypadCanvas.GetComponentInChildren<Text>().text = "Invalid";
                m_keypadCanvas.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = m_buzzerImage;
                ClearKeypad();
            }
        }
        else
        {
            // Play the incorrect buzzer sound
            m_keypadAudioSource.clip = m_incorrectBuzzer;
            m_keypadAudioSource.enabled = true;
            m_keypadAudioSource.volume = 0.5f;
            m_keypadAudioSource.Play();

            m_keypadCanvas.GetComponentInChildren<Text>().text = "Invalid";
            m_keypadCanvas.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = m_buzzerImage;
            ClearKeypad();
        }
    }

    public void ClearButton()
    {
        ClearKeypad();
        m_keypadCanvas.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = m_offImage;
        UpdateKeypadText();
    }

    // Called when the Player inputs the numbers
    public void InputNum(int input)
    {
        if (m_totalInput <= 3)
        {
            // Play button sound
            m_keypadAudioSource.clip = m_buttonClip;
            m_keypadAudioSource.enabled = true;
            m_keypadAudioSource.volume = 1.0f;
            m_keypadAudioSource.Play();

            switch (m_totalInput)
            {
                case 0:
                    // First Input inputed
                    m_keypadCanvas.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = m_offImage;
                    m_inputNum1 = input;
                    break;
                case 1:
                    // Second Input
                    m_inputNum2 = input;
                    break;
                case 2:
                    // Third Input
                    m_inputNum3 = input;
                    break;
                case 3:
                    // Fourth Input
                    m_inputNum4 = input;
                    break;
            }

            m_totalInput += 1;
            UpdateKeypadText();
        }
    }

    // Checks if the numbers entered were correct
    public bool IsInputCorrect()
    {
        // Convert all of the ints to a simple string to more easily compare them
        string KeypadString = m_keypadNum1.ToString() + m_keypadNum2.ToString() + m_keypadNum3.ToString() + m_keypadNum4.ToString();
        string InputString = m_inputNum1.ToString() + m_inputNum2.ToString() + m_inputNum3.ToString() + m_inputNum4.ToString();

        if (InputString == KeypadString)
        {
            // Input was correct
            return true;
        }
        // Input was incorrect
        return false;
    }

    // Updating the keypad
    public void UpdateKeypadText()
    {
        // Updates the keypad canvas text to display the input of the player
        m_keypadCanvas.GetComponentInChildren<Text>().text = m_inputNum1.ToString() + m_inputNum2.ToString() + m_inputNum3.ToString() + m_inputNum4.ToString();
    }

    // Showing the Text on screen
    public void ShowKeypadMenu()
    {
        if (b_isKeypadShowing == false)
        {
            m_playerController.GetUIAppear().RemoveUseItemDisplay();
            Pause();
            m_keypadCanvas.gameObject.SetActive(true);
            m_playerController.GetUIAppear().IsKeypadShowing = true;
            b_isKeypadShowing = true;

            if (m_playerMovement.Controller.CurrInput == MouseKeyPlayerController.EInputType.XboxController)
            {
                m_bControllerInput = true;
                m_buttonsIndex = 0;
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(Buttons[m_buttonsIndex]);
            }
        }
    }

    // Hiding the Keypad
    public void HideKeypadMenu()
    {
        if (b_isKeypadShowing == true)
        {
            ClearKeypad();
            UpdateKeypadText();
            Resume();
            m_keypadCanvas.gameObject.SetActive(false);
            m_playerController.GetUIAppear().IsKeypadShowing = false;
            b_isKeypadShowing = false;
        }
    }

    // Basic game pause and resume methods
    public void Pause()
    {
        Time.timeScale = 0f;
        m_playerMovement.bGameIsPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        m_playerMovement.bGameIsPaused = false;
    }

    // Resets the kepad
    public void ClearKeypad()
    {
        // Resets all values to default when this is called
        m_timer = 3.0f;
        m_totalInput = 0;
        m_inputNum1 = 0;
        m_inputNum2 = 0;
        m_inputNum3 = 0;
        m_inputNum4 = 0;
    }

    float GetHorizontalInput()
    {
        if (Input.GetAxis("XBHorizontalDPad") != 0.0f)
            return Input.GetAxis("XBHorizontalDPad");
        else if (Input.GetAxis("Horizontal") != 0.0f)
            return Input.GetAxis("Horizontal");

        return 0.0f;
    }

    float GetVerticalInput()
    {
        if (Input.GetAxis("XBVerticalDPad") != 0.0f)
            return Input.GetAxis("XBVerticalDPad");
        else if (Input.GetAxis("Vertical") != 0.0f)
            return Input.GetAxis("Vertical");

        return 0.0f;
    }

    void CycleHorizontal()
    {
        if (GetHorizontalInput() > 0.0f)
        {
            m_buttonsIndex++;

            if (m_buttonsIndex == Buttons.Count)
            {
                m_buttonsIndex = 0;
            }
        }
        else if (GetHorizontalInput() < 0.0f)
        {
            m_buttonsIndex--;

            if (m_buttonsIndex < 0.0f)
            {
                m_buttonsIndex = Buttons.Count - 1;
            }
        }

        m_bCycleIndex = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(Buttons[m_buttonsIndex]);
    }

    void CycleVertical()
    {
        if (GetVerticalInput() > 0.0f)
        {
            m_buttonsIndex -= 3;

            if (m_buttonsIndex < 0.0f)
            {
                m_buttonsIndex += Buttons.Count;
            }
        }
        else if (GetVerticalInput() < 0.0f)
        {
            m_buttonsIndex += 3;


            if (m_buttonsIndex >= Buttons.Count)
            {
                m_buttonsIndex -= Buttons.Count;
            }
        }

        m_bCycleIndex = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(Buttons[m_buttonsIndex]);
    }

    void ResetCycleTimer()
    {
        if (m_bCycleIndex)
        {
            m_cycleTimer -= Time.fixedDeltaTime;

            if (m_cycleTimer <= 0.0f)
            {
                m_bCycleIndex = false;
                m_cycleTimer = 0.35f;
            }
        }
    }
}
