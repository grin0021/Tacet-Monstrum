// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles the working of the Stun Trap
ChangeLog:
20/10/2021: Created basic Trigger script
22/10/2021: Created a function in FollowMonster and linked this trigger to Stunned() function
05/11/2021: Refined code, added comments and updated changeLog
13/11/2021: Added null checks and refined variables names
*/

/*
Author: Seth Grinstead
ChangeLog:
08/11/2021: Removed reference to FollowMonster to update with new AI
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrapTrigger : MonoBehaviour
{
    public GameObject Monster;               // Reference to monster object
    bool m_bIsActive;                        // Bool to check if trap can be used or not

    Animator m_animator;
    AIController m_aiController;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize values
        m_bIsActive = true;

        m_animator = Monster.GetComponentInChildren<Animator>();
        m_aiController = Monster.GetComponent<AIController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when something enters the TriggerBox of the this GameObject
    void OnTriggerEnter(Collider other)
    {
        // Check if the trap is still active
        if (m_bIsActive == true)
        {
            // Check if the GameObject to pass through is the Enemy/Monster
            if (other.gameObject.tag == "Enemy")
            {
                m_animator.SetBool("Stun", true);

                m_aiController.StoreState();
                m_aiController.SetState(new StunState(m_aiController));

                m_aiController.GetComponentInChildren<AnimEventManager>().ReleasePlayer();
                m_bIsActive = false;
            }
        }
    }
}
