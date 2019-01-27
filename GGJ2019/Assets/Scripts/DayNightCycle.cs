using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
	public float speed = 1f;
	public float nightAtAngle = -10f;

	void Update()
	{
		if (transform.rotation.x > nightAtAngle) {
			
		}
	}
}
