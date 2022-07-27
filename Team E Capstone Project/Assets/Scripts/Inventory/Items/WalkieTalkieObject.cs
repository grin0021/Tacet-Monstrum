using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WalkieTalkie Object", menuName = "Inventory/Items/Walkie-Talkie")]
public class WalkieTalkieObject : ItemObject
{
    public void Awake()
    {
        Type = EItemType.WalkieTalkie;
    }
}