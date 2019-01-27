using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour {
	public float fadeSpeed = 1f;

	//TODO: Get post fx and also fade those?

	private DayNightCycle daynight;
	private Image image;
	private float daynightSpeed = 1f;
	private bool isPlaying = false;

	void Start() {
		image = gameObject.GetComponentInChildren<Image>();
		daynight = Object.FindObjectOfType<DayNightCycle>();
		daynightSpeed = daynight.speed;
		daynight.speed = 0f;
	}

	void Update() {
		if (!isPlaying) {
			if (Input.anyKeyDown) {
				daynight.speed = daynightSpeed;
				isPlaying = true;
			}
		}
		else {
			Color color = image.color;
			color.a = Mathf.Lerp( image.color.a, 0f, fadeSpeed * Time.deltaTime );
			image.color = color;
		}
	}
}
