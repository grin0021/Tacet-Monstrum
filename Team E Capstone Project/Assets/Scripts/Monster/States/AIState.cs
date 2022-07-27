using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Description: Base class for AI states
ChangeLog: 
10/02/2022: Script created.
07/04/2022: Removed uneccessary move input function
*/

public abstract class AIState
{
    public AIController AIController { get; private set; }     // Reference to owning AIController

    public AIState(AIController aiController)
    {
        // Set owning controller
        AIController = aiController;
    }

    public abstract void Activate();

    public abstract void Deactivate();

    public abstract string GetName();

    public abstract void Update();
}