// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 01/11/2021

/*
Author: Bryson Bertuzzi
Date Created: 20/10/2021
Comment/Description: Script for Saving and Loading of Player's data and Inventories
ChangeLog:
21/10/2021: Saving and loading works with both player data and inventory
25/10/2021: Cleaned up SaveSystem and Added ability to delete duplicate items on load
11/11/2021: Added Saving for Documents and Audio Tapes
05/12/2021: Added Saving and loading for Settings 
05/12/2021: Updated CheckIfFileExists function to properly check a specific file
22/03/2022: Added saving and loading for doors
31/03/2022: Reworked deleting duplicate objects on load inventory
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Script for Saving and Loading of Player's data and Inventory
ChangeLog:
07/11/2021: Added comments for all methods and the class
15/11/2021: Refined code and added date of creation to changelog
*/

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

// Class that handles saving Player's data
public static class SaveSystem
{
    private static string m_savePathPlayer = Application.persistentDataPath + "/save.data"; // Save path for Player
    private static string m_savePathInventory = Application.persistentDataPath + "/inventory.data"; // Save path for Inventory
    private static string m_savePathDocuments = Application.persistentDataPath + "/documents.data"; // Save path for Documents
    private static string m_savePathAudioTapes = Application.persistentDataPath + "/audiotapes.data"; // Save path for Audio Tapes
    private static string m_savePathSettings = Application.persistentDataPath + "/settings.data"; // Save path for Settings
    private static string m_savePathDoors = Application.persistentDataPath + "/Doors.data"; // Save path for Doors

    // Method for saving the data
    public static void SavePlayer(PlayerData data)
    {
        try
        {
            // Save PlayerData to Save File
            Serialize(m_savePathPlayer, data);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    // Method that handles Loading of the player
    public static PlayerData LoadPlayer()
    {
        if (File.Exists(m_savePathPlayer))
        {
            // Create a PlayerData
            PlayerData data = new PlayerData();

            // Load PlayerData from Save File
            data = (PlayerData)Deserialize(m_savePathPlayer, data);

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + m_savePathPlayer);

            return null;
        }
    }

    // Saving player's Inventory
    public static void SaveInventory(InventoryObject inventory, string inventoryName)
    {
        // If inventoryName is equal to...
        switch (inventoryName)
        {
            case "Inventory":
                // Set inventoryName to Inventory Save Path
                inventoryName = m_savePathInventory;
                break;

            case "Documents":
                // Set inventoryName to Documents Save Path
                inventoryName = m_savePathDocuments;
                break;

            case "AudioTapes":
                // Set inventoryName to Audio Tapes Save Path
                inventoryName = m_savePathAudioTapes;
                break;
        }

        try
        {
            // Save Inventory to Save File
            Serialize(inventoryName, inventory);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex, inventory);
        }
    }

    // Loading player's inventory
    public static InventoryObject LoadInventory(string inventoryName)
    {
        // If inventoryName is equal to...
        switch (inventoryName)
        {
            case "Inventory":
                // Set inventoryName to Inventory Save Path
                inventoryName = m_savePathInventory;
                break;

            case "Documents":
                // Set inventoryName to Documents Save Path
                inventoryName = m_savePathDocuments;
                break;

            case "AudioTapes":
                // Set inventoryName to Audio Tapes Save Path
                inventoryName = m_savePathAudioTapes;
                break;
        }

        if (File.Exists(inventoryName))
        {
            // Create instance of InventoryObject
            InventoryObject inventory = ScriptableObject.CreateInstance<InventoryObject>();

            // Load Inventory from Save File
            inventory = (InventoryObject)Deserialize(inventoryName, inventory);

            // Clean up duplicate items that are in the world and inventory
            {
                // Find all GameObjects in the scene with the tag "Interactable Object".
                GameObject[] go = GameObject.FindGameObjectsWithTag("Interactable Object");

                for (int i = 0; i < go.Length; i++)
                {
                    for (int j = 0; j < inventory.GetInventory().Items.Length; j++)
                    {
                        if (go[i].GetComponent<GroundItem>() != null)
                        {
                            GroundItem groundItem = go[i].GetComponent<GroundItem>();

                            if (groundItem.Item != null)
                            {
                                // If scene GameObject has the same ID as an item in the inventory...
                                if (groundItem.Item.Id == inventory.GetInventory().Items[j].Item.Id)
                                {
                                    // Destroy the scene GameObject
                                    GameObject.Destroy(go[i]);
                                }
                            }
                        }
                    }
                }
            }
            return inventory;
        }
        else
        {
            Debug.LogError("Save file not found in " + inventoryName);

            return null;
        }
    }

    // Method for saving the data
    public static void SaveSettings(SettingsData data)
    {
        try
        {
            // Save PlayerData to Save File
            Serialize(m_savePathSettings, data);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    // Method that handles Loading of the player
    public static SettingsData LoadSettings()
    {
        if (File.Exists(m_savePathSettings))
        {
            // Create a PlayerData
            SettingsData data = new SettingsData();

            // Load PlayerData from Save File
            data = (SettingsData)Deserialize(m_savePathSettings, data);

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + m_savePathSettings);

            return null;
        }
    }

    // Method for saving the data
    public static void SaveDoors(DoorData data)
    {
        try
        {
            // Save DoorData to Save File
            Serialize(m_savePathDoors, data);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    // Method that handles Loading of the doors
    public static DoorData LoadDoors()
    {
        if (File.Exists(m_savePathDoors))
        {
            // Create a PlayerData
            DoorData data = new DoorData();

            // Load PlayerData from Save File
            data = (DoorData)Deserialize(m_savePathDoors, data);

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + m_savePathDoors);

            return null;
        }
    }

    // Binary stuff saving and loading
    private static void Serialize(string filePath, object objectToOverwrite)
    {
        // Create a BinaryFormatter 
        BinaryFormatter formatter = new BinaryFormatter();

        // Generate a JSON representation of the objectToOverwrite
        string saveData = JsonUtility.ToJson(objectToOverwrite);

        // Create a FileStream using the savePath we'll be serializing
        FileStream stream = new FileStream(filePath, FileMode.Create);

        // Convert object into a JSON string that can be restored later
        {
            // First Parameter is the FileStream created above
            // Second Parameter is the object which we want to serialize
            formatter.Serialize(stream, saveData);
        }

        // Close the FileStream
        stream.Close();
    }

    // Binary stuff saving and loading
    private static object Deserialize(string filePath, object objectToOverwrite)
    {
        if (filePath == null || objectToOverwrite == null)
        {
            return null;
        }

        // Create a BinaryFormatter 
        BinaryFormatter formatter = new BinaryFormatter();

        // Create a FileStream using the savePath we'll be deserializing
        FileStream stream = File.Open(filePath, FileMode.Open);

        // Convert JSON string into a object
        JsonUtility.FromJsonOverwrite(formatter.Deserialize(stream).ToString(), objectToOverwrite);

        // Close the FileStream
        stream.Close();

        return objectToOverwrite;
    }

    // Method which returns true if there is already an existing save file for the player's data
    public static bool CheckIfFileExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}