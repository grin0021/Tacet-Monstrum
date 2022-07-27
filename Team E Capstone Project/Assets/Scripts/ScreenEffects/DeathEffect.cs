// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 11/02/2022

/*Author: Aviraj Singh Virk
Comment/Description: Main Player/Character Controller Script
ChangeLog:
04/03/2022: Added Example ChangeLog and added comments
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathEffect : MonoBehaviour
{
    public float FadeRate;          // Dae rate of the effect
    private Image image;            // Background image to show
    private float targetAlpha;      // Alpha of background image for the fade effect
    private PlayerController playerRef;

    // Called at the start of the game
    private void Start()
    {
        if (this.gameObject.GetComponent<Image>() != null)
        {
            image = this.gameObject.GetComponent<Image>();
            Debug.Log("Ur mother");
        }
        else
        {
            Debug.Log("Image is broken as fuck dude");
        }

        playerRef = GameObject.Find("PR_Player").GetComponent<PlayerController>();
    }

    // Coroutine to fade the image out
    public IEnumerator FadeIn()
    {
        image = this.gameObject.GetComponent<Image>();

        float flashDuration = 5 / 2;
        for (float i = 0; i <= flashDuration; i += Time.deltaTime)
        {

            Color frameColor = image.color;
            frameColor.a = Mathf.Lerp(0, 1.5f, i / flashDuration);
            image.color = frameColor;

            // wait for next frame
            yield return null;
        }

        yield return new WaitForSeconds(5.0f);
    }

    public void ExitToMainMenu()
    {
        // Use coroutine to load the Scene in the background
        StartCoroutine(LoadAsyncScene());
    }

    public void LoadGame()
    {
        playerRef.PlayerRevive();
    }

    // LoadAsyncScene is called when loading the Scene in the background
    IEnumerator LoadAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        AsyncOperation aSyncLoad = SceneManager.LoadSceneAsync("LD_MainMenu");

        // Wait until the asynchronous scene fully loads
        while (!aSyncLoad.isDone)
        {
            yield return null;
        }
    }
}
