// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 20/10/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Script handling various Door Sounds
ChangeLog:
12/11/2021: Added Comments and updated ChangeLog 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightHandling : MonoBehaviour
{
    private int constantFactor = 90;        // Constant factor for calculating the weight

    // Main method calculating the weight of the object according to physics
    public int CalculateObjectWeight(Rigidbody Rbody)
    {
        if (Rbody != null)
        {
            //Uses a basic formula to give an appropriate carry weight to the given object
            //WARNING: Extremely low object mass can result in funky results! Most things don't actually way a single mass unit

            float objectMass = Rbody.mass;
            int objectWeight = GravitySim(objectMass);
            return objectWeight;
        }
        return 0;
    }

    // Returns the gravity value
    int GravitySim(float number)
    {
        int returnResult = constantFactor / (int)number;

        return returnResult;
    }


}
