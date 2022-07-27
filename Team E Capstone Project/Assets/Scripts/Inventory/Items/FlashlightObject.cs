using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Flashlight Object", menuName = "Inventory/Items/Flashlight")]
public class FlashlightObject : ItemObject
{
    public void Awake()
    {
        Type = EItemType.Flashlight;
    }
}
