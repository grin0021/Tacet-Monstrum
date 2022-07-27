// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Jenson Portelance
Comment/Description: Script that handles Inventories within the project
ChangeLog:
20/10/2021: Initial creation of script, added AddItem and InventorySlot Class
04/11/2021: Massive overhaul, changed inventory to use an Array instead of a list (Added Inventory Class to do this), added MoveItem, RemoveItem and SetEmptySlot
08/11/2021: Added text variable to InventorySlot
11/11/2021: Added AudioClip variable to InventorySlot. Changed code to match coding standards and added more comments.
*/

/*
Author: Bryson Bertuzzi
Comment/Description: Script that handles Inventories within the project
ChangeLog:
21/10/2021: Added GetInventory and SetInventory functions
03/11/2021: Fixed GetInventory and SetInventory functions 
14/11/2021: Added null checking for components
*/

/*
Author: Aviraj Singh Virk
Comment/Description: Script that handles Inventories within the project
ChangeLog:
10/12/2021: Added comments for functions and variables
04/03/2022: Added comments and refined code
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class InventoryObject : ScriptableObject
{
    public ItemDatabaseObject Database;                                         // Reference to our project's item Datbase
    public Inventory Inventory;                                                 // Reference to the Inventory we are handling   
    public bool InventoryFull = false;

    // Called when the Game loads up, Before Start is called
    private void Awake()
    {
        // Fetch components on the same gameObject and null check them
        {
            //if (Database == null)
            //{
            //    Debug.LogError("Missing Item Database Object", this);
            //}

            //if (Inventory == null)
            //{
            //    Debug.LogError("Missing Inventory", this);
            //}
        }
    }

    // Adding Item in the Inventory
    public void AddItem(Item item, int amount)
    {
        // Check if we already have the object being added
        for (int i = 0; i < Inventory.Items.Length; i++)
        {
            // We have a match, item is already in our inventory
            if (Inventory.Items[i].ID == item.Id)
            {
                // Add to the amount of that object we have
                Inventory.Items[i].AddAmount(amount);
                return;
            }
        }
        // There was no match, so fill the first empty slot with our new item
        SetEmptySlot(item, amount);
    }

    // Setting empty slot
    public InventorySlot SetEmptySlot(Item item, int amount)
    {
        // Check through each inventory slot
        for (int i = 0; i < Inventory.Items.Length; i++)
        {
            // An empty inventory Slot is found
            if (Inventory.Items[i].ID <= -1)
            {
                // Update that empty slot with our new item
                Inventory.Items[i].UpdateSlot(item.Id, item, amount, item.RestoreValue, item.Text, item.AudioClip);
                return Inventory.Items[i];
            }
        }
        // Inventory is full (NOT SETUP YET)
        return null;
    }

    // Checks if all the inventory slots are already full
    public bool IsInventoryFull()
    {
        // Check through each inventory slot
        for (int i = 0; i < Inventory.Items.Length; i++)
        {
            // An empty inventory Slot is found
            if (Inventory.Items[i].ID <= -1)
            {
                return false;
            }
        }
        return true;
    }

    // Move an item
    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        // Store second item into a temporary Inventory Slot
        InventorySlot temp = new InventorySlot(item2.ID, item2.Item, item2.Amount, item2.Item.RestoreValue, item2.Item.Text, item2.Item.AudioClip);
        // Swap the position in the inventory of the two items
        item2.UpdateSlot(item1.ID, item1.Item, item1.Amount, item1.Item.RestoreValue, item1.Item.Text, item1.Item.AudioClip);
        item1.UpdateSlot(temp.ID, temp.Item, temp.Amount, temp.Item.RestoreValue, temp.Item.Text, temp.Item.AudioClip);
    }

    // Remove specific item from the inventory
    public void RemoveItem(Item item)
    {
        // Check through each inventory slot
        for (int i = 0; i < Inventory.Items.Length; i++)
        {
            // We are at the slot with the item that was passed in
            if (Inventory.Items[i].Item == item)
            {
                // Update the slot to contain nothing
                Inventory.Items[i].UpdateSlot(-1, new Item(), 0, 0, "", null);
            }
        }
    }

    // Returing Inventory Object
    public Inventory GetInventory()
    {
        return Inventory;
    }

    // Setting Inventory Object
    public void SetInventory(Inventory newslots)
    {
        for (int i = 0; i < newslots.Items.Length; i++)
        {
            Inventory.Items[i].UpdateSlot(newslots.Items[i].ID, newslots.Items[i].Item, newslots.Items[i].Amount, newslots.Items[i].Item.RestoreValue,
                                           newslots.Items[i].Item.Text, newslots.Items[i].Item.AudioClip);
        }
    }

    // Making a new inventory object
    [ContextMenu("Clear")]
    public void Clear()
    {
        Inventory = new Inventory();
    }
}

// Basic class of an Inventory
[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[8];                            // Array of InventorySlots called Items, used by Inventory objects
}

// Base class of an Inventory Slot
[System.Serializable]
public class InventorySlot
{
    public int ID = -1;                        // Unique ID of the item in the inventory Slot
    public Item Item;                          // The Item itself
    public int Amount;                         // The amount of that item there is  
    public int RestoreValue;                   // The amount this item restores to the player (if it does)
    public string Text;                        // Text contained within the item, used for documents and Audio recordings
    public AudioClip AudioClip;                // Audio Clip contained within the item, used for Audio recordings
    
    // Contructor
    public InventorySlot()
    {
        ID = -1;
        Item = null;
        Amount = 0;
        RestoreValue = 0;
    }

    // Overloaded Constructor
    public InventorySlot(int id, Item item, int amount, int restoreValue, string text, AudioClip clip)
    {
        ID = id;
        Item = item;
        Amount = amount;
        RestoreValue = restoreValue;
        Text = text;
        AudioClip = clip;
    }

    // Updating slots in the inventory
    public void UpdateSlot(int id, Item item, int amount, int restoreValue, string text, AudioClip clip)
    {
        ID = id;
        Item = item;
        Amount = amount;
        RestoreValue = restoreValue;
        Text = text;
        AudioClip = clip;
    }

    // Increasing the amount of items
    public void AddAmount(int value)
    {
        Amount += value;
    }
}
