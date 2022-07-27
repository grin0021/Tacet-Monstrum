// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Jenson Portelance
Comment/Description: Script that handles displaying the given inventory in the project
ChangeLog:
20/10/2021: Initial creation of script
04/11/2021: Massive overhaul, optimized how inventory is displayed and added delagation of all mouse interactions with the displayed inventory
08/11/2021: Added functionality for using Document Items
11/11/2021: Added functionality for using Audio Recording Items. Changed code to match coding standards and added more comments.
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Script that handles displaying the given inventory in the project
ChangeLog:
14/11/2021: Added null checking for components and headers
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Script that handles displaying the given inventory in the project
ChangeLog:
10/12/2021: Added Comments for methods and Variables
04/03/2022: Added comments and refined code
*/

/*
Author: Seth Grinstead
ChangeLog:
01/20/2022: Added functions to allow for controller use in inventory menus
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
    [Header("Inventory")]
    public GameObject InventoryPrefab;                  // Prefab for the Inventory item
    public InventoryObject Inventory;                   // Reference to the Inventory

    [Header("Player")]
    public UIAppear PlayerUI;                           // Reference to the Player's UI
    public INVMouse Mouse = new INVMouse();                   // Reference to the Mouse
    [SerializeField]
    private PlayerController m_playerController;        // Reference to the PlayerController
    private MouseKeyPlayerController m_controller;      // Reference to player input management

    [Header("Table Setup")]
    public int HorStart;                                // Horizontal Start Point
    public int VerStart;                                // Vertical Start Point
    public int HorSpaceBetweenItems;                    // Horizontal Space between Items
    public int NumOfColumns;                            // Number of Columns in Inventory
    public int VerSpaceBetweenItems;                    // Horizontal Space between Items

    private bool b_isDragging = false;                  // Bool if the item is being dragged

    List<GameObject> m_buttons;                         // List of inventory slots
    int m_buttonsIndex = 0;                             // Index of inventory slot list
    int m_dragIndex = 0;                                // Index used when dragging inventory items
    bool m_bCycleIndex = false;                         // Has player cycled recently
    float m_cycleTimer = 0.5f;                          // Rate of cycling through buttons

    Dictionary<GameObject, InventorySlot> m_itemsDisplayed = new Dictionary<GameObject, InventorySlot>();

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (InventoryPrefab == null)
            {
                Debug.LogError("Missing Inventory Prefab", this);
            }

            if (Inventory == null)
            {
                Debug.LogError("Missing Inventory Object", this);
            }

        }

        m_playerController = GameObject.Find("PR_Player").GetComponent<PlayerController>();
        PlayerUI = m_playerController.GetUIAppear();

        // Create new game object list
        m_buttons = new List<GameObject>();

        CreateInvSlots();

        // Get reference of player input management from PlayerMovement script
        m_controller = m_playerController.GetPlayerMovement().Controller;

        // If input type is not mouse and keyboard
        if (m_controller.CurrInput != MouseKeyPlayerController.EInputType.KeyboardAndMouse)
        {
            // Set first selected object (button) when inventory is opened
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(m_buttons[m_buttonsIndex]);

            OnEnter(m_buttons[m_buttonsIndex]);
        }
    }

    void OnEnable()
    {
        m_buttonsIndex = 0;

        // If input type is not mouse and keyboard
        if (m_controller != null)
        {
            if (m_controller.CurrInput != MouseKeyPlayerController.EInputType.KeyboardAndMouse)
            {
                // Set first selected object (button) when inventory is opened
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(m_buttons[m_buttonsIndex]);

                OnEnter(m_buttons[m_buttonsIndex]);
            }
        }
    }

    void OnDisable()
    {
        OnExit(m_buttons[m_buttonsIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSlots();

        if (m_controller.CurrInput != MouseKeyPlayerController.EInputType.KeyboardAndMouse)
        {
            // If dragging inventory item
            if (b_isDragging)
            {
                DragInput();
            }
            else
            {
                GeneralInput();

                // Drag item input for controller
                if (Input.GetButtonDown("XBxButton"))
                {
                    // Not dragging already, start drag
                    OnDragStart(m_buttons[m_buttonsIndex]);

                    m_dragIndex = m_buttonsIndex;
                }
            }

            if (Input.GetButtonDown("Submit"))
            {
                Submit();
            }

            if (Input.GetButtonDown("Cancel"))
            {
                Cancel();
            }
        }

        // float/bool limiting button cycle speed on controller
        ResetCycleTimer();
    }

    // Update the slots with new Inventory Items
    public void UpdateSlots()
    {
        // Look through our item dictionary
        foreach (KeyValuePair<GameObject, InventorySlot> slot in m_itemsDisplayed)
        {
            // There is something in the slot
            if (slot.Value.ID >= 0)
            {
                // Set the sprite of the item to be whatever sprite the item has associated with it
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = Inventory.Database.GetItem[slot.Value.Item.Id].UIDisplay;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            }
            // There is nothing in the slot
            else
            {
                // Set the sprite to be null, empty
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            }
        }
    }

    // Script that makes adding delegate events easier, instead of repeating it several times, we just do it once heres
    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger Trigger = obj.GetComponent<EventTrigger>();
        var EventTrigger = new EventTrigger.Entry();
        EventTrigger.eventID = type;

        EventTrigger.callback.AddListener(action);
        Trigger.triggers.Add(EventTrigger);
    }

    // Create new slots according number of Inventory Items
    public void CreateInvSlots()
    {
        // Fill our dictionary
        m_itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        // Loop through each slot in our inventory
        for (int i = 0; i < Inventory.Inventory.Items.Length; i++)
        {
            // Create a visually empty item for each slot
            var Obj = Instantiate(InventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            Obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            m_buttons.Add(Obj);

            // Add triggers for each slot
            AddEvent(Obj, EventTriggerType.PointerEnter, delegate { OnEnter(Obj); });
            AddEvent(Obj, EventTriggerType.PointerExit, delegate { OnExit(Obj); });
            AddEvent(Obj, EventTriggerType.BeginDrag, delegate { OnDragStart(Obj); });
            AddEvent(Obj, EventTriggerType.Drag, delegate { OnDrag(Obj); });
            AddEvent(Obj, EventTriggerType.EndDrag, delegate { OnDragEnd(Obj); });
            AddEvent(Obj, EventTriggerType.PointerClick, delegate { OnPointerClick(Obj); });

            // Ad to our dictionary
            m_itemsDisplayed.Add(Obj, Inventory.Inventory.Items[i]);
        }
    }

    // On Inventory Show
    public void OnEnter(GameObject obj)
    {
        Mouse.HoverObj = obj;
        if (m_itemsDisplayed.ContainsKey(obj))
        {
            // Check if item slot actually has anything in it
            if (m_itemsDisplayed[obj].ID >= 0)
            {
                EItemType Type = Inventory.Database.GetItem[m_itemsDisplayed[obj].ID].Type;
                Item Item = m_itemsDisplayed[obj].Item;

                string ObjectName = Item.Name;
                string ObjectDescription = Item.Text;

                // If item is a document or audio recording, use a different description
                if (Type == EItemType.Document || Type == EItemType.AudioRecording)
                {
                    ObjectDescription = "Click on this item to view its contents.";
                }

                // Fill in the name and description with this object's name an description
                UpdateNameAndDesc(ObjectName, ObjectDescription);

                // Display its name and description
                PlayerUI.ShowNameAndDesc();
            }
            Mouse.HoverItem = m_itemsDisplayed[obj];
        }
    }

    // On inventory Exit
    public void OnExit(GameObject obj)
    {
        PlayerUI.HideNameAndDesc();
        Mouse.HoverObj = null;
        Mouse.HoverItem = null;
    }

    // When the item is being dragged
    public void OnDragStart(GameObject obj)
    {
        b_isDragging = true;

        var mouseObject = new GameObject();

        // Create an object that we will drag around with the mouse as a visual representation of the drag
        var RectTrans = mouseObject.AddComponent<RectTransform>();
        RectTrans.sizeDelta = new Vector2(64, 64);
        mouseObject.transform.SetParent(transform.parent);

        // Check if item slot actually has anything in it
        if (m_itemsDisplayed[obj].ID >= 0)
        {
            // if there is actually an item in the slot, make the object we drag the same as the item in the slot
            var Img = mouseObject.AddComponent<Image>();
            Img.sprite = Inventory.Database.GetItem[m_itemsDisplayed[obj].ID].UIDisplay;
            Img.raycastTarget = false;
        }
        Mouse.Obj = mouseObject;
        Mouse.Item = m_itemsDisplayed[obj];
    }

    // When the item is being dragged
    public void OnDrag(GameObject obj)
    {
        if (Mouse.Obj != null)
        {
            // Update position of the drag object
            Mouse.Obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    // When the item has been dragged
    public void OnDragEnd(GameObject obj)
    {
        b_isDragging = false;

        if (Mouse.HoverObj != null)
        {
            // If the object we dragged is placed onto another object, swap the items
            Inventory.MoveItem(m_itemsDisplayed[obj], m_itemsDisplayed[Mouse.HoverObj]);
        }
        Destroy(Mouse.Obj);
        Mouse.Item = null;
    }

    // Pointer cliked in inventory
    public void OnPointerClick(GameObject obj)
    {
        if (b_isDragging == false)
        {
            PlayerUI.HideNameAndDesc();
            UseItem(obj);
        }
    }

    // Function when player Uses the Item
    public void UseItem(GameObject obj)
    {
        if (m_itemsDisplayed[obj].ID >= 0)
        {
            // Get the passed in object's item type and also set the item inot a variable
            EItemType Type = Inventory.Database.GetItem[m_itemsDisplayed[obj].ID].Type;
            Item Item = m_itemsDisplayed[obj].Item;

            if (Type == EItemType.HealthKit || Type == EItemType.SanityKit)
            {
                if (Type == EItemType.HealthKit)
                {
                    // Apply health healing effect
                    m_playerController.GetHealthComponent().HealPlayer(Item.RestoreValue);
                    m_playerController.flashEffect.FlashScreen(0.25f, 0.15f, Color.green);
                }
                else
                {
                    // Apply sanity healing effect
                    m_playerController.GetSanityComponent().GainSanity(Item.RestoreValue);
                    m_playerController.flashEffect.FlashScreen(0.25f, 0.15f, Color.blue);
                }

                // Close Inventory Screen
                PlayerUI.HideTabMenu();
                m_playerController.SetTabMenuEnabled(false);

                // Check for how much of the item is left 
                if (m_itemsDisplayed[obj].Amount > 1)
                {
                    m_itemsDisplayed[obj].Amount -= 1;
                }
                else
                {
                    // If there is only 1 left of the item, remove it from the inventory
                    Inventory.RemoveItem(Item);
                }

            }
            else if (Type == EItemType.Document)
            {
                // Show the document canvas, and replace the text on it with the text value of this item
                PlayerUI.ShowDocumentUI(Item.Text);
            }
            else if (Type == EItemType.AudioRecording)
            {
                // Show the Audio recording canvas, and replace the text on it with the text value of this item while passing in its audio clip
                PlayerUI.ShowAudioRecordingUI(Item.Text, Item.AudioClip);
            }
            else if (Type == EItemType.WalkieTalkie || Type == EItemType.Flashlight)
            {
                if (Type == EItemType.WalkieTalkie)
                {
                    if (m_playerController.IsUsingWalkieTalkie)
                    {
                        m_playerController.UnEquipUtilityItems();
                        m_playerController.GetUIAppear().ShowText("Un-equiped the Walkie-Talkie", EAlignementType.Middle, 1.5f);
                    }
                    else
                    {
                        m_playerController.UnEquipUtilityItems();
                        m_playerController.EquipWalkieTalkie();
                    }
                }
                else if (Type == EItemType.Flashlight)
                {
                    if (m_playerController.IsUsingFlashlight)
                    {
                        m_playerController.UnEquipUtilityItems();
                        m_playerController.GetUIAppear().ShowText("Un-equiped the Flashlight", EAlignementType.Middle, 1.5f);
                    }
                    else
                    {
                        m_playerController.UnEquipUtilityItems();
                        m_playerController.EquipFlashlight();
                    }
                }
                // Close Inventory Screen
                PlayerUI.HideTabMenu();
                m_playerController.SetTabMenuEnabled(false);
            }
            else
            {
                // Close Inventory Screen
                PlayerUI.HideTabMenu();
                m_playerController.SetTabMenuEnabled(false);

                // Pass item properties to player
                int ID = Inventory.Database.GetItem[m_itemsDisplayed[obj].ID].Id;
                m_playerController.SetUseItemProperties(Item, ID);

                // Display item's icon in the middle of the screen using item use canvas
                PlayerUI.ItemUseCanvas.GetComponentInChildren<Image>().sprite = Inventory.Database.GetItem[m_itemsDisplayed[obj].ID].UIDisplay;
                PlayerUI.ShowItemUseUI();

                // Enable "IsUsingItem"
                m_playerController.isUsingItem = true;
            }

        }
    }

    // Get Position for the current iventory object
    public Vector3 GetPosition(int invSlot)
    {
        // Get the position of the passed in inventory slot
        return new Vector3(HorStart + (HorSpaceBetweenItems * (invSlot % NumOfColumns)), VerStart + (-VerSpaceBetweenItems * (invSlot / NumOfColumns)), 0f);
    }

    // Update items according to name in descending order
    public void UpdateNameAndDesc(string name, string description)
    {
        Transform NameObj = PlayerUI.GetNameAndDescCanvas().transform.GetChild(0).transform.GetChild(1);
        NameObj.GetComponent<Text>().text = name;

        Transform DescriptionObj = PlayerUI.GetNameAndDescCanvas().transform.GetChild(0).transform.GetChild(3);
        DescriptionObj.GetComponent<Text>().text = description;
    }

    // Checks which object was selected
    void SelectionInput(ref int index)
    {
        // If player has not cycled recently
        if (!m_bCycleIndex)
        {
            // If vertical input is detected
            if (Input.GetAxisRaw("XBVerticalDPad") != 0.0f)
            {
                // If vertical input is positive
                if (Input.GetAxisRaw("XBVerticalDPad") > 0.3f)
                {
                    // If index - 4 is within range
                    if (index - 4 > -1)
                    {
                        // End hover behaviour for previous item
                        OnExit(m_buttons[index]);

                        // Adjust index
                        index -= 4;

                        // Begin hover behaviour for next item
                        OnEnter(m_buttons[index]);
                    }
                }

                // If vertical input is negative
                if (Input.GetAxisRaw("XBVerticalDPad") < -0.3f)
                {
                    // If index + 4 is within range
                    if (index + 4 < m_buttons.Count)
                    {
                        // End hover behaviour for previous item
                        OnExit(m_buttons[index]);

                        // Adjust index
                        index += 4;

                        // Begin hover behaviour for next item
                        OnEnter(m_buttons[index]);
                    }
                }

                // Set cycle timer
                m_bCycleIndex = true;
                m_cycleTimer = 0.5f;
            }

            // If horizontal input is detected
            if (Input.GetAxisRaw("XBHorizontalDPad") != 0.0f)
            {
                // If horizontal input is positive
                if (Input.GetAxisRaw("XBHorizontalDPad") > 0.3f)
                {
                    // End hover behaviour on last button
                    OnExit(m_buttons[index]);

                    // Increment index
                    index++;

                    // If index is greater than list range, set to zero
                    if (index == m_buttons.Count)
                        index = 0;

                    // Start hover behaviour on new button
                    OnEnter(m_buttons[index]);
                }
                // If horizontal input is negative
                else if (Input.GetAxisRaw("XBHorizontalDPad") < -0.3f)
                {
                    // End hover behaviour on last button
                    OnExit(m_buttons[index]);

                    // Decrement index
                    index--;

                    // If index is below zero, set to end of list
                    if (index < 0)
                        index = m_buttons.Count - 1;

                    // Start hover behaviour on new button
                    OnEnter(m_buttons[index]);
                }

                // Set cycle timer
                m_bCycleIndex = true;
                m_cycleTimer = 0.5f;
            }

            if (Input.GetAxisRaw("XBVerticalDPad") == 0.0f && Input.GetAxisRaw("XBHorizontalDPad") == 0.0f)
            {
                m_bCycleIndex = false;
                m_cycleTimer = 0.5f;
            }
        }
    }

    // Method that is called to check player input every frame
    void GeneralInput()
    {
        // Handle controller input
        SelectionInput(ref m_buttonsIndex);

        // Set selected game object to button object at index
        if (EventSystem.current.currentSelectedGameObject != m_buttons[m_buttonsIndex])
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(m_buttons[m_buttonsIndex]);
        }
    }

    // Calle when something is dragged across the inventory
    void DragInput()
    {
        // Handle controller input
        SelectionInput(ref m_dragIndex);

        // Get RectTransform components from dragged object and button hovered on
        RectTransform rect1 = Mouse.Obj.GetComponent<RectTransform>();
        RectTransform rect2 = m_buttons[m_dragIndex].GetComponent<RectTransform>();
        
        // Set dragged object position to hovered button position with slight offset
        rect1.position = new Vector3(rect2.position.x + 5.0f, rect2.position.y, rect2.position.z);
    }

    // When the player leaves the object
    void Submit()
    {
        if (b_isDragging)
        {
            OnDragEnd(m_buttons[m_buttonsIndex]);
            m_buttonsIndex = m_dragIndex;
            OnEnter(m_buttons[m_buttonsIndex]);
        }
        else
        {
            UseItem(m_buttons[m_buttonsIndex]);
        }
    }

    // When the player cancels the dragging
    void Cancel()
    {
        if (b_isDragging)
        {
            OnExit(m_buttons[m_dragIndex]);
            OnEnter(m_buttons[m_buttonsIndex]);
            OnDragEnd(m_buttons[m_buttonsIndex]);
        }
        else
        {
            PlayerUI.HideTabMenu();
        }
    }

    // Timer for reset
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

// Class containing minor data
public class INVMouse
{
    public GameObject Obj;
    public InventorySlot Item;
    public InventorySlot HoverItem;
    public GameObject HoverObj;
}