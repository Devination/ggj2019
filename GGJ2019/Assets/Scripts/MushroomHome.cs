using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomHome : MonoBehaviour
{
    public float scale;
    public float growthTime;
    public float scaleFactor;
    public float eatTime;
	public AudioClip EatSound;
	public AudioClip GrowSound;
	public AudioClip Tutorial1Sound;
	public AudioClip Tutorial2Sound;
	public AudioClip[] MushroomComboSounds;
	public AudioSource HomeAudioSource;
	public AudioSource ComboAudioSource;

	private static float MUSHROOM_EAT_TIME = 1.0f;

    [HideInInspector]
    public GameManager gm;

    private int mushroomCount = 0;
    public int mushroomsToCollect = 0;

    public Mesh[] meshes;

    private int currentMesh = -1;
    private bool isGrowing;
    private float originalScale; 
    private bool isBouncing = false;
	private float mushroomEatTimer = 0;
	private int mushroomComboCount = 0;

    public int GetCurrentMeshindex()
    {
        return currentMesh;
    }

    private void Update()
    {
        if (!isBouncing)
        {
            originalScale = transform.localScale.x;
        }
    }

    public void Grow()
    {
        if (!isGrowing)
        {
			HomeAudioSource.clip = GrowSound;
			HomeAudioSource.Play();
            StartCoroutine("IncreaseSizeOverTime");
            mushroomCount = 0;
        }
    }

    IEnumerator IncreaseSizeOverTime()
    {
        isGrowing = true;
        float scaleTimer = 0.0f;
        float startScale = transform.localScale.y;
        float targetScale = startScale * scaleFactor;

        while (scaleTimer < growthTime)
        {
            scaleTimer += Time.deltaTime;
            float pct = scaleTimer / growthTime;

            float newScale = Mathf.Lerp(startScale, targetScale, pct);
            transform.localScale = new Vector3(newScale, newScale, newScale);

            //Camera.main.gameObject.GetComponent<CameraStretcher>().LookAtHome();

            yield return null;
        }
        isGrowing = false;
    }

    public void SwapMesh()
    {
        currentMesh++;
        GameManager.mushroomHouseIndex = currentMesh;
        if (currentMesh < meshes.Length)
        {
            GetComponent<MeshFilter>().mesh = meshes[currentMesh];
        }
    }

	public void PlayMushroomCombo() {
		switch( mushroomComboCount ) {
			case 7:
				ComboAudioSource.clip = MushroomComboSounds[0];
				ComboAudioSource.Play();
				break;
			case 16:
				ComboAudioSource.clip = MushroomComboSounds[1];
				ComboAudioSource.Play();
				break;
			case 27:
				ComboAudioSource.clip = MushroomComboSounds[2];
				ComboAudioSource.Play();
				break;
			case 39:
				ComboAudioSource.clip = MushroomComboSounds[3];
				ComboAudioSource.Play();
				break;
			case 48:
				ComboAudioSource.clip = MushroomComboSounds[4];
				ComboAudioSource.Play();
				break;
			case 80:
				ComboAudioSource.clip = MushroomComboSounds[5];
				ComboAudioSource.Play();
				break;
		}
	}

    public void EatMushroom()
    {
		mushroomCount++;
		GameManager.numMushroomsCollected++;
		if( mushroomCount >= mushroomsToCollect ) {
			gm.UpgradeHome();
		}

		if( Time.time - MUSHROOM_EAT_TIME < mushroomEatTimer ) {
			mushroomComboCount++;
			PlayMushroomCombo();
		} else {
			mushroomComboCount = 0;
		}
		mushroomEatTimer = Time.time;

		if (isGrowing) return;

        StartCoroutine("AnimateEating");
    }

    public int GetShroomsRemaining()
    {
        return mushroomsToCollect - mushroomCount;
    }

    IEnumerator AnimateEating()
    {
        isBouncing = true;
		HomeAudioSource.clip = EatSound;
		HomeAudioSource.Play();

		float eatTimer = 0.0f;
        float scale = originalScale;
        float targetScale = scale * scaleFactor;

        // Scale up
        while (eatTimer < eatTime / 2.0f)
        {
            float pct = eatTimer / eatTime;
            float newScale = Mathf.Lerp(scale, targetScale, pct);
            transform.localScale = new Vector3(newScale, newScale, newScale);

            eatTimer += Time.deltaTime;
            isBouncing = true;
            yield return null;
        }

        // Scale down
        while (eatTimer < eatTime)
        {
            float pct = eatTimer / eatTime;
            float newScale = Mathf.Lerp(targetScale, scale, pct);
            transform.localScale = new Vector3(newScale, newScale, newScale);

            eatTimer += Time.deltaTime;
            isBouncing = true;
            yield return null;
        }
        isBouncing = false;

        transform.localScale = new Vector3(originalScale, originalScale, originalScale);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Mushroom"))
        {
            if( other.gameObject.GetComponent<Mushroom>().State != Mushroom.MushroomState.Idle )
            {
                Destroy( other.gameObject );
                EatMushroom();
            }
        }
    }
}
