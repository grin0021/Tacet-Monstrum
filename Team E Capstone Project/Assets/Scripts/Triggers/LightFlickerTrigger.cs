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

public class LightFlickerTrigger : MonoBehaviour
{
    public bool m_isFlickering = false;     // Bool if light is flickering
    public float timeDelay;                 // Time between 2 flickers
    private Material m_lightMat;
    private Color m_color;

    private void Start()
    {
        if (m_lightMat == null)
        {
            m_lightMat = gameObject.GetComponentInParent<Renderer>().material;
        }
        m_color = m_lightMat.GetColor("_EmissiveColor");
        m_lightMat.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isFlickering == false)
        {
            StartCoroutine(FlickeringLight());
        }
    }

    // Coroutine to flicker the lights
    IEnumerator FlickeringLight()
    {
        m_isFlickering = true;
        this.gameObject.GetComponent<Light>().enabled = false;
        timeDelay = Random.Range(0.01f, 0.5f);
        m_lightMat.SetColor("_EmissiveColor", Color.black);
        m_lightMat.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(timeDelay);

        this.gameObject.GetComponent<Light>().enabled = true;
        timeDelay = Random.Range(0.01f, 0.5f);
        m_lightMat.SetColor("_EmissiveColor", m_color);
        m_lightMat.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(timeDelay);
        m_isFlickering = false;
    }
}