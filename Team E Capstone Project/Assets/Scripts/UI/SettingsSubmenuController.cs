// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

/*
Author: Bryson Bertuzzi
Date Created: 29/03/2022
Comment/Description: Settings Submenu UI Controller
ChangeLog:
29/03/2022: Added Seth's controller input for setting menu from SettingMenu script
04/04/2022: Added Brightness to ESettingsSubmenu
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsSubmenuController : MonoBehaviour
{
    // Enum with different submenus
    private enum ESettingsSubmenus
    {
        Graphics,
        Audio,
        Controls,
        Brightness
    }

    [SerializeField] private ESettingsSubmenus m_currentSubmenu;      // Stores the current submenu

    public List<GameObject> SettingsButtons;
    List<GameObject> m_dropButtons;

    int m_settingsIndex = 0;
    int m_dropDownIndex = 0;

    float m_cycleTimer = 0.5f;

    bool m_bCycleIndex = false;
    bool m_bInDropDown = false;

    StandaloneInputModule m_module;

    // OnEnable is called when the object becomes enabled and active
    void Awake()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(SettingsButtons[m_settingsIndex]);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components
        m_module = GameObject.Find("EventSystem").GetComponent<StandaloneInputModule>();

        m_dropButtons = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // If Cancel button is pressed
        if (Input.GetButtonDown(m_module.cancelButton))
        {
            Cancel();
        }

        // If vertical input is detected
        if (GetVerticalAxis() != 0.0f)
        {
            // If vertical input is above 0.2f
            if (GetVerticalAxis() > 0.5f)
            {
                // If in a dropdown and dropdown list has items
                if (m_bInDropDown && m_dropButtons.Count > 0)
                {
                    CycleButtons(m_dropButtons, ref m_dropDownIndex, -1);
                }
                else
                {
                    CycleButtons(SettingsButtons, ref m_settingsIndex, -1);
                }
            }
            // If vertical input is less than -0.2f
            else if (GetVerticalAxis() < -0.5f)
            {
                // If in a dropdown and dropdown list has items
                if (m_bInDropDown && m_dropButtons.Count > 0)
                {
                    CycleButtons(m_dropButtons, ref m_dropDownIndex, 1);
                }
                else
                {
                    CycleButtons(SettingsButtons, ref m_settingsIndex, 1);
                }
            }
        }
        // If no vertical input is detected
        else
        {
            // Allow for cycling and reset cycle rate timer
            m_bCycleIndex = false;
            m_cycleTimer = 0.5f;
        }

        // If Submit button is detected
        if (Input.GetButtonDown(m_module.submitButton))
        {
            Select();
        }

        // If the current selected button is a slider
        if (SettingsButtons[m_settingsIndex].GetComponent<Slider>())
        {
            // If there is horizontal input on controller
            if (GetHorizontalAxis() > 0.5f || GetHorizontalAxis() < -0.5f)
            {
                // Modify slider value based on horizontal input
                SettingsButtons[m_settingsIndex].GetComponent<Slider>().value += GetHorizontalAxis() / 20.0f;
            }
        }

        ResetCycleTimer();
    }

    float GetVerticalAxis()
    {
        if (Input.GetAxisRaw("KMVertical") != 0.0f)
            return Input.GetAxisRaw("KMVertical");
        else if (Input.GetAxisRaw("XBVerticalDPad") != 0.0f)
            return Input.GetAxisRaw("XBVerticalDPad");

        return 0.0f;
    }

    float GetHorizontalAxis()
    {
        if (Input.GetAxisRaw("KMHorizontal") != 0.0f)
            return Input.GetAxisRaw("KMHorizontal");
        else if (Input.GetAxisRaw("XBHorizontalDPad") != 0.0f)
            return Input.GetAxisRaw("XBHorizontalDPad");

        return 0.0f;
    }

    void Cancel()
    {
        // If not in a dropdown selection
        if (m_bInDropDown == false)
        {
            // Exit settings menu
            //BackButton();
        }
        else
        {
            // Hide dropdown selection menu
            SettingsButtons[m_settingsIndex].GetComponent<TMPro.TMP_Dropdown>().Hide();

            // Tell program its no longer in a dropdown
            m_bInDropDown = false;

            // Reset dropdown values
            m_dropDownIndex = 0;
            m_dropButtons.Clear();
        }
    }

    void ResetCycleTimer()
    {
        //Timer for holding input scrolling menu items
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

    void CycleButtons(List<GameObject> list, ref int index, int dir)
    {
        if (!m_bCycleIndex)
        {
            //Adjust index by dir
            index += dir;

            //If pause index is less than zero, cycle to end of list
            if (index < 0)
                index = list.Count - 1;

            //If index is equal to list count, cycle to zero index
            if (index == list.Count)
                index = 0;

            //Player has cycled recently
            m_bCycleIndex = true;

            // Set selected object to list item at index
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(list[index]);
        }
    }

    void Select()
    {
        // If settings item is a dropdown selection menu
        if (SettingsButtons[m_settingsIndex].GetComponent<TMPro.TMP_Dropdown>())
        {
            // if not in a dropdown
            if (m_bInDropDown == false)
            {
                // Enter dropdown
                m_bInDropDown = true;

                // If reference collection is successful
                if (m_dropButtons.Count == 0)
                {
                    // If current item is 1st element (Resolution dropdown
                    if (m_settingsIndex == 0 && m_currentSubmenu == ESettingsSubmenus.Graphics)
                    {
                        // Add necessary buttons
                        m_dropButtons.Add(GameObject.Find("Item 0: 960 x 540"));
                        m_dropButtons.Add(GameObject.Find("Item 1: 1280 x 720"));
                        m_dropButtons.Add(GameObject.Find("Item 2: 1600 x 900"));
                        m_dropButtons.Add(GameObject.Find("Item 3: 1920 x 1080"));

                        // Set first selected object 
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(m_dropButtons[m_dropDownIndex]);
                    }
                    // If current item is 5th element (Control type dropdown)
                    else if (m_settingsIndex == 0 && m_currentSubmenu == ESettingsSubmenus.Controls)
                    {
                        // Add necessary buttons
                        m_dropButtons.Add(GameObject.Find("Item 0: Keyboard/Mouse"));
                        m_dropButtons.Add(GameObject.Find("Item 1: Xbox One"));
                        m_dropButtons.Add(GameObject.Find("Item 2: Playstation 4"));

                        // Set first selected object 
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(m_dropButtons[m_dropDownIndex]);
                    }
                    // If current item is 2nd element (Graphics dropdown)
                    else if (m_settingsIndex == 1 && m_currentSubmenu == ESettingsSubmenus.Graphics)
                    {
                        // Add necessary buttons
                        m_dropButtons.Add(GameObject.Find("Item 0: Low"));
                        m_dropButtons.Add(GameObject.Find("Item 1: Medium"));
                        m_dropButtons.Add(GameObject.Find("Item 2: High"));

                        // Set First Selected Object
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(m_dropButtons[m_dropDownIndex]);
                    }
                }
            }
            else
            {
                // If already in a dropdown, select and exit to the top
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(SettingsButtons[m_settingsIndex]);
                m_bInDropDown = false;
                m_dropDownIndex = 0;
                m_dropButtons.Clear();
            }
        }
    }

    private void OnBecameInvisible()
    {
        if (m_dropButtons.Count > 0)
            m_dropButtons.Clear();
    }
}
