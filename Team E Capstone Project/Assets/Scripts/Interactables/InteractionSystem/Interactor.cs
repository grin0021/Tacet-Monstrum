/*
Author: Jacob Kaplan-Hall
Summary: base interactor script for interface interaction
Update Log:
17/11/21: created script
2/12/22 improved mouse over accuracy with small objects
*/

/*
Author: Bryson Bertuzzi
Comment/Description: base interactor script for interface interaction
ChangeLog:
08/12/2021: Fixed issue with script not working in scene loading
19/01/2021: Fixed naming issue with player
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
[RequireComponent (typeof(PlayerMovementScript))]
public class Interactor : MonoBehaviour
{
    [SerializeField]
    Transform m_LookTransform = null;
    [SerializeField]
    Rigidbody m_RigidBody = null;
    [SerializeField]
    PlayerController m_Player = null;
    [SerializeField]
    float m_Range = 3f;
    [SerializeField]
    float m_Radius = .5f;
    IInteractable m_CurrentInteractable = null;
    IInteractable m_CurrentHighlightedInteractable = null;


    bool m_bInteracting = false;

    MouseKeyPlayerController Controller;
    void Start()
    {


        m_LookTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        m_RigidBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();

        if (!m_LookTransform)
        {
            //dont run if we dont have a look transform
            Debug.LogError("Interactor on " + gameObject.name + " has no look transform!");
            enabled = false;
        }


        Controller = GetComponent<PlayerMovementScript>().Controller;
    }
    // Update is called once per frame
    void Update()
    {
        //if we are not interacting at the moment, highlight interactables
        if (m_CurrentInteractable == null)
        {

            //look for interactables in front of the player 
            RaycastHit HitInfo;
            Physics.Raycast(m_LookTransform.position, m_LookTransform.forward, out HitInfo, m_Range,1 << 0 | 1 << 11);
            
            IInteractable NewInteractable = null;

            if (HitInfo.collider != null)
            {
                NewInteractable = HitInfo.collider.gameObject.GetComponent<IInteractable>();
            }

            //if (NewInteractable == null)
			//{
            //    RaycastHit HitInfo2;
            //    Physics.SphereCast(m_LookTransform.position, m_Radius, m_LookTransform.forward, out HitInfo2, m_Range);
            //    if (HitInfo2.collider != null)
            //    {
            //        NewInteractable = HitInfo2.collider.gameObject.GetComponent<IInteractable>();
            //
            //    }
            //}


            //if something has changed about the thing we are hovering over
            if (m_CurrentHighlightedInteractable != NewInteractable)
            {
                //if we got one in our sights
                if (NewInteractable != null)
                {
                    //set the highlighted interactable
                    m_CurrentHighlightedInteractable = NewInteractable;
                    Debug.Log("Interactor " + name + " started hovering over " + m_CurrentHighlightedInteractable);
                    //enable hover image
                    if (m_Player)
                        m_Player.GetUIAppear().ShowHandGrabUI();
                }
                else
                {
                    //if we already are null just ignore it
                    if (m_CurrentHighlightedInteractable != null)
                    {
                        Debug.Log("Interactor " + name + " stopped hovering over Interactable");
                        //if we didnt get anything
                        m_CurrentHighlightedInteractable = null;
                        //disable hover image
                        if (m_Player)
                            m_Player.GetUIAppear().HideHandGrabUI();
                    }
                }
            }
            
        }
        else
        {
            //if we are interacting at the moment
            Debug.Log("Interactor " + name + " interacting with " + m_CurrentInteractable);
            m_CurrentInteractable.UpdateHoldInteract(this);
        }


        //take input
        if (m_bInteracting == false && Controller.IsPickingUp() == true)
        {
            m_bInteracting = true;
            //if we have an interactable highlighted
            if (m_CurrentHighlightedInteractable != null)
            {
                //set the interactable we are interacting with
                m_CurrentInteractable = m_CurrentHighlightedInteractable;
                //start interaction with interactable
                m_CurrentInteractable.OnInteract(this);

                //UI
                if (m_Player)
				{
                    m_Player.GetUIAppear().ShowClosedHandUI();
				}
            }
        }
        else if (m_bInteracting == true && Controller.IsPickingUp() == false)
        {
            m_bInteracting = false;
            StopInteracting();
        }
    }
    private void FixedUpdate()
    {
        if (m_CurrentInteractable != null)
            m_CurrentInteractable.FixedUpdateHoldInteract(this);
    }
    public Transform GetLookTransform()
    {
        return m_LookTransform;
    }
    public Rigidbody GetRigidBody()
    {
        return m_RigidBody;
    }
    public void StopInteracting()
    {
        //end interaction with interactable if we have one
        if (m_CurrentInteractable != null)
        {
            m_CurrentInteractable.OnEndInteract(this);
            m_CurrentInteractable = null;

            if (m_Player)
			{
                m_Player.GetUIAppear().HideClosedHandUI();
                m_Player.GetUIAppear().ShowHandGrabUI();
			}
            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(m_LookTransform.position, m_LookTransform.position + m_LookTransform.forward * m_Range);
        //Gizmos.DrawSphere(m_LookTransform.position + m_LookTransform.forward * m_Range,m_Radius);
    }




}