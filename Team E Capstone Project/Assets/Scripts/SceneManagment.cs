// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 01/11/2021

/*
Author: Matthew Beaurivage
Date Created: 08/12/2021
Comment/Description: Loads and Unloads Scene when the Loaders tell it to
ChangeLog:
08/12/2021: Added Scene Loading
08/12/2021: Added Scnen Unloding
08/12/2021: Commented the code
*/

/*
Author: Aviraj Singh Virk
Date Created: 08/12/2021
Comment/Description: Loads and Unloads Scene when the Loaders tell it to
ChangeLog:
19/01/2022: Adding the logic for loading screen when changing between 2 scenes
01/02/2022: Made another separate IEnumerator function for loading screen
03/02/2022: Refined the logic for Loading
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagment : MonoBehaviour
{
    private string m_sceneToLoad;
    private string m_sceneToUnload;

    public List<string> m_loadedScenes;

    // Loading Screen
    public GameObject m_LoadingCanvas;
    public float m_MinLoadTime;
    public Slider m_LoadingSlider;
    public bool m_isLoadScreen;

    public void Start()
    {
        m_loadedScenes.Add("Empty");
    }

    // Func that allows the loaders to tell this that it need to load a scene
    public void LoadScene(string sceneToLoad)
    {
        // recives a scene name from the loader 
        m_sceneToLoad = sceneToLoad;
        
        // Trigger Loading Screen
        if (m_isLoadScreen == true)
        {
            StartCoroutine(LoadScreen());
        }
        // Loads the desired scene
        StartCoroutine(LoadAsyncScene());
    }

    // Func that allows the loader to tell this that it need to unload a scene
    public void UnloadScene(string sceneToUnload)
    {
        // recives a scene name from the loader
        m_sceneToUnload = sceneToUnload;
        // Unloads the desired scene
        StartCoroutine(UnloadAsyncScene());
    }

    // Func that loads the desired level asyncronisly
    private IEnumerator LoadAsyncScene()
    {
        bool canLoad = false;
        for (int i = 0; i < m_loadedScenes.Count; i++)
        {
            // if any of the loaded scenes match the one we're
            // looking to load don't load it
            if (m_loadedScenes[i] == m_sceneToLoad)
            {
                canLoad = false;
            }
            else
            {
                canLoad = true;
            }
        }

        // If the Scene is already loaded Don't load it again
        if (canLoad && !SceneManager.GetSceneByName(m_sceneToLoad).isLoaded)
        {
            // The Application loads the Scene Additivly in the background as the current Scene runs.
            AsyncOperation m_aSyncLoad = SceneManager.LoadSceneAsync(m_sceneToLoad, LoadSceneMode.Additive);
            m_loadedScenes.Add(m_sceneToLoad);

            // Wait until the asynchronous scene fully loads
            while (!m_aSyncLoad.isDone)
            {
                yield return null;
            }
        }
    }

    // Func that unloads the desired level asyncronisly
    private IEnumerator UnloadAsyncScene()
    {
        for (int i = 0; i < m_loadedScenes.Count; i++)
        {
            if (m_loadedScenes[i] == m_sceneToUnload)
            {
                // If the Scene is already Unloaded Don't Unload it
                if (SceneManager.GetSceneByName(m_sceneToUnload).isLoaded)
                {
                    // The Application loads the Scene in the background as the current Scene runs.
                    AsyncOperation m_aSyncLoad = SceneManager.UnloadSceneAsync(m_sceneToUnload);
                    m_loadedScenes.RemoveAt(i);
                    // Wait until the asynchronous scene fully loads
                    while (!m_aSyncLoad.isDone)
                    {
                        yield return null;
                    }
                }
            }
        }
    }

    // Func that triggers load screen
    private IEnumerator LoadScreen()
    {
        // Setting Loading Canvas to show up
        m_LoadingCanvas.SetActive(true);

        // Temporary variable for elapsed time
        float elapsedTime = 0.0f;

        // Loop until the elapsed time crosses the minimum load time
        while (elapsedTime < m_MinLoadTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / m_MinLoadTime);
            m_LoadingSlider.value = progress;

            // If the timer expired but the scene didn't load, reset timer
            if (elapsedTime >= m_MinLoadTime && SceneManager.GetSceneByName(m_sceneToLoad).isLoaded == false)
            {
                elapsedTime = 0.0f;
            }

            m_isLoadScreen = false;
            yield return null;
        }

        m_LoadingCanvas.SetActive(false);
    }
}
