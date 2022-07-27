// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Jenson Portelance
Comment/Description: Script that serves as the parent of all key objects
ChangeLog:
20/10/2021: Initial creation of script
*/

using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Tool Object", menuName = "Inventory/Items/Tool")]
public class ToolObject : ItemObject
{
    public void Awake()
    {
        Type = EItemType.Tool;
    }
}
