using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	const float SPEED = 25f;
	const float SLOW_DURATION = 0.25f;
	float slowStartTime;
	Vector3 headDirection;
	private Rigidbody body;

	void Start () {
		body = GetComponent<Rigidbody>();
		headDirection = new Vector3( 0, 0, -1 );
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
		if( input.x == 0 && input.z == 0 && body.velocity != Vector3.zero ) {
			slowStartTime = slowStartTime == -1 ? Time.time : slowStartTime;
			float slowElapsedTime = Time.time - slowStartTime;
			body.velocity = Vector3.Lerp( body.velocity, Vector3.zero, slowElapsedTime / SLOW_DURATION );
		}
		else {
			body.velocity = velocity;
			slowStartTime = -1;
		}

		/*if( input.x ) {
			headDirection = headInput;
			noInputStartTime = -1;
		}
		else if( headInput.x != 0 && headInput.y != 0 ) {
			headDirection.x = 0;
			headDirection.y = headInput.y;
		}
		else if( headInput == Vector2.zero ) {
			noInputStartTime = noInputStartTime == -1 ? Time.time : noInputStartTime;
			if( HEAD_DOWN_DURATION >= Time.time - noInputStartTime ) {
				headDirection = Vector2.down;
			}
		}*/
	}
}
