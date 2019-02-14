﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
	public float SPEED = 1f;
	public float DUSK_SPEED = 0.2f;
	public static float DUSK_START_TIME = 105f;
	public static float DUSK_END_TIME = 120f;
	public static float RotationSoFar = 0f; //Increment this and stop rotating after we've rotated 180
	public const float MAX_ROTATION = 128f;

	public static bool IsDusk() {
		return RotationSoFar >= DUSK_START_TIME && RotationSoFar <= DUSK_END_TIME;
	}

	public static bool IsNight() {
		return RotationSoFar >= DUSK_START_TIME;
	}

	void Update()
	{
		if( GameManager.GetState() == GameManager.GameState.TitleScreen || GameManager.GetState() == GameManager.GameState.Tutorial )
			return;

		if ( RotationSoFar < MAX_ROTATION )
		{
			float step = SPEED;
			if( IsDusk() )
				step = DUSK_SPEED;

			float delta = step * Time.deltaTime;
			transform.Rotate(delta, 0, 0, Space.Self);
			RotationSoFar += delta;
		}
        if( RotationSoFar >= MAX_ROTATION )
        {
            SceneManager.LoadScene("Results");
        }
	}
}
