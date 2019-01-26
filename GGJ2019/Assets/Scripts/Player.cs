using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	const float SPEED = 3.5f;
	const float SLOW_DURATION = 0.25f;
	float slowStartTime;
	Vector2 headDirection;
	private Rigidbody body;

	void Start () {
		body = GetComponent<Rigidbody>();
		headDirection = Vector2.down;
	}

	void Throw () {
		/*GameObject mushroom = Instantiate( Mushroom, body.transform.position, body.transform.rotation );
		Mushroom mushScript = mushroom.GetComponent<Mushroom>();
		mushScript.SetVelocity( body.velocity, new Vector2( direction.x, direction.y ) );*/
	}

	void FixedUpdate () {
		// Handle player movement
		Vector3 input = new Vector3( Input.GetAxisRaw( "Horizontal" ), 0, Input.GetAxisRaw( "Vertical" ) );
		Debug.Log( "X: " + input.x );
		Debug.Log( "Y: " + input.z );
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
	}
}
