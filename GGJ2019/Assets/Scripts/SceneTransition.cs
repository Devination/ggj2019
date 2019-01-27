using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour {
	public float fadeSpeed = 1f;

	//TODO: Get post fx and also fade those?

	private DayNightCycle daynight;
	private Image image;

	void Start() {
		image = gameObject.GetComponentInChildren<Image>();
	}

	void Update() {
		if ( GameManager.GetState() == GameManager.GameState.TitleScreen ) {
			if (Input.anyKeyDown || Input.GetAxis("Horizontal") > 0.1f || Input.GetAxis("Vertical") > 0.1f) {
				GameManager.SetState( GameManager.GameState.Tutorial );
				GameObject.Find( "Main Camera" ).GetComponent<GameManager>().PlayTutorial( 0 );
			}
		}
		else {
			Color color = image.color;
			color.a = Mathf.Lerp( image.color.a, 0f, fadeSpeed * Time.deltaTime );
			image.color = color;
		}
	}
}
