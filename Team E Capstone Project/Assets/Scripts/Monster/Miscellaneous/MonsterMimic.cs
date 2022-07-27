using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Description: Script handling monster mimic behaviour using an arbitrary list of clips
ChangeLog:
28/01/2022: Created script. Contains a timer and a list of clips. When timer hits zero, monster has a chance to play random clip
 */

public class MonsterMimic : MonoBehaviour
{
    // Reference to Monster's animator component
    AIController m_aiController;

    [Tooltip("List of arbitrary sounds for monster to make")]
    public List<AudioClip> ClipList;

    [Tooltip("When this timer hits zero, monster has a chance to make a sound")]
    public float MaxMimicTimer = 10.0f;

    [Tooltip("Decimal percent value. Keep between 0 and 1")]
    public float MimicChance = 0.33f;

    // Current timer value
    float m_mimicTimer;

    // Start is called before the first frame update
    void Start()
    {
        // Retrieve animator reference from game object
        m_aiController = GetComponent<AIController>();

        // Set current timer to max value
        m_mimicTimer = MaxMimicTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // If animator is not in vent or chase states
        if (m_aiController.GetCurrentState().GetName() == "Patrol State" || m_aiController.GetCurrentState().GetName() == "Investigate State")
        {
            // Decrease timer
            m_mimicTimer -= Time.deltaTime;

            // If timer reaches zero
            if (m_mimicTimer <= 0.0f)
            {
                // Reset timer value
                m_mimicTimer = MaxMimicTimer;

                // If a randomly generated number is within specified range
                if (Random.Range(0.0f, 1.0f) <= MimicChance)
                {
                    //Play Sound
                    //AudioMananger.EmitSound(GetComponent<AudioSource>(), ClipList[Random.Range(0, ClipList.Count)]);
                    Debug.Log("Monster says roar");
                }
            }
        }
        else
        {
            // Reset timer for later use
            m_mimicTimer = MaxMimicTimer;
        }
    }
}
