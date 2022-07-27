using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Description: Simple script to help create a more dynamic patrol for the monster
ChangeLog:
16/03/2022: Script created, returns are random point from a list of adjacent points to create a more dynamic patrol for the AI
07/04/2022: Added comments
 */

public class DynamicPatrol : MonoBehaviour
{
    [Header("Point References")]                 
    public List<GameObject> AdjacentPoints;         // References to adjacent patrol points

    public GameObject GetRandomPoint(GameObject lastPoint)
    {
        int randomNum = 0;

        do
        {
            // Chose point randomly from list of adjacent points
            if (AdjacentPoints.Count > 1)
            {
                randomNum = Random.Range(0, AdjacentPoints.Count);
            }
            else
            {
                randomNum = 0;
            }
        } while (AdjacentPoints[randomNum] == lastPoint && AdjacentPoints.Count > 1);
        
        // Return chosen point
        return AdjacentPoints[randomNum];
    }
}
