// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 06/11/2021

/*
Author: Jenson Portelance
Comment/Description: Script that serves as the parent of all key objects
ChangeLog:
06/11/2021: Initial creation of script
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SanityKit Object", menuName = "Inventory/Items/Sanity Kit")]
public class SanityKitObject : ItemObject
{
    public void Awake()
    {
        Type = EItemType.SanityKit;
    }
} 
