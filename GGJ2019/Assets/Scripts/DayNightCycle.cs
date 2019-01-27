using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
	public float speed = 1f;
	float rotationSoFar = 0f; //Increment this and stop rotating after we've rotated 180

	void Update()
	{
		if (rotationSoFar < 180)
		{
			float delta = speed * Time.deltaTime;
			transform.Rotate(delta, 0, 0, Space.Self);
			rotationSoFar += delta;
		}
	}
}
