using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
Author: Seth Grinstead
Description: Script handling venting behaviour of AI
ChangeLog: 
10/02/2022: Script created. Holds similar logic to original version, but checks paths after calculation to ensure proper vents are used
*/

public class VentState : AIState
{
    GameObject[] m_vents;                    // Array of vents in scene
    GameObject m_closestVent = null;         // Reference to closest vent

    Vector3 m_desiredDest;                   // Vector storing desired destination

    float m_trappedWaitTime = 10.0f;         // Time AI waits in a room if it's trapped

    bool m_bWarping = false;                 // bool flagging that AI is preparing to warp
    bool m_bisTrapped = false;               // If AI is in a closed room with no vents

    public VentState(AIController aiController, Vector3 goal)
        : base (aiController)
    {
        // Store desired destination pre-vent state
        m_desiredDest = goal;
    }

    public override void Activate()
    {
        AIController.Animator.SetBool("Vent", false);
        AIController.Animator.SetFloat("Vent Action", 0.0f);

        // Gather vents from scene
        m_vents = GameObject.FindGameObjectsWithTag("Vent");

        FindClosestVent(m_desiredDest, true);

        if (!m_closestVent)
        {
            AIController.SetSafetyState();
            return;
        }

        // Calculate closest vent to AI
        FindClosestVent(AIController.transform.position, false);

        // If closest vent is found
        if (m_closestVent)
        {
            // Set destination to closest vent
            AIController.NavMesh.SetDestination(m_closestVent.transform.position);
        }
        else
        {
            m_bisTrapped = true;
        }
    }

    public override void Deactivate()
    {
        // Set closest vent reference to null
        m_closestVent = null;

        // Set animator vent values to default values
        AIController.Animator.SetBool("Vent", false);
        AIController.Animator.SetFloat("Vent Action", 0.0f);

        // If Collider component is disabled, enable
        if (AIController.GetComponent<Collider>().enabled == false)
        {
            AIController.GetComponent<Collider>().enabled = true;
        }
    }

    public override string GetName()
    {
        return "Vent State";
    }

    public override void Update()
    {
        if (!m_bisTrapped)
        {
            // If NavMesh has arrived at destination
            if (AIController.NavMesh.remainingDistance < AIController.NavMesh.stoppingDistance && !m_bWarping)
            {
                // Set stored vent to null
                m_closestVent = null;

                // Start vent behaviour
                Vent();
            }
            else
            {
                // If not at destination, ensure that vent animation is not playing
                AIController.Animator.SetBool("Vent", false);
                AIController.Animator.SetFloat("Vent Action", 0.0f);
            }

            // If ready to warp
            if (m_bWarping)
            {
                // Set vent action float to 1.0f to reverse animation
                AIController.Animator.SetFloat("Vent Action", 1.0f);

                // Get current animator state info
                int layer = AIController.Animator.GetLayerIndex("Base Layer");
                AnimatorStateInfo info = AIController.Animator.GetCurrentAnimatorStateInfo(layer);

                // If state normalized time is 95%
                if (info.normalizedTime % 1 > 0.95f)
                {
                    // Set vent bool and action float to reset animation
                    AIController.Animator.SetBool("Vent", false);
                    AIController.Animator.SetFloat("Vent Action", 0.0f);

                    // Restore desired state
                    AIController.SetState(AIController.GetStoredState());
                }
            }
        }
        // If AI is trapped
        else
        {
            // Wait a duration before finding an escape point
            m_trappedWaitTime -= Time.deltaTime;

            // Once timer is done
            if (m_trappedWaitTime <= 0.0f)
            {
                // Find closest vent point
                FindClosestVent(AIController.transform.position, true);

                Renderer renderer = AIController.GetComponent<Renderer>();

                // If a point is found
                if (m_closestVent && !renderer.isVisible)
                {
                    // Calculate the direction towards the Player
                    Vector3 Dir = (AIController.PlayerTransform.position - AIController.transform.position).normalized;
                    float distance = Vector3.Distance(AIController.transform.position, AIController.PlayerTransform.position);

                    RaycastHit Ray;
                    int layerMask = LayerMask.GetMask("Not in Reflection") | LayerMask.GetMask("Default") | LayerMask.GetMask("Environment");
                    if (Physics.Raycast(AIController.transform.position, Dir, out Ray, distance, layerMask))
                    {
                        if (Ray.collider.gameObject.tag != "Player")
                        {
                            // Warp to escape point
                            AIController.NavMesh.Warp(m_closestVent.transform.position);

                            // Restore desired state
                            AIController.SetState(AIController.GetStoredState());
                        }
                    }
                    else
                    {
                        // Warp to escape point
                        AIController.NavMesh.Warp(m_closestVent.transform.position);

                        // Restore desired state
                        AIController.SetState(AIController.GetStoredState());
                    }
                }
            }
        }
    }

    void FindClosestVent(Vector3 destination, bool warp)
    {
        // Temp variable to store shortest distance between destination and vent
        float shortDis = 500.0f;

        // If vents have been found in the scene
        if (m_vents.Length > 0)
        {
            // Iterate through vent array
            for (int i = 0; i < m_vents.Length; i++)
            {
                // Temp variable to store distance between current vent and destination
                Vector3 loc = m_vents[i].transform.position - destination;

                // If first iteration or if this distance is shorter than stored shortest distance
                if (i == 0 || loc.magnitude < shortDis)
                {
                    // Check if vent at index is the current closest vent
                    if (m_closestVent == m_vents[i])
                    {
                        // If true, vent is invalid. Continue search.
                        continue;
                    }

                    // Create NavMeshPath variable
                    NavMeshPath path = new NavMeshPath();

                    // Calculate path to vent
                    AIController.NavMesh.CalculatePath(m_vents[i].transform.position, path);

                    // If path status is partial
                    if (path.status != NavMeshPathStatus.PathComplete && !warp)
                    {
                        continue;
                    }

                    // Reset path variable
                    path = new NavMeshPath();

                    // If AI is ready to warp
                    if (warp)
                    {
                        // Calculate a path between desired vent and desired location
                        NavMesh.CalculatePath(m_vents[i].transform.position, m_desiredDest, NavMesh.AllAreas, path);

                        // If path is partial
                        if (path.status != NavMeshPathStatus.PathComplete)
                        {
                            continue;
                        }
                    }

                    // Set new shortest distance
                    shortDis = loc.magnitude;

                    //Set closest vent to vent at index
                    m_closestVent = m_vents[i];
                }
            }
        }
    }

    void Vent()
    {
        // If collider is enabled, disable
        if (AIController.GetComponent<Collider>().enabled)
        {
            AIController.GetComponent<Collider>().enabled = false;
        }

        // If vent animation is not set and is not ready to warp
        if (!AIController.Animator.GetBool("Vent") && !m_bWarping)
        {
            // Set vent bool and action float to start vent animation
            AIController.Animator.SetBool("Vent", true);
            AIController.Animator.SetFloat("Vent Action", 0.0f);
        }

        // Get current animation state info
        int layer = AIController.Animator.GetLayerIndex("Base Layer");
        AnimatorStateInfo info = AIController.Animator.GetCurrentAnimatorStateInfo(layer);

        // If first vent animation is complete
        if (info.IsName("Vent") && info.normalizedTime % 1 < 0.05f)
        {
            // AI prepared to warp
            m_bWarping = true;

            // Find vent point closest to desired destination
            FindClosestVent(m_desiredDest, m_bWarping);

            // End vent behaviour
            if (m_closestVent)
            {
                // Warp to desired vent
                AIController.NavMesh.Warp(m_closestVent.transform.position);
            }
        }
    }
}