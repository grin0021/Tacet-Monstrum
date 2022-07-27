using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsInteractable : MonoBehaviour , IInteractable
{
	//Interaction Interface
	[SerializeField]
	float m_holdDist = 2f;
	[SerializeField]
	float m_HoldSpeed = 5000f;
	Interactor m_CurrentInteractor = null;

	

	//another interactable that this can send updates to
	IInteractableListener m_Listener = null;

	Rigidbody m_MyRigidBody = null;

	[SerializeField]
	float StopSpeed = 500f;
    private void Start()
    {
		m_MyRigidBody = GetComponent<Rigidbody>();
		m_Listener = GetComponent<IInteractableListener>();
	}
    public void OnInteract(Interactor interactor)
	{
		if (m_Listener != null)
			m_Listener.OnInteract(interactor);

		m_CurrentInteractor = interactor;


	}

	public void UpdateHoldInteract(Interactor interactor)
	{
		if (m_Listener != null)
			m_Listener.UpdateHoldInteract(interactor);
	}

	public void FixedUpdateHoldInteract(Interactor interactor)
	{
		if (m_Listener != null)
			m_Listener.FixedUpdateHoldInteract(interactor);

		m_MyRigidBody.velocity = Vector3.ClampMagnitude(m_MyRigidBody.velocity, 10);

		m_MyRigidBody.angularVelocity = Vector3.zero;
		Vector3 targetPos = interactor.GetLookTransform().position + interactor.GetLookTransform().forward * m_holdDist;

		Vector3 DownForce = Vector3.down * 2000f * (Mathf.Max(0,Vector3.Dot(Vector3.down,interactor.GetLookTransform().forward)));

		Vector3 relative = (targetPos - transform.position);
		Vector3 vel = m_MyRigidBody.velocity;
		
		float sqrMag = relative.sqrMagnitude;

		Vector3 force = (relative * m_HoldSpeed) - vel * StopSpeed;

		m_MyRigidBody.AddForce(force);
		m_MyRigidBody.AddForce(DownForce);

		
		//m_MyRigidBody.AddForce((dist - mag) * Relative * m_HoldSpeed);




		//m_MyRigidBody.AddForce((dist - mag) * 1000f * Relative.normalized);
		//m_MyRigidBody.transform.position = Vector3.Lerp(m_MyRigidBody.transform.position, TargetPos, m_HoldSpeed * Time.deltaTime);



	}

	public void OnEndInteract(Interactor interactor)
	{
		if (m_Listener != null)
			m_Listener.OnEndInteract(interactor);


		m_CurrentInteractor = null;
	}

	public void Drop()
    {
		if (m_CurrentInteractor != null)
        {
			m_CurrentInteractor.StopInteracting();
        }
    }
}
