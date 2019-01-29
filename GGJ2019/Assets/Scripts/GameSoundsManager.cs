using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundsManager : MonoBehaviour
{
	public enum BackgroundState {
		Morning,
		MorningFade,
		Night,
	}
	public const float FADE_TIME = 6f;
	AudioSource m_backgroundSource;
	AudioSource m_otherSource;
	public static BackgroundState CurrentState = BackgroundState.Morning;
	public static bool m_fading = false;
	public static float m_startBackgroundVolume;

	float NIGHT_VOLUME = 0.02f;
	float MORNING_VOLUME = 0.2f;

	public AudioClip DaytimeAmbience;
	public AudioClip AfternoonAmbience;
	public AudioClip NighttimeAmbience;
	public AudioClip[] GoodPianoClips;
	public AudioClip[] BadPianoClips;
	int m_currentPianoClip = 0;
	public AudioClip EndClip;
	float PIANO_CLIP_STEP_SIZE = 0.5f;
	float m_lastPianoClipTime = 0;
	float NUM_GOOD_CLIP = 4;

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
		m_otherSource.volume = 2;
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
		if( DayNightCycle.RotationSoFar > ( DayNightCycle.MAX_ROTATION / 2.5f ) ) {
			if( CurrentState == BackgroundState.Morning ) {
				CurrentState = BackgroundState.MorningFade;
				IEnumerator fadeBackground = FadeOut( m_backgroundSource, BackgroundState.Night );
				StartCoroutine( fadeBackground );
			}

			if( CurrentState == BackgroundState.Night && m_backgroundSource.volume <= 0 ) {
				m_backgroundSource.clip = AfternoonAmbience;
				m_backgroundSource.Play();
				SetBackgroundVolume( NIGHT_VOLUME );
				m_currentPianoClip = 0;
			}
		}

		if( DayNightCycle.RotationSoFar > m_lastPianoClipTime + PIANO_CLIP_STEP_SIZE ) { 
			if( CurrentState != BackgroundState.Night ) {
				m_otherSource.clip = GoodPianoClips[m_currentPianoClip];
				m_otherSource.Play();
				m_currentPianoClip++;
				if( m_currentPianoClip >= GoodPianoClips.Length ) {
					m_currentPianoClip = 0;
				}
			} else {
				m_otherSource.clip = BadPianoClips[m_currentPianoClip];
				m_otherSource.Play();
				m_currentPianoClip++;
				if( m_currentPianoClip >= BadPianoClips.Length ) {
					m_currentPianoClip = 0;
				}
			}
			Debug.Log( "Played clip " + m_currentPianoClip );
			m_lastPianoClipTime = Time.time;
		}
    }
}
