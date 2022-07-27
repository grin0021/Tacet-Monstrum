using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
Author: Seth Grinstead
Description: Script handling persistent AI behaviours and state changes
ChangeLog: 
10/02/2022: Script created. Central point for AI work, as animator was in previous version
03/03/2022: Fully implemented sound void mechanic.
07/04/2022: Removed unnecessary code, reorganized, commented appropriately.
*/

public class AIController : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent NavMesh;     // Reference to AI's NavMeshAgent
    [HideInInspector] public GameObject Target;        // Reference to target object

    [Header("AudioDetector Settings")]
    public float PatrolListenVol = 0.1f;               // Minimum hearing volume for patrol state
    public float InvestigateListenVol = 0.01f;         // Minimum hearing volume for investigate state
    public float ChaseListenVol = 0.005f;              // Minimum hearing volume for chase state

    [Header("NavMesh Settings")]
    public GameObject FirstPatrolPoint;                // List of Patrol Point transforms for reference
    public float ReachDestinationDist = 2.0f;          // Normal stopping distance for AI
    public float MaxAttackDist = 3.0f;                 // Attack distance for AI  
    public float PatrolSpeed = 3.5f;                   // Speed of AI in patrol state
    public float InvestigateSpeed = 5.0f;              // Speed of AI in investigate state
    public float ChaseSpeed = 7.5f;                    // Speed of AI in chase state

    [Header("AI References")]
    public Animator Animator;                          // Reference to AI animator controller
    public MonsterDetection MonsterDetection;          // Reference to AI MonsterDetection script

    [Header("Player References")]
    public Transform PlayerTransform;                  // Reference to player transform

    int m_attackLayerIndex;                            // Stored value of Attack layer index in animator
    float m_attackLayerWeight;                         // Weight value of attack layer in animator

    AIState m_currentState;                            // Current state of AI
    AIState m_storedState;                             // Stored state of AI
    AIState m_defaultPatrol;                           // Safety default state

    // Start is called before the first frame update
    void Start()
    {
        // Get NavMeshAgent reference from game object
        NavMesh = GetComponent<NavMeshAgent>();

        // Get attack animation layer index
        m_attackLayerIndex = Animator.GetLayerIndex("Attack Layer");

        // Set AI state to patrol state
        SetState(new PatrolState(this, FirstPatrolPoint));
        m_defaultPatrol = m_currentState;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimations();
        
        // If current state is not null
        if (m_currentState != null)
        {
            // Update current state
            m_currentState.Update();
        }

        // If path of NavMesh is partial
        if (NavMesh.pathStatus != NavMeshPathStatus.PathComplete)
        {
            // If not already in vent state
            if (m_currentState.GetName() != "Vent State")
            {
                // Store current state
                StoreState();

                // Set state to vent state
                SetState(new VentState(this, GetLastDestination()));
            }
        }
    }

    // Updates animator values as necessary
    void UpdateAnimations()
    {
        // Set blend float in animator as percentage of max speed
        float val = NavMesh.velocity.magnitude / ChaseSpeed;
        Animator.SetFloat("Blend", val);

        // If AI is attacking
        if (ShouldUseAttackLayer())
        {
            //Lerp weight of attack layer to play with base animations
            m_attackLayerWeight = Mathf.Lerp(m_attackLayerWeight, 1.0f, Time.deltaTime * 10.0f);
            Animator.SetLayerWeight(m_attackLayerIndex, m_attackLayerWeight);
        }
        else
        {
            // Return to zero weight to return to base animations
            m_attackLayerWeight = Mathf.Lerp(m_attackLayerWeight, 0.0f, Time.deltaTime * 7.5f);
            Animator.SetLayerWeight(m_attackLayerIndex, m_attackLayerWeight);
        }
    }

    // Should Animator attack layer be used
    bool ShouldUseAttackLayer()
    {
        // Return true if AI is in attack state
        return m_currentState.GetName() == "Attack State";
    }

    // Set AI state
    public void SetState(AIState state)
    {
        // Set speed and minimum hearing radius based on state
        switch (state.GetName())
        {
            case "Patrol State":
                MonsterDetection.HearDistance = 10.0f;
                NavMesh.speed = PatrolSpeed;
                break;
            case "Investigate State":
                MonsterDetection.HearDistance = 15.0f;
                NavMesh.speed = InvestigateSpeed;
                break;
            case "Chase State":
                MonsterDetection.HearDistance = 20.0f;
                NavMesh.speed = ChaseSpeed;
                break;
        }

        // If current state is not null
        if (m_currentState != null)
        {
            // Deactivate current state
            m_currentState.Deactivate();
        }

        // Set new state
        m_currentState = state;

        // If new state is not null
        if (m_currentState != null)
        {
            // Activate new state
            m_currentState.Activate();
        }
    }

    // Set state to last version of patrol state
    public void SetSafetyState()
    {
        SetState(m_defaultPatrol);
    }

    // Store current AI state
    public void StoreState()
    {
        // Store current state in new variable
        m_storedState = m_currentState;
    }

    // Store last desired destination
    public void SetLastDestination(Vector3 dest)
    {
        // Store desired destination
        MonsterDetection.SetLastDestination(dest);
    }

    // Save patrol "Safety" state
    public void SaveSafetyState(AIState state)
    {
        if (state.GetName() == "Patrol State")
        {
            m_defaultPatrol = state;
        }
    }

    // Get last desired destination
    public Vector3 GetLastDestination()
    {
        // return last desired destination
        return MonsterDetection.GetLastDestination();
    }

    // Get current AI state
    public AIState GetCurrentState()
    {
        // Return current state
        return m_currentState;
    }

    // Get stored AI state
    public AIState GetStoredState()
    {
        // return stored state
        return m_storedState;
    }
}
