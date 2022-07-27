// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 04/10/2021
Comment/Description: Main Player/Character Controller Script
ChangeLog:
20/10/2021: Added Saving and Loading to PlayerController
21/10/2021: Added Inventory Saving to PlayerController
25/10/2021: Cleaned up LoadPlayer() function
02/11/2021: Added Pause Menu functions
03/11/2021: Fixed issue with character freezing when running New Game from Main Menu
03/11/2021: Added ability for character to load character save data on Awake() 
09/11/2021: Added Tab Menu functions
09/11/2021: Added Documents Inventory
11/11/2021: Added Audio Tapes Inventory
12/11/2021: Save and Loading working with all inventories
13/11/2021: Added null checking for components
15/11/2021: Fixed Flashlight and WalkieTalkie Toggle
16/11/2021: Limited Player Loading to Build only and limited OnGUI to Editor only
16/11/2021: Commented out Controller Settings in Update
05/12/2021: Updated CheckIfFileExists function to properly check a specific file
06/12/2021: Removed function for closing game on esc
02/02/2022: Added SetSettings function from SettingsData in Awake
22/03/2022: Added SetDoors function from DoorData in Awake
30/03/2022: Fixed bug with game resuming when in settings menu
15/04/2022: Fix issue with game resuming when pressing tab in settings menu
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Main Player/Character Controller Script
ChangeLog:
05/11/2021: Added Example ChangeLog
06/11/2021: Added Main input to be the MouseKeyController
11/11/2021: Refined document with comments and explaining functions in 1 line
13/11/2021: Added null checks and refined variables names
15/11/2021: Replaced Raycast with SphereCast to have more range. Updated Player Prefab
01/11/2021: Added comments and removed extra code
04/03/2022: Added comments and refined code
*/

/* Author: Jacob Kaplan-Hall
 * ChangeLog:
 * 17/11/21: workaround for interact icon
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    bool DoesLoadInventory = true;

    // Player
    [Header("Player")]
    public PlayerMovementScript playerMovement;     // Reference to PlayerMovementScript
    public PlayerCollisionScript playerCollision;   // Reference to PlayerCollisionScript
    public PlayerHandScript playerHand;             // Reference to PlayerHandScipt
    public InventoryObject inventory;               // Reference to Player's Inventory
    public InventoryObject SecondInventory;
    public InventoryObject Documents;               // Reference to Player's Documents Inventory
    public InventoryObject SecondDocuments;
    public InventoryObject AudioTapes;              // Reference to Player's Audio Tapes Inventory
    public InventoryObject SecondAudioTapes;
    public Transform PlayerCamTransform;            // PlayerCamera's transform

    bool m_isHidden;                                // Bool for hidden stuff

    // Raycasts
    [Header("Raycasts")]
    public bool isPickup = false;                   // Checks if the object can be picked up
    private Ray m_interactionRay;                   // Interaction Ray
    private RaycastHit m_interactionInfo;           // Structure used to get information back from a raycast
    private GameObject m_hitObject;                 // Object Hit with Raycast

    // Use Item Properties
    [Header("Use Item Properties")]
    public bool isUsingItem = false;                // Bool checking if the item is being used
    public int useItemID = -1;                      // Stores Item ID
    public Item useItem = null;                     // Reference to the Item

    // Health & Sanity
    private HealthComponent m_healthComponent;      // Reference to Health Component of the Player
    private SanityComponent m_sanityComponent;      // Reference to Sanity Component of the Player

    // Misc
    [Header("Misc")]
    public FlashEffect flashEffect;                 // Reference to the Screen Effect
    public FlashEffect slashEffect;
    public Canvas deathCanvas;
    private GameObject lastSelectedObject = null;   // Last Selected Object
    private Color lastSelectedColor;                // Last Selected Object Color
    private bool m_tabMenuEnabled = false;          // If the inventory is enabled
    private bool m_pauseMenuEnabled = false;        // If the PauseMenu is enabled
    public bool SettingsMenuEnabled = false;        // If the SettingsMenu is enabled
    public int DebugInt;
    [HideInInspector] public bool IsUsingWalkieTalkie = false;
    [HideInInspector] public bool IsUsingFlashlight = true;

    private UIAppear m_uiAppear;                    // Reference for the UI  
    private AudioSource m_audioSource;              // Reference for the AudioSource

    public GameObject TestObject;                   // Test Object for testing Inventory works

    [SerializeField] float m_interactDistance = 2.5f;
    public float InteractAngleCutoff = 45.0f;

    public Transform MonsterTransform;              // Reference to AI transform

    [HideInInspector] public bool bIsBitten = false;

    // Called when the Game loads up, Before Start is called
    private void Awake()
    {
        // Only run fuctions on Build
        //if (Application.isEditor == false)
        {
            // Initialize Values
            if (DoesLoadInventory && SaveSystem.CheckIfFileExists(Application.persistentDataPath + "/save.data") == true)
            {
                // Loading Player if previous data exists
                LoadPlayer();

                // Initilizing Inventories
                InventoryObject playerInventory = ScriptableObject.CreateInstance<InventoryObject>();
                InventoryObject playerDocuments = ScriptableObject.CreateInstance<InventoryObject>();
                InventoryObject playerAudioTapes = ScriptableObject.CreateInstance<InventoryObject>();

                playerInventory = SaveSystem.LoadInventory("Inventory");
                playerDocuments = SaveSystem.LoadInventory("Documents");
                playerAudioTapes = SaveSystem.LoadInventory("AudioTapes");

                inventory.SetInventory(playerInventory.GetInventory());
                Documents.SetInventory(playerDocuments.GetInventory());
                AudioTapes.SetInventory(playerAudioTapes.GetInventory());

                // Initilizing Doors
                if (SaveSystem.CheckIfFileExists(Application.persistentDataPath + "/Doors.data"))
                {
                    DoorData doorData = SaveSystem.LoadDoors();
                    doorData.SetDoors();
                }
            }
        }

        // Unfreeze the game
        Time.timeScale = 1;

        // Fetch components on the same gameObject and null check them
        {
            if (!(m_healthComponent = GetComponent<HealthComponent>()))
            {
                Debug.LogError("Missing Health Component", this);
            }

            if (!(m_sanityComponent = GetComponent<SanityComponent>()))
            {
                Debug.LogError("Missing Sanity Component", this);
            }

            if (!(m_audioSource = GetComponent<AudioSource>()))
            {
                Debug.LogError("Missing Audio Source Component", this);
            }

            if (!(m_uiAppear = GetComponent<UIAppear>()))
            {
                Debug.LogError("Missing UIAppear Component", this);
            }

            if (inventory == null)
            {
                Debug.LogError("Missing Inventory", this);
            }

            if (Documents == null)
            {
                Debug.LogError("Missing Documents Inventory", this);
            }

            if (AudioTapes == null)
            {
                Debug.LogError("Missing Audio Tapes Inventory", this);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SettingsData data = new SettingsData();
        data.SetSettings();

        if (m_audioSource != null)
        {
            m_audioSource.enabled = false;
        }

        // Cursor setup
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // When the player spawns, give it a flashlight item
        ItemObject flashlight = playerHand.Flashlight.transform.GetChild(0).gameObject.GetComponent<Flashlight>().flashlightItem;
        Item item = new Item(flashlight);
        inventory.AddItem(item, 1);


        // Display the cursor by default
        m_uiAppear.ShowDotUI();

        // playerMovement.Controller.CurrInput = MouseKeyPlayerController.EInputType.KeyboardAndMouse;
    }

    // Update is called once per frame
    void Update()
    {
        // If the Game is not Puased, Keep cursor locked and dont show it
        if (playerMovement.bGameIsPaused == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Highlight Objects which can be interacted with
        HighlightObjects();

        // If the Player's aim is not on the Object, Reset highlighting them
        if (m_hitObject != null && m_hitObject.tag != "Interactable Object")
            ResetHighlightedObject();

        // Check character Input every frame
        CheckCharacterInput();

        // If Hit Object is Enemy...
        if (m_hitObject != null && m_hitObject.tag == "Enemy")
        {
            if (m_sanityComponent.CurrentSanity > 0.0f)
            {
                // Calculate Sanity Damage
                float sanityDamage = 1 * Time.deltaTime;
                m_sanityComponent.LoseSanity(sanityDamage);
            }
            if (m_sanityComponent.CurrentSanity == 0)
            {
                // When the sanity is 0, Health starts to decrease
                float healthDamage = 10 * Time.deltaTime;
                m_healthComponent.TakeDamage(healthDamage);
            }
        }

        if (bIsBitten)
        {
            playerMovement.GetRigidbody().velocity = Vector3.zero;
            Vector3 direction = MonsterTransform.position - PlayerCamTransform.position;
            direction.y += 2.0f;
            Quaternion rot = Quaternion.LookRotation(direction);
            PlayerCamTransform.rotation = Quaternion.Lerp(PlayerCamTransform.rotation, rot, 5.0f * Time.deltaTime);
        }
    }

    // Main Method which checks for the Character Inputs
    public void CheckCharacterInput()
    {
        if (playerMovement.bPlayerIsDead == false)
        {
            // Must always be checked, or else player won't be able to close inventory
            if (playerMovement.Controller.ToggleInventory())
            {
                // If the inventory is not enabled yet, Enable it
                if (m_tabMenuEnabled == false)
                {
                    if (m_pauseMenuEnabled && m_uiAppear.IsSettingsMenuActive() == false)
                    {
                        m_uiAppear.HidePauseMenu();
                        m_pauseMenuEnabled = false;
                    }

                    m_uiAppear.ShowTabMenu();
                    m_tabMenuEnabled = true;
                    Debug.Log("Inventory ON");
                }
                else
                {
                    m_uiAppear.HideTabMenu();
                    m_tabMenuEnabled = false;
                    Debug.Log("Inventory OFF");
                }
            }

            // If the Keys set to Pause the game are pressed
            //if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            if (playerMovement.Controller.QuitGame())
            {
                // Check if the menu is not enabled already
                if (m_pauseMenuEnabled == false)
                {
                    if (m_tabMenuEnabled == true)
                    {
                        m_tabMenuEnabled = false;
                        m_uiAppear.HideTabMenu();
                    }

                    //if (m_uiAppear.)


                    UIAppear ui = GetComponent<UIAppear>();
                    if (ui)
                        ui.ShowPauseMenu();
                    m_pauseMenuEnabled = true;
                    Debug.Log("Pause Menu ON");
                }
                else if (m_pauseMenuEnabled == true && GameObject.Find("UI_PR_SettingsCanvas") == null)
                {
                    UIAppear ui = GetComponent<UIAppear>();
                    if (ui)
                        ui.HidePauseMenu();
                    m_pauseMenuEnabled = false;
                    Debug.Log("Pause Menu OFF");
                }
            }

            // If the game is not currently Paused
            if (playerMovement.bGameIsPaused == false)
            {
                // If Interact is pressed, interact with object
                if (playerMovement.Controller.IsPickingUp())
                {
                    playerHand.OnLeftMouseClick(m_hitObject);

                    if (!playerHand.GetHeldObject())
                    {
                        m_hitObject = null;
                    }
                }

                // If Mouse Left Click is left, Drop Object in Hand
                if (playerMovement.Controller.IsPickingUp() == false)
                {
                    // If the Player is holding an object
                    if (playerHand.IsObjectHeld())
                    {
                        playerHand.DropObject();
                    }
                }

                if (playerMovement.Controller.IsThrowing())
                {
                    playerHand.Throw();
                }
            }
        }

    }

    // HighlightObjects is called when looking at an object
    void HighlightObjects()
    {
        bool Hovering = false;
        // SphereCast out the Player's camera for identifying the Objects in range
        if (PlayerCamTransform.localEulerAngles.x > -InteractAngleCutoff)
        {
            if (Physics.Raycast(PlayerCamTransform.position, PlayerCamTransform.forward, out m_interactionInfo, m_interactDistance, 1, QueryTriggerInteraction.Ignore))
            {
                // If the ray hits something which has a collider
                if (m_interactionInfo.collider.gameObject != null)
                {

                    m_hitObject = m_interactionInfo.collider.gameObject;
                    // If the object has a tag "Interactable Object" or "Movable Door"
                    if (m_hitObject.tag == "Interactable Object")
                    {

                        // Check to see if the object we're picking up is an item for our inventory
                        if (m_hitObject.GetComponent<GroundItem>() != null)
                        {
                            isPickup = true;
                        }

                        // Checks to see if the variable storing the last object held by the player
                        if (lastSelectedObject != null)
                        {
                            ResetObjectColor();
                        }

                        lastSelectedObject = m_hitObject;

                        // Save the material color of the last selected object
                        if (lastSelectedObject.GetComponent<Renderer>() != null)
                        {
                            lastSelectedColor = lastSelectedObject.GetComponent<Renderer>().material.color;
                        }

                        if (isPickup == true || m_hitObject.name == "LightBox" || m_hitObject.name == "ModerateBox" || m_hitObject.name == "HeavyBox")
                        {
                            // Temporary demonstration hack to keep example objects from highlighting
                        }
                        else
                        {
                            if (m_hitObject.GetComponent<Renderer>() != null)
                                m_hitObject.GetComponent<Renderer>().material.color = Color.blue;
                        }

                        // Display Hand Grab UI
                        if (m_uiAppear)
                        {
                            m_uiAppear.ShowHandGrabUI();
                        }
                    }
                }
            }
            else
            {
                if (lastSelectedObject != null)
                {
                    ResetHighlightedObject();
                }
            }
        }
        else
        {
            if (lastSelectedObject != null)
            {
                ResetHighlightedObject();
            }

            m_hitObject = null;
        }
    }

    public void PlayerRevive()
    {
        LoadPlayer();

        InventoryObject playerInventory = ScriptableObject.CreateInstance<InventoryObject>();
        InventoryObject playerDocuments = ScriptableObject.CreateInstance<InventoryObject>();
        InventoryObject playerAudioTapes = ScriptableObject.CreateInstance<InventoryObject>();

        playerInventory = SaveSystem.LoadInventory("Inventory");
        playerDocuments = SaveSystem.LoadInventory("Documents");
        playerAudioTapes = SaveSystem.LoadInventory("AudioTapes");

        inventory.SetInventory(playerInventory.GetInventory());
        Documents.SetInventory(playerDocuments.GetInventory());
        AudioTapes.SetInventory(playerAudioTapes.GetInventory());
    }


    // ResetHighlightedObject is called when resetting a hightlighted object
    void ResetHighlightedObject()
    {
        // If the last selected object stored is not null
        if (lastSelectedObject != null)
        {
            if (isPickup == true)
            {
                isPickup = false;
            }

            ResetObjectColor();

            // Hide Hand Grab UI
            if (m_uiAppear)
            {
                m_uiAppear.HideHandGrabUI();
                m_uiAppear.ShowDotUI();
            }
        }
    }

    // ResetObjectColor is called when resetting the last selected object color
    public void ResetObjectColor()
    {
        if (lastSelectedObject.GetComponent<Renderer>() != null)
        {
            lastSelectedObject.GetComponent<Renderer>().material.color = lastSelectedColor;
            lastSelectedObject = null;
        }
    }

    // Sets the Hidden ability on and off
    public void SetIsHidden(bool hide)
    {
        m_isHidden = hide;
    }

    // Returns the current value of the variable m_IsHidden 
    public bool GetIsHidden()
    {
        return m_isHidden;
    }

    // Setter for the Inventory to be active or not
    public void SetTabMenuEnabled(bool isActive)
    {
        m_tabMenuEnabled = isActive;
    }

    // Set the properties of the Item in use
    public void SetUseItemProperties(Item item, int id)
    {
        useItem = item;
        useItemID = id;
    }

    // Returns the instance PlayerMovementScript currently applied on the Player
    public PlayerMovementScript GetPlayerMovement()
    {
        return playerMovement;
    }

    // Returns the instance PlayerCollisionScript currently applied on the Player
    public PlayerCollisionScript GetPlayerCollision()
    {
        return playerCollision;
    }

    // Returns the instance PlayerHandScript currently applied on the Player
    public PlayerHandScript GetPlayerHand()
    {
        return playerHand;
    }

    // Saves the Player data such as Position, Roation to the disk
    public void SavePlayer()
    {
        PlayerData data = new PlayerData(this);
        SaveSystem.SavePlayer(data);
    }

    // Loads the Player's data from the disk
    public void LoadPlayer()
    {
        PlayerData data = new PlayerData();
        data = SaveSystem.LoadPlayer();

        transform.position = data.Position;
        transform.rotation = data.Rotation;
    }

#if UNITY_EDITOR

    // Called when the UI has to appear which includes PauseMenu, Inevntory
    void OnGUI()
    {
        // Creates the UI Box
        GUI.Box(new Rect(0, 470, 100, 100), "Menu");

        // Makes the Clickable Buttons for Save Player & Inventory
        if (GUI.Button(new Rect(10, 500, 80, 20), "Save Game"))
        {
            SavePlayer();
            SaveSystem.SaveInventory(inventory, "Inventory");
            SaveSystem.SaveInventory(Documents, "Documents");
            SaveSystem.SaveInventory(Documents, "AudioTapes");
        }

        // Makes the Clickable Buttons for Load Player & Inventory
        if (GUI.Button(new Rect(10, 530, 80, 20), "Load Game"))
        {
            LoadPlayer();

            InventoryObject playerInventory = ScriptableObject.CreateInstance<InventoryObject>();
            InventoryObject playerDocuments = ScriptableObject.CreateInstance<InventoryObject>();
            InventoryObject playerAudioTapes = ScriptableObject.CreateInstance<InventoryObject>();

            playerInventory = SaveSystem.LoadInventory("Inventory");
            playerDocuments = SaveSystem.LoadInventory("Documents");
            playerAudioTapes = SaveSystem.LoadInventory("AudioTapes");

            inventory.SetInventory(playerInventory.GetInventory());
            Documents.SetInventory(playerDocuments.GetInventory());
            AudioTapes.SetInventory(playerAudioTapes.GetInventory());
        }
    }

#endif

    public void OnDeath()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerMovement.bGameIsPaused = true;
        playerMovement.bPlayerIsDead = true;
        playerMovement.GetRigidbody().constraints = RigidbodyConstraints.None;
        playerMovement.GetRigidbody().AddForce(Random.onUnitSphere * 20);
        m_uiAppear.HideDotUI();
        m_uiAppear.HideHandGrabUI();
        m_uiAppear.HideClosedHandUI();
        deathCanvas.gameObject.SetActive(true);
        deathCanvas.GetComponentInChildren<DeathEffect>().StartCoroutine(deathCanvas.GetComponentInChildren<DeathEffect>().FadeIn());
    }

    public void EquipFlashlight()
    {
        IsUsingFlashlight = true;
        playerHand.Flashlight.SetActive(!playerHand.Flashlight.activeSelf);
        playerHand.WalkieTalkie.SetActive(false);
    }
    public void EquipWalkieTalkie()
    {
        IsUsingWalkieTalkie = true;
        playerHand.Flashlight.SetActive(false);
        playerHand.WalkieTalkie.SetActive(!playerHand.WalkieTalkie.activeSelf);
    }
    public void UnEquipUtilityItems()
    {
        playerHand.WalkieTalkie.SetActive(false);
        playerHand.Flashlight.SetActive(false);
        IsUsingFlashlight = false;
        IsUsingWalkieTalkie = false;
    }

    // When the game is quitted, empty the inventory conatiner of the Player
    private void OnApplicationQuit()
    {
        inventory.Clear();
        Documents.Clear();
        AudioTapes.Clear();
    }

    // Returns the reference of HealthComponent attached to the Player
    public HealthComponent GetHealthComponent()
    {
        return m_healthComponent;
    }

    // Returns the reference of SanityComponent attached to the Player
    public SanityComponent GetSanityComponent()
    {
        return m_sanityComponent;
    }

    // Sets the toggle value for PauseMenu to be active or not
    public void TogglePauseMenuFalse()
    {
        m_pauseMenuEnabled = false;
    }

    // Returns if the UI is active
    public UIAppear GetUIAppear()
    {
        return m_uiAppear;
    }

    // Retruns the reference to the AudioSource the player is using
    public AudioSource GetAudioSource()
    {
        return m_audioSource;
    }
}