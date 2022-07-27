// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Bryson Bertuzzi
Date Created: 04/10/2021
Comment/Description: Main Player/Character Input Script
ChangeLog:
13/11/2021: Fixed nameing convention of InputType to EInputType
09/02/2022: Added Getters for Mouse/Controller Sensitivity
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Main Player/Character Input Script
ChangeLog:
05/11/2021: Added Example ChangeLog
07/11/2021: Added Enum states for Input type
08/11/2021: Added functionality to PS$Controller and XboxController types
09/11/2021: Added all buttons from Unity Input Manager
10/11/2021: Added Toggle Inventory for inventory Input
01/12/2021: Added LeanRight and LeanLeft declarations to parent class IControllerInterface
*/

/*
Author: Seth Grinstead
ChangeLog:
15/11/2021: Added inputs for leaning
26/01/2022: Added floats for look sensitivity on mouse and controller
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Interface which has all the methods for controls of the Player
public class MouseKeyPlayerController : IControllerInterface
{
    // Enum for Input types for the Player Control
    public enum EInputType
    {
        KeyboardAndMouse,
        PS4Controller,
        XboxController,
    }

    public EInputType CurrInput;

    float m_mouseSensitivity = 2.0f;
    float m_xboxSensitivity = 2.0f;

    // Default constructor for the class
    public MouseKeyPlayerController()
    {
        CurrInput = EInputType.KeyboardAndMouse;
    }

    // Method that returns the movement of the player
    public Vector3 GetMoveInput()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return new Vector3(Input.GetAxis("KMHorizontal"), 0.0f, Input.GetAxis("KMVertical"));

            case EInputType.PS4Controller:
                return new Vector3(Input.GetAxis("PSHorizontal"), 0.0f, Input.GetAxis("PSVertical"));

            case EInputType.XboxController:
                return new Vector3(Input.GetAxis("XBHorizontal"), 0.0f, Input.GetAxis("XBVertical"));
        }
        return Vector3.zero;
    }

    // Returns Mouse input or where the mouse is facing on the screen
    public Vector3 GetLookInput()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return new Vector3(Input.GetAxis("KMX") * m_mouseSensitivity, Input.GetAxis("KMY") * m_mouseSensitivity, 0.0f);

            case EInputType.PS4Controller:
                return new Vector3(Input.GetAxis("PSX"), Input.GetAxis("PSY"), 0.0f);

            case EInputType.XboxController:
                return new Vector3(Input.GetAxis("XBX") * m_xboxSensitivity, Input.GetAxis("XBY") * m_xboxSensitivity, 0.0f);
        }
        return Vector3.zero;
    }

    // Returns true when the Jump key is pressed
    public bool IsJumping()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetButtonDown("KMJump");

            case EInputType.PS4Controller:
                return Input.GetButtonDown("PSJump");

            case EInputType.XboxController:
                return Input.GetButtonDown("XBJump");
        }
        return false;
    }

    // Returns true when the Pickup key is pressed
    public bool IsPickingUp()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetButton("KMInteract");

            case EInputType.PS4Controller:
                return Input.GetButton("PSInteract");

            case EInputType.XboxController:
                return Input.GetAxis("XBInteract") > 0.0f;
        }
        return false;
    }

    public bool IsThrowing()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetButton("KMThrow");

            case EInputType.PS4Controller:
                return Input.GetButton("PSThrow");

            case EInputType.XboxController:
                return Input.GetAxis("XBThrow") > 0.0f;
        }
        return false;
    }

    // Returns true when the Sprint key is pressed
    public bool IsSprinting()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetButton("KMSprint");

            case EInputType.PS4Controller:
                return Input.GetButtonDown("PSSprint");

            case EInputType.XboxController:
                return Input.GetButtonDown("XBSprint");
        }
        return false;
    }

    // Returns true when the Crouch key is pressed
    public bool ToggleCrouch()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetKeyDown(KeyCode.C);

            case EInputType.PS4Controller:
                return Input.GetKeyDown(KeyCode.JoystickButton3);

            case EInputType.XboxController:
                return Input.GetKeyDown(KeyCode.JoystickButton1);
        }
        return false;
    }

    // Returns true when the Crawl key is pressed
    public bool ToggleCrawl()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetKeyDown(KeyCode.V);

            case EInputType.PS4Controller:
                return Input.GetKey(KeyCode.JoystickButton2);

            case EInputType.XboxController:
                return Input.GetKey(KeyCode.JoystickButton1);
        }
        return false;
    }

    // Returns true when the Quit key is pressed
    public bool QuitGame()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetKeyDown(KeyCode.Escape);

            case EInputType.PS4Controller:
                return Input.GetKeyDown(KeyCode.JoystickButton9);

            case EInputType.XboxController:
                return Input.GetKeyDown(KeyCode.JoystickButton7);
        }
        return false;
    }

    // Returns true when the Switch WalkieTalkie key is pressed
    public bool SwitchToWalkieTalkie()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetKeyDown(KeyCode.P);

            case EInputType.PS4Controller:
                return Input.GetKeyDown(KeyCode.JoystickButton4);

                //case EInputType.XboxController:
                //    return Input.GetKeyDown(KeyCode.JoystickButton4);
        }
        return false;
    }

    // Returns true when the Crank Flashlight/Toggle Flashlight key is pressed
    public bool CrankFlashlight()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetKey(KeyCode.G);

            case EInputType.PS4Controller:
                return Input.GetKey(KeyCode.JoystickButton10);

            case EInputType.XboxController:
                return Input.GetKey(KeyCode.JoystickButton4);
        }
        return false;
    }

    // Returns true when the flashlight is toggled
    public bool ToggleFlashlight()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetKeyDown(KeyCode.F);

            case EInputType.PS4Controller:
                return Input.GetKeyDown(KeyCode.JoystickButton11);

            case EInputType.XboxController:
                return Input.GetKeyDown(KeyCode.JoystickButton5);
        }
        return false;
    }

    // Method to toggle between the inventory
    public bool ToggleInventory()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetKeyDown(KeyCode.Tab);

            case EInputType.PS4Controller:
                return Input.GetKeyDown(KeyCode.JoystickButton13);

            case EInputType.XboxController:
                return Input.GetKeyDown(KeyCode.JoystickButton6);
        }
        return false;
    }

    // Returns true when button is pressed to lean left
    public bool LeanLeft()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetKey(KeyCode.Q);

            case EInputType.PS4Controller:
                return Input.GetAxis("PSHorizontalDPad") < 0.0f;

            case EInputType.XboxController:
                return Input.GetAxis("XBHorizontalDPad") < 0.0f;
        }
        return false;
    }

    // Returns true when button is pressed to lean right
    public bool LeanRight()
    {
        switch (CurrInput)
        {
            case EInputType.KeyboardAndMouse:
                return Input.GetKey(KeyCode.E);

            case EInputType.PS4Controller:
                return Input.GetAxis("PSHorizontalDPad") > 0.0f;

            case EInputType.XboxController:
                return Input.GetAxis("XBHorizontalDPad") > 0.0f;
        }
        return false;
    }

    public bool Cancel()
    {
        switch (CurrInput)
        {
            case EInputType.PS4Controller:
                return Input.GetButtonDown("PSCancel");

            case EInputType.XboxController:
                return Input.GetButtonDown("XBCancel");
        }
        return false;
    }

    // Sets sensitivity of mouse
    public void SetMouseSensitivity(float sens)
    {
        m_mouseSensitivity = sens;
    }

    // Sets sensitivity of controller
    public void SetControllerSensitivity(float sens)
    {
        m_xboxSensitivity = sens;
    }

    // Gets sensitivity for mouse
    public float GetMouseSensitivity()
    {
        return m_mouseSensitivity;
    }

    // Gets sensitivity for controller
    public float GetControllerSensitivity()
    {
        return m_xboxSensitivity;
    }
}