// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

// CHANGE LOG
// DATE FORMAT: DD/MM/YYYY
// DATE CREATED: 30/11/2021

/*
Author: Bryson Bertuzzi
Date Created: 24/02/2022
Comment/Description: Material Swaping Script
ChangeLog:
24/02/2022: Script created
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwap : MonoBehaviour
{
    public Material FirstMaterial;        // Reference for original material of object
    public Material SecondMaterial;       // Reference for lit/damaged material of object
    private bool m_bSwapMaterials = true; // Bool for checking if the materials can swap
    private Renderer m_renderer;          // Reference for the object's renderer
    public MaterialSwap Partner;          // Reference for partner MaterialSwap to allow paring of functions 

    // Start is called before the first frame update
    void Start()
    {
        // Initlaize Values
        m_renderer = GetComponent<Renderer>();
    }

    // OnTriggerEnter is called when another object enters the trigger
    void OnTriggerEnter(Collider other)
    {
        // If the Player is the one activating it...
        if (other.gameObject.tag == "Player")
        {
            // Swap the materials
            SwapMaterials();
        }
    }

    // SwapMaterials is called when swapping materials
    public void SwapMaterials()
    {
        // If swaping materials is true...
        if (m_bSwapMaterials == true)
        {
            // Set swapping materials to false
            m_bSwapMaterials = false;
            // Set current material to SecondMaterial
            m_renderer.material = SecondMaterial;
        }
    }

    // ResetMaterials is called when swapping back materials
    public void ResetMaterials()
    {
        // Set current material to FirstMaterial
        m_renderer.material = FirstMaterial;

        // If partner is available...
        if (Partner != null)
        {
            // Set partner material swap to false
            Partner.SetMaterialSwap(false);
            // Reset partner materials
            Partner.ResetMaterials();
        }
    }

    // Setter for material swap
    public void SetMaterialSwap(bool bswap)
    {
        m_bSwapMaterials = bswap;
    }

    // Getter for material swap
    public bool GetMaterialSwap()
    {
        return m_bSwapMaterials;
    }
}