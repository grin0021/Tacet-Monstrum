// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 03/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Class that handles the working of the Checkpoints
ChangeLog:
03/11/2021: Created basic Triggerbox 
04/11/2021: Linked it to the Inventory directly
04/11/2021: Linked it to PlayerController calling the functions to Load and Save Player and Inventory
13/11/2021: Added null checks and refined variables names
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Name of the checkpoint according to the room/level
    [SerializeField]
    string m_CheckpointName;

    // Deactivate when the player has saved the progress once
    [SerializeField]
    bool m_bIsActive;

    // Start is called before the first frame update
    void Start()
    {
        // Set the checkpoints to be active when the Scene Loads
        m_bIsActive = true;

        // If there is no name provided, name it "Default Checkpoint"
        if (m_CheckpointName == "")
        {
            m_CheckpointName = "Default Checkpoint";
        }
    }

    // Will be called when the Player passes through the trigger box of the Checkpoint
    private void OnTriggerEnter(Collider pcollider)
    {
        // If the checkpoint is still ACTIVE
        if (m_bIsActive)
        {
            // Check if the collided object is the Player by checking the tag
            if (pcollider.gameObject.tag == "Player")
            {
                // Save Inventory and SpawnPosition of the Player for the particular checkpoint
                if (pcollider.gameObject.GetComponent<PlayerController>() != null)
                {
                    // Calling SavePlayer() on the Player to save its data
                    pcollider.gameObject.GetComponent<PlayerController>().SavePlayer();

                    // Calling SaveInventory() and feeding Player's current inevntory to save
                    SaveSystem.SaveInventory(pcollider.gameObject.GetComponent<PlayerController>().inventory, "Inventory");
                    SaveSystem.SaveInventory(pcollider.gameObject.GetComponent<PlayerController>().Documents, "Documents");
                    SaveSystem.SaveInventory(pcollider.gameObject.GetComponent<PlayerController>().AudioTapes, "AudioTapes");

                    // Deactivate the checkpoint after saving everything
                    m_bIsActive = false;

                    DoorData data = new DoorData();
                    SaveSystem.SaveDoors(data);
                }
            }
        }
    }
}
