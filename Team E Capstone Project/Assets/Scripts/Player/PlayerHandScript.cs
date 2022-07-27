// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 04/10/2021
Comment/Description: Main Player/Character Controller Script
ChangeLog:
12/10/2021: Seperated PlayerHand from m_playerController
19/10/2021: Added Radio and Radio functions to PlayerHandScript
20/10/2021: Renamed Radio to WalkieTalkie
13/11/2021: Added null checking for components
15/11/2021: Fixed Walkie Talkie functions
24/11/2021: Upgraded Door Key function with new sound cues
09/12/2021: Fixed door making sounds on start
26/01/2022: Add null checking for DoorSound component in TryPickup function
27/01/2022: Diabled rotation on m_objectInHand to fix bug with object snapping
23/03/2022: Added duel lock function for doors that use both keypad and keycard
06/04/2022: Added function to update partner door in duel lock function
06/04/2022: Reduced number of calls for getcomponent of doors 
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Main Player/Character's Hand Script
ChangeLog:
25/10/2021: Added Interaction and Holding Object Logic from Prototype
26/10/2021: Added DropObject function
05/11/2021: Added Example ChangeLog
06/11/2021: Added seperate functionality for door interaction
11/11/2021: Changed every input function to use MouseKeyController for asking for the Input
13/11/2021: Added null checks and improved GetComponent calls
14/11/2021: Added public Camera Transform
15/11/2021: Replaced Raycast with a SphereCast and added Gizmos to show them in Scene View. Updated Player Prefab
04/03/2022: Added comments and refined code
*/

/*
Author: Seth Grinstead
ChangeLog:
08/11/2021: Player now changes sound flag on held objects to assist in AI detection
18/11/2021: Updated how player affects sound flags to fit new audio system
07/04/2022: Removed "HeldByPlayer" bool references for box sounds
*/

/*
Author: Jenson Portelance
ChangeLog:
11/11/2021: Added player audio source component reference as well as a player UIAppear component reference. Removed obsolute Healthkit and Sanitykit usage.
*/

/*
Author: Jacob Kaplan-Hall
ChangeLog
14/11/21: changed script to work with new audio system
*/

using UnityEngine;

public class PlayerHandScript : MonoBehaviour
{
    [Header("Player References")]
    public Transform PlayerCamTransform;            // PlayerCamera's transform

    private PlayerController m_playerController;    // Reference to the PlayerController of Player

    // Object Pickup Handling
    private WeightHandling m_weightHandling;        // Reference to WeightHandling to calculate velocity of objects when they move
    private bool m_isObjectHeld = false;            // Is there an Object held?
    private GameObject m_objectInHand;              // Object in Player Hand
    private Collider m_objectCollider;              // Collider Component of the interactable object
    private int m_objectHoldingSpeed;               // Speed of Held Object
    [SerializeField] float m_holdDistance = 2.5f;                    // Distance objects are held from player

    [Header("Tools")]
    public GameObject WalkieTalkie;                 // Prefab of the Walkie Talkie  
    public GameObject Flashlight;                   // Prefab of the FlashLight
    public GameObject PickupSocket;

    private AudioSource m_playerAudioSource;        // Reference to the player's audio source component
    private UIAppear m_playerUI;                    // Reference to the player's UI appear component

    private Vector3 m_lastAngle = Vector3.zero;     // Stores data about Player Hand's last angle

    float m_throwForce = 10000.0f;                  // Object throwing force
    float m_throwTimer = 0.5f;                      // Object throw timer
    bool m_bIsThrowing = false;                     // Is the player throwing an object at the moment

    int m_playerLayer = 9;
    int m_storedLayer = 0;


    float m_maxPickupDist = 2.8f;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components on the same gameObject and null check them
        {
            if (!(m_playerController = GetComponent<PlayerController>()))
            {
                //Debug.LogError("Missing Player Controller Component", this);
            }

            if (!(m_weightHandling = GetComponent<WeightHandling>()))
            {
                Debug.LogError("Missing Weight Handling Component", this);
            }

            if (!(m_playerUI = m_playerController.GetUIAppear()))
            {
                Debug.LogError("Missing Player Controller Component", this);
            }

            if (!(m_playerAudioSource = m_playerController.GetAudioSource()))
            {
                Debug.LogError("Missing Audio Source Component", this);
            }
        }

        // Initializing variables and Gameobjects
        {
            //WalkieTalkie = GameObject.FindGameObjectWithTag("WalkieTalkie");

            if (WalkieTalkie != null)
            {
                WalkieTalkie.SetActive(false);
            }

            //Flashlight = GameObject.FindGameObjectWithTag("Flashlight");
        }

        //m_Controller = new MouseKeym_playerController();
    }

    // Update is called once per frame
    void Update()
    {
        // Activate the Walkie Talkie when the player switches to it
        //if (m_playerController.playerMovement.m_Controller.SwitchToWalkieTalkie())
        //{
        //    WalkieTalkie.SetActive(!WalkieTalkie.activeSelf);
        //}

        // If m_m_objectInHand exists and has a volume control knob

        //if (m_objectInHand)
        //{
        //    if (m_objectInHand.GetComponent<TransmitterKnob>())
        //    {
        //        m_objectInHand.GetComponent<TransmitterKnob>().Rotate(transform.forward.z - m_lastAngle.z);
        //    }
        //}
        //
        //m_lastAngle = transform.forward;


        if (m_isObjectHeld == true)
		{
            HoldObject();
		}

        if (m_bIsThrowing)
        {
            m_throwTimer -= Time.deltaTime;

            if (m_throwTimer <= 0.0f)
            {
                m_throwTimer = 0.5f;
                m_bIsThrowing = false;
            }
        }
    }

    // OnLeftMouseClick is called when interacting with objects
    public void OnLeftMouseClick(GameObject hitObject)
    {
        if (m_playerController.isUsingItem == true)
        {
            UseItem();
        }

        if (hitObject != null && hitObject.tag != "Player" && hitObject.tag != "Untagged") // Null check
        {
            if (m_bIsThrowing == false)
            {
                // If there is no Object in hand right now
                if (m_isObjectHeld == false)
                {
                    // Try picking up object 
                    TryPickup();
                }
                else
                {
                    // Hold Object
                    HoldObject();
                }
            }
        }
    }

    // Called when the Player releases an object toi throw it
    public void Throw()
    {
        if (m_objectInHand)
        {
            Rigidbody rb = m_objectInHand.GetComponent<Rigidbody>();

            if (rb)
            {
                Vector3 dir = PlayerCamTransform.forward;
                Vector3 Force = m_throwForce * dir;

                rb.AddForce(Force);

                DropObject();
                m_bIsThrowing = true;
            }
        }
    }

    // TryPickup is called when attempting to pickup an object
    void TryPickup()
    {
        // If there is no current object in hand, then only try pickup
        if (m_isObjectHeld == false)
        {
            RaycastHit hitInfo;

            // Cast a ray from the camera to the WorldScreen
            if (Physics.Raycast(PlayerCamTransform.position, PlayerCamTransform.transform.forward, out hitInfo, m_maxPickupDist, 1, QueryTriggerInteraction.Ignore))
            {
                // If the Raycast was successful and hits something
                m_objectInHand = hitInfo.collider.gameObject;
                m_objectCollider = hitInfo.collider;


                // If the player is ready to pickup
                if (m_playerController.isPickup == true)
                {
                    var item = m_objectCollider.GetComponent<GroundItem>();
                    if (item != null)
                    {
                        m_playerUI.ShowDotUI();

                        m_playerController.flashEffect.FlashScreen(0.25f, 0.15f, Color.cyan);

                        // Play this object's pickup sound (which is just the clip in their audio source
                        AudioClip objClip = m_objectInHand.GetComponent<AudioSource>().clip;
                        m_playerAudioSource.clip = objClip;
                        m_playerAudioSource.enabled = true;
                        m_playerAudioSource.Play();

                        if (item.Item.Type == EItemType.Document)
                        {
                            // Call Show document and pass in item desc
                            m_playerUI.ShowDocumentUI(item.Item.Description);

                            // Add the item to the documents inventory
                            if (m_playerController.Documents.IsInventoryFull() == false)
                            {
                                m_playerController.Documents.AddItem(new Item(item.Item), 1);
                            }
                            else
                            {
                                // Add it to the second page instead
                                m_playerController.SecondDocuments.AddItem(new Item(item.Item), 1);
                            }

                        }
                        else if (item.Item.Type == EItemType.AudioRecording)
                        {
                            // Call Show document and pass in item desc
                            m_playerUI.ShowAudioRecordingUI(item.Item.Description, item.Item.AudioClip);

                            // Add the item to the audio tapes inventory
                            if (m_playerController.AudioTapes.IsInventoryFull() == false)
                            {
                                m_playerController.AudioTapes.AddItem(new Item(item.Item), 1);
                            }
                            else
                            {
                                // Add it to the second page instead
                                m_playerController.SecondAudioTapes.AddItem(new Item(item.Item), 1);
                            }

                        }
                        else
                        {
                            // Display test saying what item was picked up
                            m_playerUI.ShowText("Picked up " + item.Item.name, EAlignementType.Middle, 1.0f);

                            // Add the item to the player's inventory
                            if (m_playerController.inventory.IsInventoryFull() == false)
                            {
                                m_playerController.inventory.AddItem(new Item(item.Item), 1);
                            }
                            else
                            {
                                // Add it to the second page instead
                                m_playerController.SecondInventory.AddItem(new Item(item.Item), 1);
                            }

                        }
                        Destroy(m_objectInHand.gameObject);
                    }
                }

                // Object is a keypad
                if (m_objectInHand.GetComponent<KeypadUI>() != null && m_objectInHand.GetComponent<KeypadUI>().IsKeypadActive == true)
                {
                    m_objectInHand.GetComponent<KeypadUI>().ShowKeypadMenu();
                }

                // Check for the Collectibles or Interactables/Pickups
                if (m_objectInHand.GetComponent<TagList>() != null)
                {
                    if (m_objectInHand.GetComponent<TagList>().HasTag("Interactable"))
                    {
                        m_isObjectHeld = true;

                        //calculate the hold distance based on the distance of the object when we picked it up
                        m_holdDistance = Vector3.Distance(m_playerController.GetPlayerMovement().PlayerCamera.transform.position, m_objectInHand.transform.position);


                        m_objectInHand.GetComponent<Rigidbody>().useGravity = true;

                        // Check if its a door, as door has different implementation than other pickuable objects
                        if (m_objectInHand.GetComponent<TagList>().HasTag("Door"))
                        {
                            m_objectInHand.GetComponent<Rigidbody>().freezeRotation = false;
                            if (m_objectInHand.GetComponent<DoorSound>() != null)
                            {
                                m_objectInHand.GetComponent<DoorSound>().IsEnabled = true;

                                if (m_objectInHand.GetComponent<TagList>().HasTag("Trigger"))
                                {
                                    if (m_objectInHand.GetComponent<Door>() != null)
                                    {
                                        if (m_objectInHand.GetComponent<Door>().bIsDoorLocked == true)
                                        {
                                            m_playerUI.ShowText("It's locked...", EAlignementType.Bottom, 1.5f);
                                            m_objectInHand.GetComponent<Door>().PlayLockedDoorSound();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //  m_objectInHand.transform.rotation = Quaternion.identity;
                            m_storedLayer = m_objectInHand.layer;
                            m_objectInHand.layer = m_playerLayer;
                        }

                        //sorry i had to comment this out when i upgraded the audio system
                        // AudioEmitter emit = m_objectInHand.GetComponent<AudioEmitter>();
                        //
                        // if (emit)
                        // {
                        //     emit.SetSoundFlag("Player");
                        // }

                        // Store object's original layer and set to player's layer
                    }
                    else if (m_objectInHand.GetComponent<TagList>().HasTag("Radio"))
                    {
                        m_isObjectHeld = true;
                    }
                }
            }
        }
    }

    // HoldObject is called when holding an object
    void HoldObject()
    {
        m_playerUI.ShowClosedHandUI();

        //m_objectInHand.transform.SetParent(PickupSocket.transform);
        Rigidbody rb = m_objectInHand.GetComponent<Rigidbody>();
        if (rb)
        {
            // Cast a ray from camera to player's hand in the WorldScreen
            Ray playerAim = m_playerController.GetPlayerMovement().PlayerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            // Get Target and Current positions of the held object to calculate direction and velocity
            Vector3 nextPos = m_playerController.GetPlayerMovement().PlayerCamera.transform.position + playerAim.direction * m_holdDistance;

            Vector3 currPos = m_objectInHand.transform.position;

            // Calculate the drag on the object according its mass and gravity
            m_objectHoldingSpeed = m_weightHandling.CalculateObjectWeight(rb);

            // Change the velocity of the object to move it
            rb.velocity = (nextPos - currPos) * m_objectHoldingSpeed;

            // If the object is 5 units away from the Player, drop the Object
            float distance = Vector3.Distance(currPos, nextPos);
            if (distance > 1.1f)
            {
                m_objectInHand.GetComponent<Rigidbody>().velocity = m_objectInHand.GetComponent<Rigidbody>().velocity / 2.0f;
                DropObject();
            }
        }
    }

    // DropObject is called when dropping an object
    public void DropObject()
    {
        m_playerUI.ShowDotUI();
        // Set the variable to false so that HoldObject doesn't get called anymore
        m_isObjectHeld = false;

        Rigidbody rb = m_objectInHand.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.useGravity = true;
            rb.freezeRotation = false;
        }

        // Check if its a door, as door has different implementation than other pickuable objects
        if (m_objectInHand.GetComponent<TagList>().HasTag("Door"))
        {
            if (m_objectInHand.GetComponent<DoorSound>() != null)
            {
                m_objectInHand.GetComponent<DoorSound>().IsEnabled = false;
            }
        }

        // Restore object layer and delete the reference of the held object
        m_objectInHand.layer = m_storedLayer;
        m_objectInHand = null;
    }

    // Returns TRUE if the player has something in hand
    public bool IsObjectHeld()
    {
        return m_isObjectHeld;
    }

    // Called when any item from the inventory is used
    public void UseItem()
    {
        //Ray playerAim = m_playerController.GetPlayerMovement().PlayerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitInfo;

        //if (Physics.Raycast(playerAim, out hitInfo, 3.0f))
        if (Physics.Raycast(PlayerCamTransform.position, PlayerCamTransform.transform.forward, out hitInfo, m_maxPickupDist, 1, QueryTriggerInteraction.Ignore))
        {
            m_objectInHand = hitInfo.collider.gameObject;

            if (m_objectInHand != null)
            {
                // RayCast hit has a door
                if (m_objectInHand.GetComponent<Door>() != null)
                {
                    Door lockDoor = m_objectInHand.GetComponent<Door>();

                    // If the item and door match IDs, Open the door
                    if (lockDoor.doorUnlockID == m_playerController.useItemID)
                    {
                        m_playerController.flashEffect.FlashScreen(0.25f, 0.15f, Color.cyan);
                        if (lockDoor.bIsDualLocked == true)
                        {
                            lockDoor.bIsKeyCardDone = true;
                            lockDoor.UpdatePartnerDoor();
                        }
                        else
                        {
                            lockDoor.OpenDoor();
                        }

                        m_playerController.inventory.RemoveItem(m_playerController.useItem);
                    }
                }
                // RayCast hit an obstacle
                if (m_objectInHand.GetComponent<Obstacle>() != null)
                {
                    // If the item and obstacle match IDs, clear the obstacle
                    if (m_objectInHand.GetComponent<Obstacle>().ObstacleUnlockID == m_playerController.useItemID)
                    {
                        m_playerController.flashEffect.FlashScreen(0.25f, 0.15f, Color.cyan);
                        m_objectInHand.GetComponent<Obstacle>().ClearObstacle();
                        m_playerController.inventory.RemoveItem(m_playerController.useItem);
                    }
                }
            }
        }
        m_playerController.isUsingItem = false;
        m_playerUI.HideItemUseUI();
        m_playerController.SetUseItemProperties(null, -1);
    }

    // Returns the object currently held
    public GameObject GetHeldObject()
    {
        return m_objectInHand;
    }

    // Method when gizmos are called ON on this GameObject
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(PlayerCamTransform.position, PlayerCamTransform.position + PlayerCamTransform.forward * 3.0f);
        Gizmos.DrawWireSphere(PlayerCamTransform.position + PlayerCamTransform.forward * 3.0f, 0.5f);
    }
}
