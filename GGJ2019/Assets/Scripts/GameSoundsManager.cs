using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundsManager : MonoBehaviour
{
	public enum BackgroundState {
		Morning,
		MorningFade,
		Afternoon,
		AfternoonFade,
		Night
	}
	public const float FADE_TIME = 6f;
	AudioSource m_backgroundSource;
	AudioSource m_otherSource;
	public static BackgroundState CurrentState = BackgroundState.Morning;
	public static bool m_fading = false;
	public static float m_startBackgroundVolume;

	float AFTERNOON_VOLUME = 0.06f;
	float MORNING_VOLUME = 0.5f;

	public AudioClip DaytimeAmbience;
	public AudioClip AfternoonAmbience;
	public AudioClip NighttimeAmbience;
	public AudioClip[] NicePianoClips;
	public AudioClip[] ScaryPianoClips;
	public AudioClip EndClip;

    // Start is called before the first frame update
    void Start() {
		AudioSource[] audioSources = GetComponents<AudioSource>();
		m_backgroundSource = audioSources[0];
		m_otherSource = audioSources[1];
		SetBackgroundVolume( MORNING_VOLUME );
		m_backgroundSource.loop = true;
		m_backgroundSource.clip = DaytimeAmbience;
		m_backgroundSource.Play();
		m_otherSource.loop = false;
	}


	public static IEnumerator FadeOut ( AudioSource audioSource, BackgroundState newState ) {
		while( audioSource.volume > 0 ) {
			audioSource.volume -= m_startBackgroundVolume * ( Time.deltaTime / FADE_TIME );
			yield return new WaitForEndOfFrame();
		}
		CurrentState = newState;
	}


	void SetBackgroundVolume( float newVolume ) {
		m_backgroundSource.volume = newVolume;
		m_startBackgroundVolume = newVolume;
	}


    // Update is called once per frame
    void Update() {
		if( DayNightCycle.RotationSoFar > ( DayNightCycle.MAX_ROTATION / 1.4f ) ) {
			/*m_backgroundSource.Stop();
			m_backgroundSource.clip = NighttimeAmbience;
			m_backgroundSource.Play();*/
		} else if( DayNightCycle.RotationSoFar > ( DayNightCycle.MAX_ROTATION / 2.5f ) ) {
			if( CurrentState == BackgroundState.Morning ) {
				CurrentState = BackgroundState.MorningFade;
				IEnumerator fadeBackground = FadeOut( m_backgroundSource, BackgroundState.Afternoon );
				StartCoroutine( fadeBackground );
			}

			if( CurrentState == BackgroundState.Afternoon && m_backgroundSource.volume <= 0 ) {
				m_backgroundSource.clip = AfternoonAmbience;
				m_backgroundSource.Play();
				SetBackgroundVolume( AFTERNOON_VOLUME );
			}
		}
    }
}
