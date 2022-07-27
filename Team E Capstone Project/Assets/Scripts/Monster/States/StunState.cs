using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Description: Script handling stunned behaviour of AI
ChangeLog: 
10/02/2022: Script created. Same logic/functionality as original, just goes through AIController instead of animator
*/

public class StunState : AIState
{
    float m_stunTime = 9.5f;      // Maximum stun duration

    public StunState(AIController aiController)
        : base (aiController)
    {

    }

    public override void Activate()
    {
        // Set destination to current position
        AIController.NavMesh.SetDestination(AIController.transform.position);

        // Set velocity to zero
        AIController.NavMesh.velocity = Vector3.zero;
    }

    public override void Deactivate()
    {
        
    }

    public override string GetName()
    {
        return "Stun State";
    }

    public override void Update()
    {
        // Set NavMesh velocity to zero
        AIController.NavMesh.velocity = Vector3.zero;

        // Calculate animation blend value based on time in state
        float blendVal = (9.5f - m_stunTime) / 9.5f;

        // Set blend value in animator
        AIController.Animator.SetFloat("Stun Duration", blendVal);

        // Decrement stun duration
        m_stunTime -= Time.deltaTime;

        // If stun duration reaches zero
        if (m_stunTime <= 0.0f)
        {
            // Set stun bool to false;
            AIController.Animator.SetBool("Stun", false);

            // Set state to stored state
            AIController.SetSafetyState();
        }
    }
}