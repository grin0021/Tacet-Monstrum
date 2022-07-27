// Copyright (c) DeepSilentStudio Ltd. 2021. All Rights Reserved.

/*
Author: Bryson Bertuzzi
Date Created: 12/11/2021
Comment/Description: Holds common properties for all Destructible Objects
ChangeLog:
15/11/2021: Added Destroying based on velocity to Destructible Objects
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructibles : Interactables
{
    [Header("Physics")]
    public float Strength = 1.0f;       // How strong is the object from breaking
    public float BreakForce = 10.0f;    // How much force was used to break the object
  
    [Header("Destroyed Object")]
    public GameObject DestroyedVersion; // Broken parts that break off from the main object

    private Rigidbody m_rigidBody;      // Reference to objects Rigidbody
    private bool m_bisBroken = false;   // Is the object broken

    // Start is called before the first frame update
    void Start()
    {
        if (!(m_rigidBody = GetComponent<Rigidbody>()))
        {
            Debug.LogError("Missing Rigidbody Component", this);
        }

        if (DestroyedVersion == null)
        {
            Debug.LogError("Missing Destroyed Object Prefab", this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // BreakObject is called when the GameObject is destroyed
    public void BreakObject()
    {
        // Spawn Destroyed Version of Object into Scene
        GameObject fracturedObject = Instantiate(DestroyedVersion, transform.position, transform.rotation);

        Rigidbody[] rigidBodies = fracturedObject.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigidBodies.Length; i++)
        {
            Vector3 force = (rigidBodies[i].transform.position - transform.position).normalized * BreakForce;
            rigidBodies[i].AddForce(force);
        }

        // Delete Non-Destroyed GameObject in Scene
        Destroy(gameObject);
    }

    // OnCollisionEnter is called when colliding with another object
    private void OnCollisionEnter(Collision collision)
    {
        // If the length of velocity is greater than the strength of the object and is not already broken...
        if (m_rigidBody.velocity.magnitude > Strength && m_bisBroken == false)
        {
            // Set the object as broken
            m_bisBroken = true;

            // Break the object
            BreakObject();
        }
    }
}