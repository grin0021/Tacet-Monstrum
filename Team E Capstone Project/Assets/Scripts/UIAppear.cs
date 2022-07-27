// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Jenson Portelance
Comment/Description: Script used to call all UI related canvases
ChangeLog:
20/10/2021: Initial creation of script, added HandGrabUI and InventoryUI
04/11/2021: Item Use UI added
08/11/2021: Document UI added
11/11/2021: AudioRessource UI added. Fixed code to match coding standard and added more comments
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Script used to call all UI related canvases
ChangeLog:
02/11/2021: Added Pause Menu UI functions
09/11/2021: Added Tab Menu UI functions
11/11/2021: Fixed issue with Tab Menu functions
14/11/2021: Added null checking for components
22/11/2021: Fixed issue with Tab and Pause menus overlapping
15/04/2022: Added check for when settings menu is active
*/

/*
Author: Seth Grinstead
Comment/Description: Script used to call all UI related canvases
ChangeLog:
10/12/2021: Added functionality to allow for menu navigation with controllers
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIAppear : MonoBehaviour
{
    [Header("Misc")]
    [SerializeField] private PlayerMovementScript playerMovement;           // Reference to the Player Movement Script
    [Header("Canvas")]
    [SerializeField] private Canvas m_tabMenuCanvas;                        // Reference to the Tab Menu Canvas
    [SerializeField] private Canvas m_pauseMenuCanvas;                      // Reference to the Pause Menu Canvas
    [SerializeField] private Canvas m_documentCanvas;                       // Reference to the Document Canvas
    [SerializeField] private Canvas m_audioRecordingCanvas;                 // Reference to the Audio Recording Canvas
    [SerializeField] private Canvas InventoryCanvas;                        // Reference to the Inventory Canvas
    [SerializeField] private Canvas m_nameDescCanvas;                       // Refernece to the Name and Description Canvas
    [SerializeField] private Canvas m_showTextCanvas;                       // Reference to the show text Canvas
    [SerializeField] private Canvas m_handGrabCanvas;
    [SerializeField] private Canvas m_settingsMenuCanvas;
    [SerializeField] private Canvas m_healthAndSanityCanvas;                // Reference to the health and sanity Canvas
    public Canvas ItemUseCanvas;                                            // Reference to the Item Use Canvas
    public bool IsKeypadShowing = false;
    // Bools for checking if each UI menu is showing
    private bool b_isHandGrabShowing = false;
    private bool b_isHandClosedShowing = false;
    private bool b_isDotShowing = false;
    private bool b_isInventoryShowing = false;
    private bool b_isPauseMenuShowing = false;
    private bool b_isItemUseShowing = false;
    private bool b_isDocumentShowing = false;
    private bool b_isAudioRecordingShowing = false;
    private bool b_isTabMenuShowing = false;
    private bool b_isNameAndDescShowing = false;
    private bool b_isHealthAndSanityShowing = false;

    private void Awake()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (InventoryCanvas == null)
            {
                Debug.LogError("Missing Inventory Canvas", this);
            }
            if (playerMovement == null)
            {
                Debug.LogError("Missing Player Movement Script", this);
            }
            if (m_pauseMenuCanvas == null)
            {
                Debug.LogError("Missing Pause Menu Canvas", this);
            }
            if (m_documentCanvas == null)
            {
                Debug.LogError("Missing Document Canvas", this);
            }
            if (m_audioRecordingCanvas == null)
            {
                Debug.LogError("Missing Audio Recording Canvas", this);
            }
            if (m_tabMenuCanvas == null)
            {
                Debug.LogError("Missing Tab Menu Canvas", this);
            }
            if (m_handGrabCanvas == null)
            {
                Debug.LogError("Missing Hand Grab Canvas", this);
            }
            if (m_healthAndSanityCanvas == null)
            {
                Debug.LogError("Missing Health and Sanity Canvas in Player UIAppear", this);
            }
        }
    }

    private void Update()
    {
        if (b_isDocumentShowing && Input.GetButtonDown("Cancel"))
        {
            HideDocumentUI();
        }

        if (b_isAudioRecordingShowing && Input.GetButtonDown("Cancel"))
        {
            HideAudioRecordingUI();
        }
    }

    // Hand Grab UI ON/OFF methods
    public void ShowHandGrabUI()
    {
        HideDotUI();
        HideClosedHandUI();
        if (b_isHandGrabShowing == false)
        {
            m_handGrabCanvas.transform.GetChild(0).gameObject.SetActive(true);
            b_isHandGrabShowing = true;
        }
    }
    public void HideHandGrabUI()
    {
        if (b_isHandGrabShowing == true)
        {
            m_handGrabCanvas.transform.GetChild(0).gameObject.SetActive(false);
            b_isHandGrabShowing = false;
        }
    }

    public void ShowClosedHandUI()
    {
        HideDotUI();
        HideHandGrabUI();
        if (b_isHandClosedShowing == false)
        {
            m_handGrabCanvas.transform.GetChild(2).gameObject.SetActive(true);
            b_isHandClosedShowing = true;
        }
    }

    public void HideClosedHandUI()
    {
        if (b_isHandClosedShowing == true)
        {
            m_handGrabCanvas.transform.GetChild(2).gameObject.SetActive(false);
            b_isHandClosedShowing = false;
        }
    }

    public void ShowDotUI()
    {
        HideHandGrabUI();
        HideClosedHandUI();
        if (b_isDotShowing == false)
        {
            m_handGrabCanvas.transform.GetChild(1).gameObject.SetActive(true);
            b_isDotShowing = true;
        }
    }

    public void HideDotUI()
    {
        if (b_isDotShowing == true)
        {
            m_handGrabCanvas.transform.GetChild(1).gameObject.SetActive(false);
            b_isDotShowing = false;
        }
    }

    // Item Use UI ON/OFF methods
    public void ShowItemUseUI()
    {
        if (b_isItemUseShowing == false)
        {
            ItemUseCanvas.gameObject.SetActive(true);
            b_isItemUseShowing = true;
        }
    }
    public void HideItemUseUI()
    {
        if (b_isItemUseShowing == true)
        {
            ItemUseCanvas.gameObject.SetActive(false);
            b_isItemUseShowing = false;
        }
    }
    // Inventory UI ON/OFF methods
    public void ShowInventoryUI()
    {
        if (b_isInventoryShowing == false)
        {
            Pause();
            InventoryCanvas.gameObject.SetActive(true);
            b_isInventoryShowing = true;
        }
    }
    public void HideInventoryUI()
    {
        if (b_isInventoryShowing == true)
        {
            Resume();
            InventoryCanvas.gameObject.SetActive(false);
            b_isInventoryShowing = false;
        }
    }
    // Pause Menu UI ON/OFF methods
    public void ShowPauseMenu()
    {
        if (b_isPauseMenuShowing == false && m_pauseMenuCanvas && b_isDocumentShowing == false && b_isAudioRecordingShowing == false && IsKeypadShowing == false)
        {
            RemoveUseItemDisplay();

            if (b_isAudioRecordingShowing)
                HideAudioRecordingUI();
            else if (b_isDocumentShowing)
                HideDocumentUI();

            Pause();
            m_showTextCanvas.GetComponent<ShowTextUI>().ClearShowTextUI();
            m_pauseMenuCanvas.gameObject.SetActive(true);
            b_isPauseMenuShowing = true;

            //if (m_tabMenuCanvas.gameObject.activeSelf == true)
            //{
            //    m_tabMenuCanvas.GetComponent<TabMenu>().OnClose();
            //    m_tabMenuCanvas.gameObject.SetActive(false);
            //    b_isTabMenuShowing = false;
            //}
        }
    }
    public void HidePauseMenu()
    {
        if (b_isPauseMenuShowing == true && m_pauseMenuCanvas)
        {
            Resume();
            m_pauseMenuCanvas.gameObject.SetActive(false);
            b_isPauseMenuShowing = false;
        }
    }
    // Document Menu UI ON/OFF methods
    public void ShowDocumentUI(string docText)
    {
        if (b_isDocumentShowing == false)
        {
            Pause();
            // enable and display document canvas
            m_documentCanvas.gameObject.SetActive(true);
            // Replace text in canvas with this item's description
            m_documentCanvas.GetComponentInChildren<Text>().text = docText;
            b_isDocumentShowing = true;
        }
    }
    public void HideDocumentUI()
    {
        if (b_isDocumentShowing == true)
        {
            if (b_isTabMenuShowing == false)
            {
                Resume();
            }
            m_documentCanvas.gameObject.SetActive(false);
            b_isDocumentShowing = false;
        }
    }
    // Audio Recording Menu UI ON/OFF methods
    public void ShowAudioRecordingUI(string audioText, AudioClip clip)
    {
        if (b_isAudioRecordingShowing == false)
        {
            Pause();
            // enable and display audio recording Canvas
            m_audioRecordingCanvas.gameObject.SetActive(true);
            // Replace text in canvas with this item's description
            m_audioRecordingCanvas.GetComponentInChildren<Text>().text = audioText;
            // Play the associated audio clip
            PlayerController playerController = gameObject.GetComponentInParent<PlayerController>();
            AudioSource playerAudioSource = playerController.GetComponent<AudioSource>();
            AudioClip audioClip = clip;
            playerAudioSource.clip = audioClip;
            playerAudioSource.pitch = 1.0f;
            playerAudioSource.enabled = true;
            playerAudioSource.Play();
            b_isAudioRecordingShowing = true;
        }
    }
    public void HideAudioRecordingUI()
    {
        if (b_isAudioRecordingShowing == true)
        {
            PlayerController playerController = gameObject.GetComponentInParent<PlayerController>();
            if (b_isTabMenuShowing == false)
            {
                Resume();
            }
            m_audioRecordingCanvas.gameObject.SetActive(false);
            playerController.GetComponent<AudioSource>().Stop();
            b_isAudioRecordingShowing = false;
        }
    }
    // Tab Menu UI ON/OFF methods
    public void ShowTabMenu()
    {
        PlayerController playerController = gameObject.GetComponentInParent<PlayerController>();
        if (b_isPauseMenuShowing == false && b_isTabMenuShowing == false && b_isDocumentShowing == false && b_isAudioRecordingShowing == false && IsKeypadShowing == false)
        {
            Pause();
            ShowHealthAndSanity();

            RemoveUseItemDisplay();

            m_showTextCanvas.GetComponent<ShowTextUI>().ClearShowTextUI();
            m_tabMenuCanvas.gameObject.SetActive(true);
            b_isTabMenuShowing = true;
            //if (m_pauseMenuCanvas.gameObject.activeSelf == true)
            //{
            //    m_pauseMenuCanvas.gameObject.SetActive(false);
            //    b_isPauseMenuShowing = false;
            //}
        }
    }
    public void HideTabMenu()
    {
        if (b_isTabMenuShowing == true && b_isDocumentShowing == false && b_isAudioRecordingShowing == false)
        {
            Resume();
            HideHealthAndSanity();
            HideNameAndDesc();
            m_tabMenuCanvas.GetComponent<TabMenu>().OnClose();
            m_tabMenuCanvas.gameObject.SetActive(false);
            b_isTabMenuShowing = false;
        }
    }
    public void ShowNameAndDesc()
    {
        if (b_isNameAndDescShowing == false)
        {
            m_nameDescCanvas.gameObject.SetActive(true);
            b_isNameAndDescShowing = true;
        }
    }
    public void HideNameAndDesc()
    {
        if (b_isNameAndDescShowing == true)
        {
            m_nameDescCanvas.gameObject.SetActive(false);
            b_isNameAndDescShowing = false;
        }
    }

    // Health and Sanity ON/OFF Methods

    public void ShowHealthAndSanity()
    {
        if (b_isHealthAndSanityShowing == false)
        {
            m_healthAndSanityCanvas.gameObject.SetActive(true);
            b_isHealthAndSanityShowing = true;
        }
    }
    public void HideHealthAndSanity()
    {
        if (b_isHealthAndSanityShowing == true)
        {
            m_healthAndSanityCanvas.gameObject.SetActive(false);
            b_isHealthAndSanityShowing = false;
        }
    }

    public void ShowText(string text, EAlignementType alignement, float lengthToShow)
    {
        m_showTextCanvas.GetComponent<ShowTextUI>().ShowText(text, alignement, lengthToShow);
    }
    public Canvas GetNameAndDescCanvas()
    {
        return m_nameDescCanvas;
    }

    public void RemoveUseItemDisplay()
    {
        PlayerController playerController = gameObject.GetComponentInParent<PlayerController>();
        playerController.isUsingItem = false;
        playerController.GetUIAppear().HideItemUseUI();
        playerController.SetUseItemProperties(null, -1);
    }

    // Basic game pause and resume methods
    public void Pause()
    {
        Time.timeScale = 0f;
        playerMovement.bGameIsPaused = true;
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        playerMovement.bGameIsPaused = false;
    }

    public bool IsSettingsMenuActive()
    {
        return m_settingsMenuCanvas.gameObject.activeSelf;
    }
}