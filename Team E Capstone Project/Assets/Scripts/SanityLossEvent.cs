using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;

public class SanityLossEvent : MonoBehaviour
{
    public Volume Volume;
    public Camera Camera;

    AudioLowPassFilter m_lowPass;

    bool m_bBeginEvent = false;
    bool m_bTimeEvent = false;

    float m_fadeValue = 0.0f;
    float m_duration = 5.0f;
    float m_currRotate = 0.0f;
    float m_maxRotate = 1.0f;

    float m_maxFrequency;

    // Start is called before the first frame update
    void Start()
    {
        Volume.enabled = false;

        m_lowPass = GetComponent<AudioLowPassFilter>();
        m_maxFrequency = m_lowPass.cutoffFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bBeginEvent)
        {
            if (Volume.isActiveAndEnabled == false)
            {
                Volume.enabled = true;
            }

            if (m_lowPass.isActiveAndEnabled == false)
            {
                m_lowPass.enabled = true;
            }

            FadeIn();
        }

        if (m_bTimeEvent)
        {
            m_duration -= Time.deltaTime;

            if (m_duration <= 0.0f)
            {
                m_bBeginEvent = false;

                FadeOut();
            }
        }

        if (m_duration > 0.0f && m_bBeginEvent)
        {
            m_currRotate = Mathf.Lerp(m_currRotate, m_maxRotate, Time.deltaTime);
            //Camera.transform.parent.Rotate(new Vector3(m_currRotate, 0.0f, 0.0f), Space.Self);

            //Vector3 angles = Camera.transform.parent.eulerAngles;
            //angles.x += m_currRotate;
            //
            //angles.x = Mathf.Clamp(angles.x, -90.0f, 90.0f);
            //
            //Camera.transform.parent.eulerAngles = angles;
            //
            //Debug.Log(Camera.transform.eulerAngles);
        }
    }

    void FadeIn()
    {
        m_fadeValue += Time.deltaTime;
        m_fadeValue = Mathf.Clamp(m_fadeValue, 0.0f, 1.0f);

        Volume.weight = m_fadeValue;
        m_lowPass.cutoffFrequency = m_maxFrequency * (1 - m_fadeValue);

        m_lowPass.cutoffFrequency = Mathf.Clamp(m_lowPass.cutoffFrequency, 1000.0f, m_maxFrequency);

        if (m_fadeValue >= 1.0f)
            m_bTimeEvent = true;
    }

    void FadeOut()
    {
        m_fadeValue -= Time.deltaTime;
        m_fadeValue = Mathf.Clamp(m_fadeValue, 0.0f, 1.0f);

        Volume.weight = m_fadeValue;
        m_lowPass.cutoffFrequency = m_maxFrequency * (1 - m_fadeValue);

        m_lowPass.cutoffFrequency = Mathf.Clamp(m_lowPass.cutoffFrequency, 1000.0f, m_maxFrequency);

        if (m_fadeValue <= 0.0f)
        {
            Volume.enabled = false;
            m_lowPass.enabled = false;
        }
    }

    public void SetBegin(bool begin)
    {
        m_bBeginEvent = begin;
    }
}