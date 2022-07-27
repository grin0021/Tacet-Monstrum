// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 02/04/2022

/*
Author: Aviraj Singh Virk
Comment/Description: Script handling the playing of cutscenes through triggers
ChangeLog:
02/04/2022: Added basic logic to play a video
03/04/2022: Hooked up script with Triggerboxes
05/04/2022: Added game pausing functionality
09/04/2022: Added new logic for Death Cutscene
12/04/2022: Added logic to turn Jeremy off after Death Cutscene
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerTrigger : MonoBehaviour
{
    public GameObject m_canvas;

    public VideoPlayer m_videoPlayer;

    public bool b_canBeReTriggered;

    UIAppear m_playerUIApper;

    public bool m_checkPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && m_checkPlayer == false)
        {
            m_playerUIApper = other.gameObject.GetComponent<UIAppear>();
            m_playerUIApper.Pause();

            m_canvas.SetActive(true);

            m_videoPlayer.Play();
        }

        if (other.gameObject.tag == "Player" && m_checkPlayer == true)
        {
            if (GameObject.Find("COLLECT_Clipboard_JeremyCode") == null)
            {
                m_playerUIApper = other.gameObject.GetComponent<UIAppear>();
                m_playerUIApper.Pause();

                m_canvas.SetActive(true);

                m_videoPlayer.Play();
            }
        }
    }

    private void Update()
    {
        if (m_canvas.gameObject.activeSelf)
        {
            if (m_videoPlayer.isPlaying == false)
            {
                m_playerUIApper.Resume();

                m_canvas.SetActive(false);

                if (m_checkPlayer == true)
                {
                    GameObject.Find("PR_CHAR_Jeremy").SetActive(false);
                }

                if (b_canBeReTriggered == false)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}
