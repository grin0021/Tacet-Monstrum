// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Comment/Description: Script dictating AI reactions to collisions
ChangeLog:
09/11/2021: Script created
11/11/2021: Modified trigger function and added comments.
13/11/2021: Added null checks and refined variables names
18/11/2021: Added code to prevent monster from triggering itself if it hits a box
10/02/2022: Modified stun logic to fit with new AI system
07/04/2022: Added comments
*/

/*
Author: Jacob Kaplan-Hall
ChangeLog
14/11/21: changed script to work with new audio system
 */

public class MonsterCollision : MonoBehaviour
{
    public AIController AIController;
    void OnCollisionEnter(Collision Col)
    {
        TagList list = Col.gameObject.GetComponent<TagList>();

        // If colliding object is a box, prevent monster from triggering itself with the impact sound
        if (list)
        {
            // If colliding object has a TagList with tag "Box"
            if (Col.gameObject.GetComponent<TagList>().HasTag("Box"))
            {
                // Flag box as having been hit by monster
                Col.gameObject.GetComponent<BoxSound>().bWasHitByMonster = true;
            }
        }

        // If colliding object is player, have AI search for player
        if (Col.gameObject.tag == "Player")
        {
            if (AIController.GetCurrentState().GetName() != "Stun State")
            {
                GetComponent<MonsterDetection>().SearchForPlayer();
            }
        }
    }

    void OnTriggerEnter(Collider Col)
    {
        // If triggering object is a trap trigger
        if (Col.gameObject.GetComponent<TagList>() && Col.gameObject.GetComponent<TagList>().HasTag("Trap Trigger"))
        {
            // Store current AI state and set to stun state
            AIController.StoreState();
            AIController.SetState(new StunState(AIController));
        }
    }
}