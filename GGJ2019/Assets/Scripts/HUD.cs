using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	private float FADE_IN_SPEED = 0.2f;
	private float FADE_OUT_SPEED = 2f;
	private Text m_shroomsRemaining;
	private Text m_controls;

    private void Start()
    {
        m_shroomsRemaining = transform.Find("ShroomsRemaining").GetComponent<Text>();
		m_controls = transform.Find( "ControlsTutorial" ).GetComponent<Text>();
	}

    public void UpdateShroomsRemaining( int numShrooms )
    {
        m_shroomsRemaining.text = numShrooms.ToString();
    }

	void FadeTitle () {
		
	}

	void Update () {
		Color controlsColor = m_controls.color;
		if( GameManager.GetState() == GameManager.GameState.Tutorial && GameManager.GetLevel() == 0 ) {
			controlsColor.a = Mathf.Lerp( m_controls.color.a, 1f, FADE_IN_SPEED * Time.deltaTime );
			m_controls.color = controlsColor;
		}
		else {
			controlsColor.a = Mathf.Lerp( m_controls.color.a, 0f, FADE_OUT_SPEED * Time.deltaTime );
			m_controls.color = controlsColor;
		}
	}
}
