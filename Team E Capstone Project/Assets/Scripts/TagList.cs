// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Interface for all Inputs
ChangeLog:
11/11/2021: Refined and added 2 new methods
13/11/2021: Added null checks and refined variables names
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Custom Component built to have multiple tags onto one GameObject
public class TagList : MonoBehaviour
{
    public List<string> tags = new List<string>();      // List of strings which will be used as TAGS

    // Called when a tag has to be added through Code
    public void AddTag(string newTag)
    {
        tags.Add(newTag);
    }

    // Called to check if the GameObject has the Particular tag
    public bool HasTag(string checkTag)
    {
        return tags.Contains(checkTag);
    }

    // Returns the total number of tags
    public int Count()
    {
        return tags.Count;
    }

    // Checks if the list of tags is empty
    public bool IsEmpty()
    {
        if (Count() != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
