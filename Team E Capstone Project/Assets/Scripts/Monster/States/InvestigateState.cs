using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
Author: Seth Grinstead
Description: Script handling investigating behaviour of AI
ChangeLog: 
10/02/2022: Script created. Same functionality as original, but communicates with AIController for needed information
*/

public class InvestigateState : AIState
{
    Vector3 m_investLoc;              // Location to investigate around

    float m_waitTime;                 // Wait time between patrol points
    float m_maxWaitTime = 2.0f;       // Max wait time between patrol points

    int m_investigateCount = 3;       // Number of patrol points before leaving investigate

    public InvestigateState(AIController aiController, Vector3 investLoc)
        : base (aiController)
    {
        // Retrieve investigation point
        m_investLoc = investLoc;

        // Set wait time to max wait time
        m_waitTime = m_maxWaitTime;
    }

    public override void Activate()
    {
        // Set NavMesh stopping distance to default distance
        AIController.NavMesh.stoppingDistance = AIController.ReachDestinationDist;

        // Set and store NavMesh destination to initial investigate location
        SetDestination(m_investLoc);
    }

    public override void Deactivate()
    {
        
    }

    public override string GetName()
    {
        return "Investigate State";
    }

    public override void Update()
    {
        // Patrol around investigation point
        UpdateInvestigationPatrol();
    }

    void UpdateInvestigationPatrol()
    {
        // If NavMesh has reached its destination
        if (AIController.NavMesh.remainingDistance < AIController.NavMesh.stoppingDistance)
        {
            // Decrement wait time
            m_waitTime -= Time.deltaTime;

            // If wait time reaches zero
            if (m_waitTime <= 0.0f)
            {
                // Reset wait time
                m_waitTime = m_maxWaitTime;

                // Decrement investigate count
                m_investigateCount--;

                // If investigate count reaches zero
                if (m_investigateCount <= 0)
                {
                    // Return to patrol state
                    AIController.SetState(AIController.GetStoredState());
                    return;
                }

                // Create path object
                NavMeshPath path = new NavMeshPath();

                Vector3 randomPoint;

                do
                {
                    // Calculate random point around investigation location
                    randomPoint = m_investLoc + new Vector3(Random.Range(-5.0f, 5.0f), 0.0f, Random.Range(-5.0f, 5.0f));

                    // Calculate path to random point
                    AIController.NavMesh.CalculatePath(randomPoint, path);

                } while (path.status != NavMeshPathStatus.PathComplete);

                // Set and store new patrol point
                SetDestination(randomPoint);
                AIController.SetLastDestination(randomPoint);
            }
        }
    }

    void SetDestination(Vector3 pos)
    {
        NavMeshPath path = new NavMeshPath();

        AIController.NavMesh.CalculatePath(pos, path);

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            AIController.SetState(new VentState(AIController, pos));
            return;
        }
        
        AIController.NavMesh.SetDestination(pos);
        AIController.SetLastDestination(pos);
    }
}