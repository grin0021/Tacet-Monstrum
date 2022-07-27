// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 01/11/2021

/*
Author: Matthew Beaurivage
Date Created: 07/12/2021
Comment/Description: Detects whent the player is inside the trigger box and lods or unloads depending on the result
ChangeLog:
07/12/2021: Added Scene Loading
07/12/2021: Added Scnen Unloding
08/12/2021: Changed Loading to tell the level Management script to handle it
08/12/2021: Commented the code
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public string m_sceneName;              // Name of the Scene this Loader is responsible for
    public SceneManagment m_sceneManager;   // reference to the Scene Managment script

    public int m_triggerCount;
    public bool bIsLoading;

    public void Start()
    { 
        m_triggerCount = 0;
        bIsLoading = false;
    }

    public void UpdateScene()
    {
        if(ShouldLoadScene())
        {
            m_sceneManager.LoadScene(m_sceneName);
        }
        else
        {
            m_sceneManager.UnloadScene(m_sceneName);
        }
    }

    public bool ShouldLoadScene()
    {
        if(m_triggerCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
