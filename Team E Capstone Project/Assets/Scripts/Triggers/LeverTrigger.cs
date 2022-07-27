// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 30/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles the working of the Levers
ChangeLog:
30/11/2021: Script created
02/12/2021: Added commensta dnchangelog
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Class that handles the working of the Levers
ChangeLog:
02/12/2021: Fixed issue with timer causing lever to double increment number of levers pulled for door
*/

/*
Author: Jacob Kaplan-Hall
ChangeLog:
28/02/2022: fixed lever behavior
4/2/22: updated lever logic to be more flexable and added sounds
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : MonoBehaviour, IInteractable
{
    public Door m_Door;                                         // Reference to Door object in scene
    public GameObject Trigger;

    [SerializeField]
    private float m_reqLeverRotation = 85f;                     // Lever rotation required to open door

    [SerializeField]
    bool m_bFlipDirection = false;

    [SerializeField]
    bool m_bVertical = true;

    [SerializeField]
    float InteractScale = .95f;

    [SerializeField]
    AudioClip m_LeverSound = null;

    float Value = 0f;


    [SerializeField]
    Transform m_RotateSocketTransform = null;                   // Transform for the pivot point to rotate about
    Quaternion m_StartRotation = Quaternion.identity;           // Starting rotation

    bool b_isComplete = false;                                  // Bool for checking if the levers are rotated completely

    // Start is called before the first frame update
    private void Start()
    {
        // Initlaize Values
        m_StartRotation = m_RotateSocketTransform.localRotation;
    }

    // Function called when Interaction happens
    public void OnInteract(Interactor interactor)
    {
        if (b_isComplete == true)
        {
            interactor.StopInteracting();
        }
        else
        {
            m_lastAngle = m_bVertical ?  -interactor.GetLookTransform().localEulerAngles.x : -interactor.GetLookTransform().eulerAngles.y;
        }
    }

    // Called when the object is held
    public void UpdateHoldInteract(Interactor interactor)
    {
        //handle timer
        //if (t_SwitchTimer != m_ActivationTime)
        //t_SwitchTimer = Mathf.Min(t_SwitchTimer + Time.deltaTime, m_ActivationTime);
        float InteractorAngle = m_bVertical ? -interactor.GetLookTransform().localEulerAngles.x : -interactor.GetLookTransform().eulerAngles.y;
        Value = Mathf.Clamp(Value + Mathf.DeltaAngle(m_lastAngle, InteractorAngle) * .03f * (m_bFlipDirection ? -1f : 1f), 0,1);
        m_lastAngle = InteractorAngle;

        //rotate lever
        m_RotateSocketTransform.localRotation = Quaternion.Lerp(m_StartRotation, m_StartRotation * Quaternion.Euler(m_reqLeverRotation, 0, 0), Value);

        //check if we're done
        if (b_isComplete == false && Value == 1)
        {
            if (m_Door)
            {
                m_Door.NumLeversPulled += 1;
                b_isComplete = true;
                AudioManager.PlaySound(transform, m_LeverSound);
                // If the levers are all pulled/pushed, open the doors
                if (m_Door.NumLeversPulled == m_Door.NumOfLeversReq)
                {
                    m_Door.OpenDoor();
                }

                interactor.StopInteracting();
            }

            if (Trigger)
            {
                Trigger.SetActive(true);
            }
        }

    }
    //interactable interface
    float m_lastAngle = 0;

    public void FixedUpdateHoldInteract(Interactor interactor)
    {
    }
    public void OnEndInteract(Interactor interactor)
    {
    }
}