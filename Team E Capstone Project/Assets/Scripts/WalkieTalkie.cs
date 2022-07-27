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

/*
Author: Bryson Bertuzzi
Comment/Description: Script controlling behaviour of the WalkieTalkie
ChangeLog:
08/11/2021: Script created
15/11/2021: Walkie Talkie can stop and start audio clips
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkieTalkie : Interactables
{
    public AudioClip[] DialogueClips;       // Array storing the Dialogue clips stored on the Walkie talkie
    public AudioSource AudioPlayer;         // Reference to the Audio Source

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components on the same gameObject
        {
            AudioPlayer = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    // Method called to play a clip
    public void PlayClip(int clipNumber)
    {
        AudioPlayer.clip = DialogueClips[clipNumber];
        AudioPlayer.Play();
    }

    // Method to check inputs called from update every frame
    public void CheckInput()
    {
        // If WalkieTalkie is active...
        if (gameObject.activeSelf == true)
        {
            // If "Toggle WalkieTalkie" is pressed...
            if (Input.GetButton("Toggle WalkieTalkie"))
            {
                // Toggle WalkieTalkie
                AudioPlayer.Stop();

                // Play random dialogue
                PlayClip(Random.Range(0, DialogueClips.Length));
            }

            // Debug Stop Audio
            if (Input.GetKeyDown(KeyCode.M))
            {
                AudioPlayer.Stop();
            }
        }
    }
}
