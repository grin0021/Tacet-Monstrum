using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Description: Script handling chase behaviour of AI
ChangeLog: 
10/02/2022: Script created. Script created. Same logic/functionality as original, just goes through AIController instead of animator 
*/

public class ChaseState : AIState
{
    public ChaseState(AIController aiController)
        : base (aiController)
    {
    }

    public override void Activate()
    {
        // Set AI stopping distance to attack distance
        AIController.NavMesh.stoppingDistance = AIController.MaxAttackDist;
    }

    public override void Deactivate()
    {
        
    }

    public override string GetName()
    {
        return "Chase State";
    }

    public override void Update()
    {
        // If target is not null
        if (AIController.Target != null)
        {
            // Set and store destination to target's position
            AIController.NavMesh.SetDestination(AIController.Target.transform.position);
            AIController.SetLastDestination(AIController.Target.transform.position);
        }
        else
        {
            // Set state to patrol state if no target
            AIController.SetState(AIController.GetStoredState());
        }

        // If AI is within attack distance
        if (AIController.NavMesh.remainingDistance < AIController.NavMesh.stoppingDistance)
        {
            // Change to attack state
            AIController.SetState(new AttackState(AIController));
        }
    }
}