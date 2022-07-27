// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 08/11/2021

/*
Author: Jenson Portelance
Comment/Description: Script that serves as the parent of all key objects
ChangeLog:
08/11/2021: Initial creation of script
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Document Object", menuName = "Inventory/Items/Document")]
public class DocumentObject : ItemObject
{
    public void Awake()
    {
        Type = EItemType.Document;
    }
}
