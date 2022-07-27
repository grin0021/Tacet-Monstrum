using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Description: Script handling patrolling behaviour of AI
ChangeLog: 
10/02/2022: Script created. Same logic/functionality as original, just goes through AIController instead of animator 
07/04/2022: Added comments
*/

public class PatrolState : AIState
{
    GameObject m_nextPatrolPoint;           // Reference to next patrol point
    GameObject m_lastPatrolPoint;           // Reference to last patrol point used

    float m_maxWaitTime = 3.0f;             // Maximum time AI waits between patrols
    float m_waitTime;                       // Current wait time between patrols

    // Start is called before the first frame update
    public PatrolState(AIController aiController, GameObject patrolPoint)
        : base(aiController)
    {
        // Store patrol point transforms
        m_nextPatrolPoint = patrolPoint;

        // Set wait time to max time
        m_waitTime = m_maxWaitTime;
    }

    public override void Activate()
    {
        // Set NavMesh stopping distance to default distance
        AIController.NavMesh.stoppingDistance = AIController.ReachDestinationDist;

        // If next patrol point is null
        if (!m_nextPatrolPoint)
        {
            // Find patrol point from scene
            m_nextPatrolPoint = GameObject.Find("PatrolPoint1");
        }

        // Set and store destination as patrol point at index
        AIController.NavMesh.SetDestination(m_nextPatrolPoint.transform.position);
        AIController.SetLastDestination(m_nextPatrolPoint.transform.position);

        // If last patrol point is null
        if (!m_lastPatrolPoint)
        {
            // Save last patrol point
            m_lastPatrolPoint = m_nextPatrolPoint;
        }
    }

    public override void Deactivate()
    {
        // Save this iteration of patrol state
        AIController.SaveSafetyState(this);
    }

    public override string GetName()
    {
        return "Patrol State";
    }

    public override void Update()
    {
        // Update Patrol
        UpdatePatrolPoint();
    }

    void UpdatePatrolPoint()
    {
        // If AI has reached its destination
        if (AIController.NavMesh.remainingDistance < AIController.NavMesh.stoppingDistance)
        {
            // If patrol point is tagged as a wait point
            if (m_nextPatrolPoint.GetComponent<TagList>().HasTag("WaitPoint"))
            {
                // Decrement wait time
                m_waitTime -= Time.deltaTime;

                // If wait time reaches zero
                if (m_waitTime <= 0.0f)
                {
                    // Set wait time to max time
                    m_waitTime = m_maxWaitTime;

                    DynamicPatrol pat = m_nextPatrolPoint.GetComponent<DynamicPatrol>();

                    if (pat)
                    {
                        // Set and store destination to patrol point at index
                        GameObject last = m_lastPatrolPoint;
                        m_lastPatrolPoint = m_nextPatrolPoint;
                        m_nextPatrolPoint = pat.GetRandomPoint(last);

                        AIController.NavMesh.SetDestination(m_nextPatrolPoint.transform.position);
                        AIController.SetLastDestination(m_nextPatrolPoint.transform.position);
                    }
                }
            }
            else
            {
                DynamicPatrol pat = m_nextPatrolPoint.GetComponent<DynamicPatrol>();

                if (pat)
                {
                    // Set and store destination to patrol point at index
                    GameObject last = m_lastPatrolPoint;
                    m_lastPatrolPoint = m_nextPatrolPoint;
                    m_nextPatrolPoint = pat.GetRandomPoint(last);

                    AIController.NavMesh.SetDestination(m_nextPatrolPoint.transform.position);
                    AIController.SetLastDestination(m_nextPatrolPoint.transform.position);
                }
            }
        }
    }
}