using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Author: Seth Grinstead
Description: Script handling sound void behaviour of the AI
Changelog:
03/03/2022: Basic functionality created. Objects within a radius are searched for lowpass to be applied.
07/04/2022: Optimized script with layer mask. Removed unnecessary code specific to player (Functions same without code)
*/

public class LowPassTrigger : MonoBehaviour
{
    Collider[] m_colliders;                     // Array of colliders within a radius

    public float DefaultFrequency = 5000.0f;    // Default max frequency of lowpass effects
    public float LowestFrequency = 250.0f;      // Lowest frequency allowed for lowpass effects
    public float EffectRadius = 10.0f;          // Effect radius

    private void Update()
    {
        // Find colliders within effect radius
        int layerMask = LayerMask.GetMask("Environment") | LayerMask.GetMask("Monster") | LayerMask.GetMask("Not in Reflection");
        m_colliders = Physics.OverlapSphere(transform.position, EffectRadius, layerMask);

        // If colliders are found
        if (m_colliders.Length > 0)
        {
            // Trigger lowpass effect
            TriggerLowPass();
        }
    }

    void TriggerLowPass()
    {
        // For every collider in array
        foreach (var col in m_colliders)
        {
            // If owning GameObject has an AudioLowPassFilter 
            if (col.GetComponent<AudioLowPassFilter>())
            {
                // Calculate effect frequency based on distance from object
                float dist = Vector3.Distance(transform.position, col.transform.position);
                float freq = (dist / EffectRadius) * DefaultFrequency;
                freq = Mathf.Clamp(freq, LowestFrequency, DefaultFrequency);

                // Apply frequency
                col.gameObject.GetComponent<AudioLowPassFilter>().cutoffFrequency = freq;
            }
        }
    }
}