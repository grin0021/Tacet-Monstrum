// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

/*
Author: Aviraj Singh Virk
Comment/Description: Script handling Obstcales
ChangeLog:
10/12/2021: Added comments and changelog
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int ObstacleUnlockID = -1;       // Integer storing the ID for unlocking the Obstacle
    public bool IsObstacleActive = true;    // Bool if the obstacle is active

    // Method called when the obstacle has to be removed
    public void ClearObstacle()
    {
        IsObstacleActive = false;
        this.gameObject.SetActive(false);
    }
}
