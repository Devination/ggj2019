﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	const float SPEED = 25f;
	const float SLOW_DURATION = 0.25f;
	private float m_slowStartTime;
	private Rigidbody m_body;
	private Animator m_animator;

	void Start () {
		m_body = GetComponent<Rigidbody>();
		m_animator = GetComponentInChildren<Animator>();
	}

	void Throw () {
		/*GameObject mushroom = Instantiate( Mushroom, body.transform.position, body.transform.rotation );
		Mushroom mushScript = mushroom.GetComponent<Mushroom>();
		mushScript.SetVelocity( body.velocity, new Vector2( direction.x, direction.y ) );*/
	}

	void FixedUpdate () {
		// Handle player movement
		Vector3 input = new Vector3( Input.GetAxisRaw( "Horizontal" ), 0, Input.GetAxisRaw( "Vertical" ) );
		Vector3 velocity = input * SPEED;
		// Slow player movement if there is no input.
		if( input.x == 0 && input.z == 0 && m_body.velocity != Vector3.zero ) {
			m_slowStartTime = m_slowStartTime == -1 ? Time.time : m_slowStartTime;
			float slowElapsedTime = Time.time - m_slowStartTime;
			m_body.velocity = Vector3.Lerp( m_body.velocity, Vector3.zero, slowElapsedTime / SLOW_DURATION );
			Debug.Log( "Slow percent: " + slowElapsedTime / SLOW_DURATION );
			m_animator.SetFloat( "MoveSpeed", Mathf.Max( 1 - ( slowElapsedTime / SLOW_DURATION ), 0 ) );
		} else {
			// TODO: Probably want some sort of lerp here.
			Vector3 newDir = Vector3.RotateTowards( transform.forward, input, 90, 0.0f );
			transform.rotation = Quaternion.LookRotation( newDir );

			m_body.velocity = velocity;
			m_slowStartTime = -1;
			Debug.Log( "We movin baby" );
			if( input != Vector3.zero ) {
				// TODO: Walk support ???
				m_animator.SetFloat( "MoveSpeed", 1 );
			}
		}
	}
}
