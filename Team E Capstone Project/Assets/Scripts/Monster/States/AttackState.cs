using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Description: Script handling attacking behaviour of AI
ChangeLog: 
10/02/2022: Script created. Script created. Same logic/functionality as original, just goes through AIController instead of animator 
*/

public class AttackState : AIState
{
    Animator m_animator;                // Reference to AI animator
    AnimEventManager m_animManager;

    public AttackState(AIController aiController)
        : base(aiController)
    {

    }

    public override void Activate()
    {
        // If animator is not set
        if (!m_animator)
        {
            // Get animator from AIController
            m_animator = AIController.Animator;
        }

        // Set animator attack bool to true
        m_animator.SetBool("Attack", true);

        // Get AnimEventManager from child object
        m_animManager = AIController.GetComponentInChildren<AnimEventManager>();

        // If AnimEventManager is successfully grabbed
        if (m_animManager)
        {
            // Set colliders relevant to attacking to false
            // (anim event manager handles enabling when necessary)
            m_animManager.JawCollider.enabled = false;
            m_animManager.HandCollider.enabled = false;
        }
    }

    public override void Deactivate()
    {
        // Set attack and bite bools to false
        m_animator.SetBool("Attack", false);
        m_animator.SetBool("Bite", false);

        // Ensure attacking colliders are disabled
        m_animManager.HandCollider.enabled = false;
        m_animManager.JawCollider.enabled = false;
    }

    public override string GetName()
    {
        return "Attack State";
    }

    public override void Update()
    {
        // Set NavMesh destination to the player
        AIController.NavMesh.SetDestination(AIController.PlayerTransform.position);

        // Store distance between AI and target
        float distance = (AIController.transform.position - AIController.PlayerTransform.position).magnitude;

        AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(m_animator.GetLayerIndex("Attack Layer"));
        
        // If distance is greater than attack distance
        if (distance > AIController.NavMesh.stoppingDistance && !info.IsName("Bite"))
        {
            // Set state to chase state
            AIController.SetState(new ChaseState(AIController));

            return;
        }

        // If target becomes null
        if (AIController.Target == null && !info.IsName("Bite"))
        {
            // Set state to patrol state
            AIController.SetState(AIController.GetStoredState());

            return;
        }

        // Calculate angle to player from AI
        Vector3 pos1 = AIController.transform.position;
        Vector3 pos2 = AIController.PlayerTransform.position;
        float angle = Mathf.Atan2(pos2.x - pos1.x, pos2.z - pos1.z) * Mathf.Rad2Deg;

        Vector3 euler = new Vector3();
        euler.y = angle;

        // Set angles with new rotation, to look at target
        AIController.transform.eulerAngles = euler;

        // Attack target
        Attack();
    }

    void Attack()
    {
        // Get distance to destination as percent 
        float distance = AIController.NavMesh.remainingDistance / AIController.NavMesh.stoppingDistance;

        // If distance is larger than 0.75 or if AI is not ready to bite
        if (distance >= 0.5f || !m_animManager.bCanBite)
        {
            // Set bite bool to false, monster does claw attack
            m_animator.SetBool("Bite", false);
        }
        else
        {
            // Set bite bool to true, monster does bite attack
            m_animator.SetBool("Bite", true);
        }
    }
}