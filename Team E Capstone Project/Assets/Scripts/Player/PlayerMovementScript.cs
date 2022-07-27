// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 04/10/2021
Comment/Description: Main Player/Character Movement Script
ChangeLog:
10/10/2021: Seperated PlayerMovement from PlayerController
13/11/2021: Added null checking for components
19/11/2021: Added getters for Crouching and Crawling
30/03/2022: Added jumping state to jumping
11/04/2022: Removed landing sound in jump
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Main Player/Character Movement script for Tacet Monstrum
ChangeLog:
05/11/2021: Added Enum for different Player States
06/11/2021: Added Commented code to integrate with Player States
07/11/2021: Replaced existing code with new Player states
13/11/2021: Added null checks and refined variables names
04/03/2022: Added comments and refined code and updated DCD
*/

/*
Author: Seth Grinstead
ChangeLog:
15/11/2021: Added leaning functionality
26/01/2022: Moved look sensitivity to MouseKeyPlayerController
*/

/*
Author: Seth Grinstead
ChangeLog:
16/11/2021: Changed movement from CharacterController to RigidBody
18/11/2021: Added Lean functionality
20/11/2021: Added Crouch and Crawl functionality that works with rigidbody
24/11/2021: Made player movement state accessible outside of script
31/01/2022: Added physics raycasts to lean function to prevent player from leaning through objects
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls and governs the Player's Movement
public class PlayerMovementScript : MonoBehaviour
{
    // Enum with different Player States
    public enum EPlayerState
    {
        Idle,
        Walking,
        Sprinting,
        Crawling,
        Crouching,
        Jumping,
    }

    EPlayerState m_currState; // Stores the current state of the Player

    private PlayerController m_playerController; // Reference to the PlayerController

    // Controllers
    public MouseKeyPlayerController Controller { get; set; }          // Player Input Controls

    // Player
    [Header("Player")]
    public float Speed = 10.0f;                   // Movement Speed
    public float JumpForce = 250.0f;              // Force applied for jump
    public bool bIsGrounded = true;               // Is Player Grounded?

    // variables to control if the player is crouching/crawling
    private bool m_bisCrouched = false;
    private bool m_bisCrawling = false;

    // Speed multipliers
    private float m_curntMvntMult = 0.0f;           // Current multipier being applied
    private float m_walkMult = 1.0f;                // Walking multiplier
    private float m_sprintMult = 1.5f;              // Sprinting multipier
    private float m_crouchMult = 0.6f;              // Crouching multipier
    private float m_crawlMult = 0.3f;               // Crawling multipier

    // Camera
    [Header("Camera")]
    public Camera PlayerCamera;                   // First Person Camera
    public float CameraVerticalAngle = 0.0f;      // Vertical Angle of Camera
    public float CameraHorizontalAngle = 0.0f;    // Vertical Angle of Camera

    // Camera Lean
    float m_leanSpeed = 100.0f;                   // Rate of rotation for lean effect
    float m_camAngle = 0.0f;                      // Current angle of lean effect
    float m_maxCamAngle = 10.0f;                  // Max angle of lean effect
    float m_maxLeanPos = 0.5f;                    // Max position offset of lean effect
    float m_currCamPos = 0.0f;                    // Current position offset of lean effect
    bool m_bIsLeaningLeft = false;                // Is player leaning left?
    bool m_bIsLeaningRight = false;               // Is player leaning right?
    Vector3 m_camLeanPos;                         // Reference to camera position to apply lean offsets

    [Header("Misc")]
    public bool bGameIsPaused = false;            // Is the game paused
    public bool bPlayerIsDead = false;            // Is the player dead. To restart or take over controls
    private StaminaComponent m_staminaComponent;  // Reference to Stamina Component of the Player
    Rigidbody m_rigidBody;                        // Reference to player's RigidBody
    float m_defaultHeight;                        // Player's default height
    float m_crouchHeight;                         // Player's crouch height
    float m_crawlHeight;                          // Player's crawl height
    bool m_bHasCrawled = false;                   // Has the player in crawling position
    bool m_bStandPriority = false;                // What is the state of player in terms of stand/crouch/crawl

    //Camera bob effect for movement
    float m_bobTimer = 0.0f;                      // Time between 2 bobs
    float m_camDefaultY = 0.0f;                   // Default Y position of camera
    float m_bobSpeed = 7.5f;                      // Speed of bobbing

    // Crawling Misc
    bool m_bIsSprinting = false;                  // Is the player sprinting
    float m_crawlTimer = 0.5f;                    // Crawling timer
    float m_moveReduction = 0.7075f;              // Speed slow down constant

    // Called before Start when the the Scene loads up
    void Awake()
    {
        if (Controller == null)
        {
            Controller = new MouseKeyPlayerController();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components on the same gameObject
        {
            if (PlayerCamera == null)
            {
                PlayerCamera = Camera.main;
            }

            if (!(m_playerController = GetComponent<PlayerController>()))
            {
                Debug.LogError("Missing Player Controller Component", this);
            }
        }

        m_curntMvntMult = m_walkMult;

        m_rigidBody = GetComponent<Rigidbody>();

        m_staminaComponent = gameObject.GetComponent<StaminaComponent>();

        m_camDefaultY = PlayerCamera.transform.localPosition.y;

        m_defaultHeight = GetComponent<CapsuleCollider>().height;
        m_crouchHeight = m_defaultHeight / 2;
        m_crawlHeight = m_crouchHeight / 4;
    }

    // Update is called once per frame
    void Update()
    {
        // Checks and handles Player's Input and Movement
        HandleCharacterMovement();
    }

    // Function called which handles the Player's movement
    public void HandleCharacterMovement()
    {
        if (bPlayerIsDead == false && !m_playerController.bIsBitten)
        {
            if (bGameIsPaused == false)
            {
                // Character Movement
                MoveCharacter();

                // Character Rotation
                CameraRotation();

                // Character Jump and Gravity
                JumpAndGravity();

                if (Controller.CurrInput == MouseKeyPlayerController.EInputType.KeyboardAndMouse)
                {
                    // If crouch toggle input is detected
                    if (Controller.ToggleCrouch())
                    {
                        m_bisCrouched = !m_bisCrouched;

                        if (m_bisCrawling)
                            m_bisCrawling = false;

                        Crouch(m_bisCrouched);
                    }

                    // If crawl toggle input is detected
                    if (Controller.ToggleCrawl())
                    {
                        m_bisCrawling = !m_bisCrawling;

                        if (m_bisCrouched)
                            m_bisCrouched = false;

                        Crawl(m_bisCrawling);
                    }
                }
                else
                {
                    if (Controller.ToggleCrouch())
                    {
                        m_bisCrouched = !m_bisCrouched;

                        if (m_bisCrawling && !m_bStandPriority)
                            m_bisCrawling = false;

                        Crouch(m_bisCrouched);
                    }
                    else if (Controller.ToggleCrawl() && !m_bHasCrawled)
                    {
                        m_crawlTimer -= Time.deltaTime;

                        if (m_crawlTimer <= 0.0f)
                        {
                            m_bHasCrawled = true;

                            m_crawlTimer = 0.5f;

                            m_bisCrawling = !m_bisCrawling;

                            if (m_bisCrouched)
                                m_bisCrouched = false;

                            Crawl(m_bisCrawling);
                        }
                    }

                    if (Input.GetKeyUp(KeyCode.JoystickButton1))
                    {
                        if (m_bisCrawling && m_bisCrouched)
                        {
                            m_bisCrawling = false;
                            m_bStandPriority = false;
                        }

                        m_crawlTimer = 0.5f;
                        m_bHasCrawled = false;
                    }

                    // If playing on controller, toggle sprint
                    if (m_staminaComponent.GetCurrentStamina() > 0)
                    {
                        if (Controller.IsSprinting())
                        {
                            m_bIsSprinting = !m_bIsSprinting;
                        }
                    }

                }
            }
        }

    }

    // MoveCharacter is called when moving character
    public void MoveCharacter()
    {
        ChangeCurrentSpeedMultipier();

        // Get input direction and translate it to world space
        Vector3 moveDir = Controller.GetMoveInput();

        // Reduce input values if moving diagonally
        if (moveDir.x != 0.0f && moveDir.z != 0.0f)
        {
            if (moveDir.x > m_moveReduction)
                moveDir.x = m_moveReduction;
            else if (moveDir.x < -m_moveReduction)
                moveDir.x = -m_moveReduction;

            if (moveDir.z > m_moveReduction)
                moveDir.z = m_moveReduction;
            else if (moveDir.z < -m_moveReduction)
                moveDir.z = -m_moveReduction;
        }

        moveDir = m_playerController.transform.TransformDirection(moveDir);

        Vector3 camPos = PlayerCamera.transform.localPosition;

        // If player is moving
        if (moveDir != Vector3.zero)
        {
            if (Controller.GetMoveInput().x != 0.0f)
                m_bobTimer += Time.deltaTime * m_bobSpeed * Controller.GetMoveInput().x;
            else if (Controller.GetMoveInput().z != 0.0f)
                m_bobTimer += Time.deltaTime * m_bobSpeed * Controller.GetMoveInput().z;

            PlayerCamera.transform.localPosition = new Vector3(camPos.x, m_camDefaultY + (Mathf.Sin(m_bobTimer) / 10), camPos.z);
        }
        else
        {
            m_bobTimer = 0.0f;
            PlayerCamera.transform.localPosition = new Vector3(camPos.x, camPos.y, camPos.z);//Mathf.Lerp(camPos.y, m_camDefaultY, Time.deltaTime * m_bobSpeed), camPos.z);
        }

        // Set velocity on rigid body to move player
        m_rigidBody.velocity = new Vector3(moveDir.x * Speed * m_curntMvntMult, m_rigidBody.velocity.y, moveDir.z * Speed * m_curntMvntMult);
    }

    // CameraRotation is called when rotating character
    public void CameraRotation()
    {
        // Horizontal character rotation
        {
            // Rotate the transform with the input speed around its local Y axis
            m_playerController.transform.Rotate(new Vector3(0.0f, (Controller.GetLookInput().x * 1.0f), 0.0f), Space.Self);
        }

        // Vertical camera rotation
        {
            // Add vertical inputs to the camera's vertical angle
            CameraVerticalAngle -= Controller.GetLookInput().y;

            // Limit the camera's vertical angle to min/max
            CameraVerticalAngle = Mathf.Clamp(CameraVerticalAngle, -90.0f, 90.0f);

            // Apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            PlayerCamera.transform.localRotation = Quaternion.Euler(CameraVerticalAngle, 0.0f, Lean());
        }
    }

    // JumpAndGravity is called when character jumping and dealing with gravity
    public void JumpAndGravity()
    {
        if (bIsGrounded == true && m_currState == EPlayerState.Jumping)
        {
            m_currState = EPlayerState.Sprinting;
        }

        // If input for jump is detected and player is on the ground
        if (Controller.IsJumping() && bIsGrounded)
        {
            // If player is crouched, stand up
            if (m_bisCrouched)
            {
                Stand();
            }
            // If player is crawling, crouch
            else if (m_bisCrawling)
            {
                m_bisCrawling = false;
                m_bisCrouched = true;

                Crouch(m_bisCrouched);
            }
            // If player is standing, jump
            else
            {
                if (m_playerController.playerMovement.m_staminaComponent.GetCurrentStamina() >= m_playerController.playerMovement.m_staminaComponent.StaminaCost * 4)
                {
                    m_rigidBody.AddForce(Vector3.up * JumpForce);
                    bIsGrounded = false;
                    m_currState = EPlayerState.Jumping;
                }
            }
        }
    }

    // Function that changes the movement speed according to Current State of the Player
    private void ChangeCurrentSpeedMultipier()
    {
        // Checks if the player is on Ground and is trying to Jump
        if ((Controller.IsSprinting() || m_bIsSprinting) && bIsGrounded)
        {
            // If crouched or crawling, stand up
            if (m_bisCrouched || m_bisCrawling)
                Stand();

            if (m_staminaComponent.GetCurrentStamina() > 0)
            {
                //if (m_camDefaultY > m_crouchHeight)
                {
                    m_curntMvntMult = m_sprintMult;
                    m_bobSpeed = 9.0f;
                }

                if (m_currState != EPlayerState.Sprinting)
                    m_currState = EPlayerState.Sprinting;
            }

        }
        // Checks if the player is not crawling and wants to crouch
        else if (m_bisCrouched && !m_bisCrawling)
        {
            m_curntMvntMult = m_crouchMult;
            m_bobSpeed = 5.0f;
        }

        // Checks if the player is already crawling
        else if (m_bisCrawling)
        {
            m_curntMvntMult = m_crawlMult;
            m_bobSpeed = 0.0f;
        }
        else
        {
            m_curntMvntMult = m_walkMult;
            m_bobSpeed = 7.0f;

            if (m_currState != EPlayerState.Walking && m_currState != EPlayerState.Jumping)
                m_currState = EPlayerState.Walking;
        }

        if (Controller.GetMoveInput().z < 0.4f && !m_bisCrawling && !m_bisCrouched)
        {
            m_curntMvntMult = m_walkMult;
            m_bobSpeed = 7.0f;

            if (m_currState != EPlayerState.Walking && m_currState != EPlayerState.Jumping)
                m_currState = EPlayerState.Walking;

            if (m_bIsSprinting)
                m_bIsSprinting = false;
        }
    }

    // Function handling lean rotation and lean camera position
    float Lean()
    {
        // If not previously leaning
        if (!m_bIsLeaningLeft && !m_bIsLeaningRight)
        {
            m_camLeanPos = PlayerCamera.transform.localPosition;
            m_currCamPos = m_camLeanPos.x;
        }

        RaycastHit ray;

        //If left lean input is detected
        if (Controller.LeanLeft())
        {
            if (Physics.Raycast(PlayerCamera.transform.position, -transform.right, out ray, m_maxLeanPos, 9, QueryTriggerInteraction.Ignore))
            {
                // Lean angle becomes a percentage of its max value, based on distance to hit object
                float angle = m_maxCamAngle * (ray.distance / m_maxLeanPos);

                // Angle and move camera to lean values
                m_camAngle = Mathf.MoveTowardsAngle(m_camAngle, angle, m_leanSpeed * Time.deltaTime);
                m_camLeanPos.x = Mathf.Lerp(m_camLeanPos.x, m_currCamPos - ray.distance, 0.1f);
            }
            else
            {
                m_camAngle = Mathf.MoveTowardsAngle(m_camAngle, m_maxCamAngle, m_leanSpeed * Time.deltaTime);
                m_camLeanPos.x = Mathf.Lerp(m_camLeanPos.x, m_currCamPos - m_maxLeanPos, 0.1f);
            }

            m_bIsLeaningLeft = true;
        }
        // If Right lean input is detected
        else if (Controller.LeanRight())
        {
            if (Physics.Raycast(PlayerCamera.transform.position, transform.right, out ray, m_maxLeanPos, 9, QueryTriggerInteraction.Ignore))
            {
                // Lean angle becomes a percentage of its max value, based on distance to hit object
                float angle = -m_maxCamAngle * (ray.distance / m_maxLeanPos);

                // Angle and move camera to lean values
                m_camAngle = Mathf.MoveTowardsAngle(m_camAngle, angle, m_leanSpeed * Time.deltaTime);
                m_camLeanPos.x = Mathf.Lerp(m_camLeanPos.x, m_currCamPos + ray.distance, 0.1f);
            }
            else
            {
                m_camAngle = Mathf.MoveTowardsAngle(m_camAngle, -m_maxCamAngle, m_leanSpeed * Time.deltaTime);
                m_camLeanPos.x = Mathf.Lerp(m_camLeanPos.x, m_currCamPos + m_maxLeanPos, 0.1f);
            }

            m_bIsLeaningRight = true;
        }
        // If no lean input is detected, reset
        else
        {
            m_camAngle = Mathf.MoveTowardsAngle(m_camAngle, 0.0f, m_leanSpeed * Time.deltaTime);
            m_camLeanPos.x = Mathf.Lerp(m_camLeanPos.x, 0.0f, 0.1f);


            m_bIsLeaningLeft = false;
            m_bIsLeaningRight = false;
        }

        // Set camera position to lean position
        PlayerCamera.transform.localPosition = m_camLeanPos;

        // Return angle for camera rotation
        return m_camAngle;
    }

    // Method called when the Player is Crouching
    void Crouch(bool toggle)
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();

        float deltaY = col.height - m_crouchHeight;

        m_bStandPriority = false;

        if (toggle)
        {
            col.height -= deltaY;

            if (col.radius != 0.5f)
                col.radius = 0.5f;

            m_camDefaultY = Mathf.Lerp(m_camDefaultY, m_crouchHeight, Time.deltaTime);

            m_currState = EPlayerState.Crouching;
        }
        else
        {
            Stand();
        }
    }

    // Method called when the Player is Crawling
    void Crawl(bool toggle)
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();

        float deltaY = col.height - m_crawlHeight;

        m_bStandPriority = true;

        if (toggle)
        {
            col.height -= deltaY;
            col.radius = m_crawlHeight;
            m_camDefaultY = Mathf.Lerp(m_camDefaultY, m_crawlHeight, Time.deltaTime);

            m_currState = EPlayerState.Crawling;
        }
        else
        {
            Stand();
        }
    }

    // Function that makes the Player stand after crouching/crawling
    void Stand()
    {
        RaycastHit ray;
        if (!Physics.SphereCast(transform.position, 0.5f, Vector3.up, out ray, 1.0f, 9))
        {
            CapsuleCollider col = GetComponent<CapsuleCollider>();

            float deltaY = col.height - m_defaultHeight;

            col.height -= deltaY;

            if (col.radius != 0.35f)
                col.radius = 0.35f;

            m_camDefaultY = Mathf.Lerp(m_camDefaultY, m_defaultHeight, Time.deltaTime);

            m_bisCrawling = false;
            m_bisCrouched = false;
            m_bStandPriority = false;


            if (m_currState != EPlayerState.Jumping)
            {
                m_currState = EPlayerState.Walking;
            }
        }
    }

    // Function that is called whenever the player enters a new State
    public void EnterState(EPlayerState newState)
    {
        switch (newState)
        {
            case EPlayerState.Walking:
                {
                    m_currState = EPlayerState.Walking;
                }
                break;
            case EPlayerState.Sprinting:
                {
                    m_currState = EPlayerState.Sprinting;
                }
                break;
            case EPlayerState.Crouching:
                {
                    m_currState = EPlayerState.Crouching;
                }
                break;
            case EPlayerState.Crawling:
                {
                    m_currState = EPlayerState.Crawling;
                }
                break;
            case EPlayerState.Jumping:
                {
                    m_currState = EPlayerState.Jumping;
                }
                break;
        }
    }

    // Getter for Player's Rigidbody
    public Rigidbody GetRigidbody()
    {
        return m_rigidBody;
    }

    // Getter for Player's current state
    public EPlayerState GetState()
    {
        return m_currState;
    }

    // Method that forces sprinting to stop based on the Stamina
    public void ForceStopSprint()
    {
        m_curntMvntMult = m_walkMult;
        m_bobSpeed = 7.0f;

        if (m_currState != EPlayerState.Walking && m_currState != EPlayerState.Jumping)
            m_currState = EPlayerState.Walking;

        if (m_bIsSprinting)
            m_bIsSprinting = false;
    }

#if UNITY_EDITOR

    // Getter for Crouch
    public bool GetCrouched()
    {
        return m_bisCrouched;
    }

    // Getter for Crawl
    public bool GetCrawling()
    {
        return m_bisCrawling;
    }
#endif
}