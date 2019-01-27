using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	enum PlayerState {
		Normal,
		Picking,
		Damaged
	};
	const float SPEED = 25f;
	const float SLOW_DURATION = 0.25f;
	const float MAX_PICKUP_DISTANCE = 2f;
	const float PICKING_TIME = 1.5f;

	private PlayerState m_state;
	
	private Rigidbody m_body;
	private Animator m_animator;
	private BoxCollider m_collider;
	private GameObject m_mushroomPosition;

	private float m_slowStartTime;

	private RaycastHit m_pickingShroom;
	private float m_pickStartTime;
	private static Stack<GameObject> m_pickedMushrooms;

	void Start () {
		m_body = GetComponent<Rigidbody>();
		m_animator = GetComponentInChildren<Animator>();
		m_collider = GetComponent<BoxCollider>();
		m_mushroomPosition = GameObject.Find( "MushroomPosition" );
		SetState( PlayerState.Normal );
	}

	void Throw () {
		/*GameObject mushroom = Instantiate( Mushroom, body.transform.position, body.transform.rotation );
		Mushroom mushScript = mushroom.GetComponent<Mushroom>();
		mushScript.SetVelocity( body.velocity, new Vector2( direction.x, direction.y ) );*/
	}

	void Pick () {
		RaycastHit hit;
		bool hitDetect = Physics.BoxCast( m_collider.bounds.center, transform.localScale, transform.forward, out hit, transform.rotation, MAX_PICKUP_DISTANCE );
		if( !hitDetect ) {
			return;
		}

		GameObject gameObject = hit.collider.gameObject;
		bool hitIsMushroom = gameObject.tag == "Mushroom";
		if( !hitIsMushroom ) {
			return;
		}

		Mushroom mushroomScript = gameObject.GetComponent<Mushroom>();
		if( mushroomScript.State != Mushroom.MushroomState.Idle )
		{
			return;
		}
		SetState( PlayerState.Picking );
		m_pickingShroom = hit;
	}

	void UpdateIdle () {
		// Handle player movement
		Vector3 input = new Vector3( Input.GetAxisRaw( "Horizontal" ), 0, Input.GetAxisRaw( "Vertical" ) );
		Vector3 velocity = input * SPEED;
		// Slow player movement if there is no input.
		if( input.x == 0 && input.z == 0 && m_body.velocity != Vector3.zero ) {
			m_slowStartTime = m_slowStartTime == -1 ? Time.time : m_slowStartTime;
			float slowElapsedTime = Time.time - m_slowStartTime;
			m_body.velocity = Vector3.Lerp( m_body.velocity, Vector3.zero, slowElapsedTime / SLOW_DURATION );
			m_animator.SetFloat( "MoveSpeed", Mathf.Max( 1 - ( slowElapsedTime / SLOW_DURATION ), 0 ) );
		}
		else {
			// TODO: Probably want some sort of lerp here.
			Vector3 newDir = Vector3.RotateTowards( transform.forward, input, 90, 0.0f );
			transform.rotation = Quaternion.LookRotation( newDir );

			m_body.velocity = velocity;
			m_slowStartTime = -1;
			if( input != Vector3.zero ) {
				// TODO: Walk support ???
				m_animator.SetFloat( "MoveSpeed", 1 );
			}
		}
	}


	void OnExitPicking() {
		m_pickStartTime = -1;
		GameObject pickedShroom = m_pickingShroom.collider.gameObject;
		m_pickedMushrooms.Push( pickedShroom );
		Mushroom mushroomScript = gameObject.GetComponent<Mushroom>();
		mushroomScript.SetState( Mushroom.MushroomState.Picked );
		int headMushCount = m_pickedMushrooms.Count;
		float mushroomHeight = headMushCount * m_pickingShroom.collider.bounds.extents.y;
		Vector3 mushroomPosition = m_mushroomPosition.transform.position;
		pickedShroom.transform.SetParent( transform );
		pickedShroom.transform.position = new Vector3( mushroomPosition.x, mushroomPosition.y + mushroomHeight, mushroomPosition.z );
	}

	void UpdatePicking() {
		float pickElapsedTime = Time.time - m_pickStartTime;
		if( pickElapsedTime > PICKING_TIME ) {
			SetState( PlayerState.Normal );
		}
	}

	void SetState ( PlayerState state ) {
		if( m_state != state ) {
			OnExitState( m_state );
			OnEnterState( state );
			m_state = state;
		}
	}

	void OnEnterState ( PlayerState state ) {
		switch( state ) {
			case PlayerState.Normal:
				break;
			case PlayerState.Picking:
				m_pickStartTime = Time.time;
				break;
			case PlayerState.Damaged:
				break;
		}
	}

	void OnExitState ( PlayerState state ) {
		switch( state ) {
			case PlayerState.Normal:
				break;
			case PlayerState.Picking:
				OnExitPicking();
				break;
			case PlayerState.Damaged:
				break;
		}
	}

	void UpdateState () {
		switch( m_state ) {
			case PlayerState.Normal:
				UpdateIdle();
				break;
			case PlayerState.Picking:
				UpdatePicking();
				break;
			case PlayerState.Damaged:
				break;
		}
	}

	private void FixedUpdate() {
		UpdateState();
	}
}
