// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 17/11/2021

/*
Author: Aviraj Singh Virk
Comment/Description: Script controls Sound for the Scene
ChangeLog:
17/11/2021: Script created
08/12/2021: Adding comments and changelog
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SceneSoundManager : MonoBehaviour
{
    private AudioSource[] m_audioSources;       // Array of all audio clips
    private AudioSource m_sceneAudio;           // Reference to current Scene Audio
    private AudioSource m_sceneEffectAudio;     // Reference to Scene Effect audio in scene
    private AudioSource m_musicAudio;           // Reference to current Scene music

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Values
        m_audioSources = GetComponents<AudioSource>();

        m_sceneAudio = m_audioSources[0];
        m_sceneEffectAudio = m_audioSources[1];
        m_musicAudio = m_audioSources[2];

        m_sceneAudio.enabled = false;
        m_sceneEffectAudio.enabled = false;
        m_musicAudio.enabled = false;
    }

    // Called when setting a Background Audio
    public void SetBackgroundAudio(AudioClip audioClip, bool shouldLoop, float fadeTime)
    {
        m_sceneAudio.clip = audioClip;
        m_sceneAudio.volume = 1.0f;
        m_sceneAudio.pitch = 1.0f;
        m_sceneAudio.panStereo = 0.0f;
        m_sceneAudio.loop = shouldLoop;

        StartCoroutine(FadeIn(m_sceneAudio, fadeTime, m_sceneAudio.volume));
    }

    // Overloaded function for Setting a background audio
    public void SetBackgroundAudio(AudioClip audioClip, float volume, bool shouldLoop, float fadeTime)
    {
        m_sceneAudio.clip = audioClip;
        m_sceneAudio.volume = volume;
        m_sceneAudio.pitch = 1.0f;
        m_sceneAudio.panStereo = 0.0f;
        m_sceneAudio.loop = shouldLoop;

        StartCoroutine(FadeIn(m_sceneAudio, fadeTime, m_sceneAudio.volume));
    }

    // Overloaded function for Setting a background audio
    public void SetBackgroundAudio(AudioClip audioClip, float volume, float pitch, float stereoPan, bool shouldLoop, float fadeTime)
    {
        m_sceneAudio.clip = audioClip;
        m_sceneAudio.volume = volume;
        m_sceneAudio.pitch = pitch;
        m_sceneAudio.panStereo = stereoPan;
        m_sceneAudio.loop = shouldLoop;

        StartCoroutine(FadeIn(m_sceneAudio, fadeTime, m_sceneAudio.volume));
    }

    // Function for setting the Scene Effect Sound
    public void SetSoundEffectAudio(AudioClip audioClip)
    {
        m_sceneEffectAudio.clip = audioClip;
        m_sceneEffectAudio.volume = 1.0f;
        m_sceneEffectAudio.pitch = 1.0f;
        m_sceneEffectAudio.panStereo = 0.0f;

        PlaySoundEffect();
    }

    // Overloaded function for setting Scene Effect sound
    public void SetSoundEffectAudio(AudioClip audioClip, float volume)
    {
        m_sceneEffectAudio.clip = audioClip;
        m_sceneEffectAudio.volume = volume;
        m_sceneEffectAudio.pitch = 1.0f;
        m_sceneEffectAudio.panStereo = 0.0f;

        PlaySoundEffect();
    }

    // Overloaded function for setting Scene Effect sound
    public void SetSoundEffectAudio(AudioClip audioClip, float volume, float pitch, float stereoPan)
    {
        m_sceneEffectAudio.clip = audioClip;
        m_sceneEffectAudio.volume = volume;
        m_sceneEffectAudio.pitch = pitch;
        m_sceneEffectAudio.panStereo = stereoPan;

        PlaySoundEffect();
    }

    // Function for setting the Music audio
    public void SetMusicAudio(AudioClip audioClip, bool shouldLoop, float fadeTime)
    {
        m_musicAudio.clip = audioClip;
        m_musicAudio.volume = 1.0f;
        m_musicAudio.pitch = 1.0f;
        m_musicAudio.panStereo = 0.0f;
        m_musicAudio.loop = shouldLoop;

        StartCoroutine(FadeIn(m_musicAudio, fadeTime, m_musicAudio.volume));
    }

    // Overloaded function for setting Music Audio
    public void SetMusicAudio(AudioClip audioClip, float volume, bool shouldLoop, float fadeTime)
    {
        m_musicAudio.clip = audioClip;
        m_musicAudio.volume = volume;
        m_musicAudio.pitch = 1.0f;
        m_musicAudio.panStereo = 0.0f;
        m_musicAudio.loop = shouldLoop;

        StartCoroutine(FadeIn(m_musicAudio, fadeTime, m_musicAudio.volume));
    }

    // Overloaded function for setting Music Audio
    public void SetMusicAudio(AudioClip audioClip, float volume, float pitch, float stereoPan, bool shouldLoop, float fadeTime)
    {
        m_musicAudio.clip = audioClip;
        m_musicAudio.volume = volume;
        m_musicAudio.pitch = pitch;
        m_musicAudio.panStereo = stereoPan;
        m_musicAudio.loop = shouldLoop;

        StartCoroutine(FadeIn(m_musicAudio, fadeTime, m_musicAudio.volume));
    }

    // Function that plays the Sound passed
    public void PlaySoundEffect()
    {
        m_sceneEffectAudio.enabled = true;
        m_sceneEffectAudio.Play();
    }

    // Called when to stop the onplaying Audio
    public void StopAudio(float fadeTime)
    {
        StartCoroutine(FadeOut(m_sceneAudio, fadeTime));
    }

    // Called when to stop the onplaying Sound Effect
    public void StopSoundEffect()
    {
        m_sceneEffectAudio.Stop();
        m_sceneEffectAudio.enabled = false;
    }

    // Called when to stop the onplaying Music in Background
    public void StopMusic(float fadeTime)
    {
        StartCoroutine(FadeOut(m_musicAudio, fadeTime));
    }

    // Coroutine for sound to Fade Out
    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.enabled = false;
        audioSource.volume = startVolume;
    }

    // Coroutine for sound to Fade In
    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime, float maxVolume)
    {
        audioSource.enabled = true;

        float startVolume = 0.2f;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < maxVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }
        audioSource.volume = maxVolume;
    }
}
