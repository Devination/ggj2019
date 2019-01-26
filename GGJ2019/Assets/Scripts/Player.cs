using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	const float SPEED = 3.5f;
	const float SLOW_DURATION = 0.25f;
	float slowStartTime;
	Vector2 headDirection;
	private Rigidbody2D body;

	void Start () {
		body = GetComponent<Rigidbody2D>();
		headDirection = Vector2.down;
	}

	void Throw () {
		/*GameObject mushroom = Instantiate( ThrownMushroom, body.transform.position, body.transform.rotation );
		ThrownMushroom mushScript = mushroom.GetComponent<ThrownMushroom>();
		mushScript.SetVelocity( body.velocity, new Vector2( direction.x, direction.y ) );*/
	}

	void FixedUpdate () {
		// Handle player movement
		Vector2 input = new Vector2( Input.GetAxisRaw( "Horizontal" ), Input.GetAxisRaw( "Vertical" ) );
		Vector2 velocity = input * SPEED;
		// Slow player movement if there is no input.
		if( input.x == 0 && input.y == 0 && body.velocity != Vector2.zero ) {
			slowStartTime = slowStartTime == -1 ? Time.time : slowStartTime;
			float slowElapsedTime = Time.time - slowStartTime;
			body.velocity = Vector2.Lerp( body.velocity, Vector2.zero, slowElapsedTime / SLOW_DURATION );
		}
		else {
			body.velocity = velocity;
			slowStartTime = -1;
		}
	}
}
