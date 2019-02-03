using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
	public float speed = 1f;
	public static float RotationSoFar = 0f; //Increment this and stop rotating after we've rotated 180
	public const float MAX_ROTATION = 135f;

	void Update()
	{
		if( GameManager.GetState() == GameManager.GameState.TitleScreen || GameManager.GetState() == GameManager.GameState.Tutorial )
			return;

		if ( RotationSoFar < MAX_ROTATION )
		{
			float delta = speed * Time.deltaTime;
			transform.Rotate(delta, 0, 0, Space.Self);
			RotationSoFar += delta;
		}
        if( RotationSoFar >= MAX_ROTATION )
        {
            SceneManager.LoadScene("Results");
        }
	}
}
