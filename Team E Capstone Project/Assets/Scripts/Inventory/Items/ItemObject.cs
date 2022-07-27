// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Jenson Portelance
Comment/Description: Script that serves as the parent of all item objects within the game
ChangeLog:
20/10/2021: Initial creation of script, added Key and tool to Enum
04/11/2021: Added restore value, added Health kit and Sanity Kit to Item Type Enum
08/11/2021: Added Document to Enum
11/11/2021: Added audio clip, added AudioRecording to Enum. Changed code to match coding standards and added more comments.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EItemType
{
    Key,
    Tool,
    Item,
    HealthKit,
    SanityKit,
    Document,
    AudioRecording,
    Flashlight,
    WalkieTalkie,
    Default
}

public abstract class ItemObject : ScriptableObject
{
    public int Id;
    public Sprite UIDisplay;
    public EItemType Type;
    [TextArea(10, 15)]
    public string Description;
    public int RestoreValue;
    public AudioClip AudioClip;
}

[System.Serializable]
public class Item
{
    public string Name;
    public int Id;
    public int RestoreValue;
    public string Text;
    public AudioClip AudioClip;
    public Item()
    {
        Name = "";
        Id = -1;
        RestoreValue = 0;
        Text = "";
        AudioClip = null;
    }
    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.Id;
        RestoreValue = item.RestoreValue;
        Text = item.Description;
        AudioClip = item.AudioClip;
    }
}
