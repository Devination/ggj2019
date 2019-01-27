using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundsManager : MonoBehaviour
{
	AudioSource m_backgroundSource;
	AudioSource m_otherSource;

	public AudioClip DaytimeAmbience;
	public AudioClip AfternoonAmbience;
	public AudioClip NighttimeAmbience;
	public AudioClip[] NicePianoClips;
	public AudioClip[] ScaryPianoClips;
	public AudioClip EndClip;

    // Start is called before the first frame update
    void Start()
    {
		m_backgroundSource = gameObject.AddComponent<AudioSource>();
		m_backgroundSource.loop = true;
		m_otherSource = gameObject.AddComponent<AudioSource>();
		m_otherSource.loop = false;
		m_backgroundSource.clip = DaytimeAmbience;
		m_backgroundSource.Play();
	}

    // Update is called once per frame
    void Update()
    {
		if( DayNightCycle.RotationSoFar > ( DayNightCycle.MAX_ROTATION / 1.4f ) ) {
			m_backgroundSource.clip = NighttimeAmbience;
		} else if( DayNightCycle.RotationSoFar > ( DayNightCycle.MAX_ROTATION / 2.5f ) ) {
			m_backgroundSource.clip = AfternoonAmbience;
		}
    }
}
