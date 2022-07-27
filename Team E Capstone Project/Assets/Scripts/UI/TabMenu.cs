// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 09/11/2021

/*
Author: Bryson Bertuzzi
Comment/Description: Tab Menu UI Controller
ChangeLog:
11/11/2021: Changed OnDisable() to OnClose() and Uncommented Audio Tapes Canvas code
14/11/2021: Added null checking for components
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Tab Menu UI Controller
ChangeLog:
10/12/2021: Added comments and refined the script
*/

/*
Author: Seth Grinstead
ChangeLog:
01/20/2022: Added ability to cycle through inventory submenus using controller bumpers
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    [SerializeField] private Canvas m_inventorySubmenuCanvas;       // Reference to the Inventory Canvas
    [SerializeField] private Canvas m_documentsSubmenuCanvas;       // Reference to the Document Canvas
    [SerializeField] private Canvas m_audioTapesSubmenuCanvas;      // Reference to the Audio Tapes Canvas

    private GameObject m_invPage1;
    private GameObject m_invPage2;
    private GameObject m_docPage1;
    private GameObject m_docPage2;
    private GameObject m_audPage1;
    private GameObject m_audPage2;

    private int m_pageNum = 1;

    private Text m_nextPageButtonText;

    // Called when the Game loads up, Before Start is called
    private void Awake()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (m_inventorySubmenuCanvas == null)
            {
                Debug.LogError("Missing Inventory Submenu Canvas", this);
            }

            if (m_documentsSubmenuCanvas == null)
            {
                Debug.LogError("Missing Documents Submenu Canvas", this);
            }

            if (m_audioTapesSubmenuCanvas == null)
            {
                Debug.LogError("Missing Audio Tapes Submenu Canvas", this);
            }
        }

        // Gross disgusting hard coded garbage that gets all the inventory objects
        m_invPage1 = m_inventorySubmenuCanvas.transform.GetChild(0).transform.GetChild(1).gameObject;
        m_invPage2 = m_inventorySubmenuCanvas.transform.GetChild(0).transform.GetChild(2).gameObject;

        m_docPage1 = m_documentsSubmenuCanvas.transform.GetChild(0).transform.GetChild(1).gameObject;
        m_docPage2 = m_documentsSubmenuCanvas.transform.GetChild(0).transform.GetChild(2).gameObject;

        m_audPage1 = m_audioTapesSubmenuCanvas.transform.GetChild(0).transform.GetChild(1).gameObject;
        m_audPage2 = m_audioTapesSubmenuCanvas.transform.GetChild(0).transform.GetChild(2).gameObject;

        m_nextPageButtonText = gameObject.transform.GetChild(1).transform.GetChild(1).gameObject.GetComponentInChildren<Text>();
    }

    // OnEnable is called when the object becomes enabled and active
    private void OnEnable()
    {
        // Open Inventory Submenu
        m_inventorySubmenuCanvas.gameObject.SetActive(true);
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

    // OnClose is called when the object becomes disable and inactive
    public void OnClose()
    {
        ResetButtonAndPageText();
        ResetInventoryPages();

        // If Inventory Submenu is open...
        if (m_inventorySubmenuCanvas.gameObject.activeSelf == true && m_inventorySubmenuCanvas != null)
        {
            // Close Inventory Submenu
            m_inventorySubmenuCanvas.gameObject.SetActive(false);
        }

        // If Documents Submenu is open...
        if (m_documentsSubmenuCanvas.gameObject.activeSelf == true && m_documentsSubmenuCanvas != null)
        {
            // Close Documents Submenu
            m_documentsSubmenuCanvas.gameObject.SetActive(false);
        }

        // If Audio Tapes Submenu is open...
        if (m_audioTapesSubmenuCanvas.gameObject.activeSelf == true && m_audioTapesSubmenuCanvas != null)
        {
            // Close Audio Tapes Submenu
            m_audioTapesSubmenuCanvas.gameObject.SetActive(false);
        }
    }

    // InventoryButton is called when Inventory Button is pressed
    public void InventoryButton()
    {
        ResetButtonAndPageText();
        m_docPage2.SetActive(false);
        m_docPage1.SetActive(true);
        m_audPage2.SetActive(false);
        m_audPage1.SetActive(true);

        // Open Inventory Submenu
        m_inventorySubmenuCanvas.gameObject.SetActive(true);

        // If Documents Submenu is open...
        if (m_documentsSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Documents Submenu
            m_documentsSubmenuCanvas.gameObject.SetActive(false);
        }

        // If Audio Tapes Submenu is open...
        if (m_audioTapesSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Audio Tapes Submenu
            m_audioTapesSubmenuCanvas.gameObject.SetActive(false);
        }
    }

    // DocumentsButton is called when Documents Button is pressed
    public void DocumentsButton()
    {
        ResetButtonAndPageText();
        m_invPage2.SetActive(false);
        m_invPage1.SetActive(true);
        m_audPage2.SetActive(false);
        m_audPage1.SetActive(true);

        // Open Documents Submenu
        m_documentsSubmenuCanvas.gameObject.SetActive(true);

        // If Inventory Submenu is open...
        if (m_inventorySubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Inventory Submenu
            m_inventorySubmenuCanvas.gameObject.SetActive(false);
        }

        // If Audio Tapes Submenu is open...
        if (m_audioTapesSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Audio Tapes Submenu
            m_audioTapesSubmenuCanvas.gameObject.SetActive(false);
        }
    }

    // AudioTapesButton is called when Audio Tapes Button is pressed
    public void AudioTapesButton()
    {
        ResetButtonAndPageText();
        m_invPage2.SetActive(false);
        m_invPage1.SetActive(true);
        m_docPage2.SetActive(false);
        m_docPage1.SetActive(true);

        // Open Audio Tapes Submenu
        m_audioTapesSubmenuCanvas.gameObject.SetActive(true);

        // If Inventory Submenu is open...
        if (m_inventorySubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Inventory Submenu
            m_inventorySubmenuCanvas.gameObject.SetActive(false);
        }

        // If Documents Submenu is open...
        if (m_documentsSubmenuCanvas.gameObject.activeSelf == true)
        {
            // Close Documents Submenu
            m_documentsSubmenuCanvas.gameObject.SetActive(false);
        }
    }

    public void NextPageButton()
    {
        // If Inventory Submenu is open...
        if (m_inventorySubmenuCanvas.gameObject.activeSelf == true)
        {
            if (m_pageNum == 1)
            {
                m_invPage1.SetActive(false);
                m_invPage2.SetActive(true);
                m_nextPageButtonText.text = "Previous Pouch";
                m_pageNum += 1;
            }
            else
            {
                m_invPage2.SetActive(false);
                m_invPage1.SetActive(true);
                m_nextPageButtonText.text = "Next Pouch";
                m_pageNum -= 1;
            }
        }
        // If Documents Submenu is open...
        else if (m_documentsSubmenuCanvas.gameObject.activeSelf == true)
        {
            if (m_pageNum == 1)
            {
                m_docPage1.SetActive(false);
                m_docPage2.SetActive(true);
                m_nextPageButtonText.text = "Previous Pouch";
                m_pageNum += 1;
            }
            else
            {
                m_docPage2.SetActive(false);
                m_docPage1.SetActive(true);
                m_nextPageButtonText.text = "Next Pouch";
                m_pageNum -= 1;
            }
        }
        // If Audio Tapes Submenu is open...
        else if (m_audioTapesSubmenuCanvas.gameObject.activeSelf == true)
        {
            if (m_pageNum == 1)
            {
                m_audPage1.SetActive(false);
                m_audPage2.SetActive(true);
                m_nextPageButtonText.text = "Previous Pouch";
                m_pageNum += 1;
            }
            else
            {
                m_audPage2.SetActive(false);
                m_audPage1.SetActive(true);
                m_nextPageButtonText.text = "Next Pouch";
                m_pageNum -= 1;
            }
        }

    }

    void TabLeft()
    {
        if (m_inventorySubmenuCanvas.gameObject.activeSelf == true)
        {
            AudioTapesButton();
        }
        else if (m_audioTapesSubmenuCanvas.gameObject.activeSelf == true)
        {
            DocumentsButton();
        }
        else if (m_documentsSubmenuCanvas.gameObject.activeSelf == true)
        {
            InventoryButton();
        }
    }

    void TabRight()
    {
        if (m_inventorySubmenuCanvas.gameObject.activeSelf == true)
        {
            DocumentsButton();
        }
        else if (m_audioTapesSubmenuCanvas.gameObject.activeSelf == true)
        {
            InventoryButton();
        }
        else if (m_documentsSubmenuCanvas.gameObject.activeSelf == true)
        {
            AudioTapesButton();
        }
    }

    void ResetButtonAndPageText()
    {
        m_pageNum = 1;
        m_nextPageButtonText.text = "Next Pouch";
    }

    void ResetInventoryPages()
    {
        m_invPage2.SetActive(false);
        m_invPage1.SetActive(true);
        m_docPage2.SetActive(false);
        m_docPage1.SetActive(true);
        m_audPage2.SetActive(false);
        m_audPage1.SetActive(true);
    }
}