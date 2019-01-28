using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Results : MonoBehaviour
{
    public Text status;
    public Text numMushrooms;
    private float waitTimeToReset = 0.0f;
	public AudioSource MyAudioSource;
	public AudioClip InadequateAudio;
	public AudioClip DecentAudio;
	public AudioClip AmazingAudio;

    void Start()
    {
        numMushrooms.text = "Number of Mushrooms: " + GameManager.numMushroomsCollected;
        
        int mushroomMeshIndex = GameManager.mushroomHouseIndex;
        mushroomMeshIndex = Mathf.Clamp( mushroomMeshIndex, -1, 1 );
        string statusStr = "";
        switch ( mushroomMeshIndex )
        {
            case -1:
                statusStr = "You are...Inadequate!";
				MyAudioSource.clip = InadequateAudio;
                break;
            case 0:
                statusStr = "You are...Decent!";
				MyAudioSource.clip = DecentAudio;
				break;
            case 1:
                statusStr = "You are...Amazing!";
				MyAudioSource.clip = AmazingAudio;
				break;
        }
        status.text = statusStr;
		MyAudioSource.Play();
    }

    private void Update()
    {
        waitTimeToReset += Time.deltaTime;
        if ( Input.anyKeyDown && waitTimeToReset > 1.0f )
        {
            GameManager.ResetStats();
            SceneManager.LoadScene("GameScene");
        }
    }

}
