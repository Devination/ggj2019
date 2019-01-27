using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
	public float speed = 10f;
	public static float RotationSoFar = 0f; //Increment this and stop rotating after we've rotated 180
	public const float MAX_ROTATION = 180f;

	void Update()
	{
		if ( RotationSoFar < MAX_ROTATION )
		{
			float delta = speed * Time.deltaTime;
			transform.Rotate(delta, 0, 0, Space.Self);
			RotationSoFar += delta;
		}
	}
}
