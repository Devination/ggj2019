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
	public AudioSource HomeAudioSource;

    [HideInInspector]
    public GameManager gm;

    private int mushroomCount = 0;
    public int mushroomsToCollect = 0;

    public Mesh[] meshes;

    private int currentMesh = -1;
    private bool isGrowing;

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

            Camera.main.gameObject.GetComponent<CameraStretcher>().LookAtHome();

            yield return null;
        }
        isGrowing = false;
    }

    public void SwapMesh()
    {
        currentMesh++;
        if (currentMesh < meshes.Length)
        {
            GetComponent<MeshFilter>().mesh = meshes[currentMesh];
        }
    }

    public void EatMushroom()
    {
        if (isGrowing) return;

        StartCoroutine("AnimateEating");

        mushroomCount++;
        if (mushroomCount >= mushroomsToCollect)
        {
            gm.UpgradeHome();
        }
    }

    IEnumerator AnimateEating()
    {
		HomeAudioSource.clip = EatSound;
		HomeAudioSource.Play();

		float eatTimer = 0.0f;
        float scale = transform.localScale.y;
        float targetScale = scale * scaleFactor;

        // Scale up
        while (eatTimer < eatTime / 2.0f)
        {
            float pct = eatTimer / eatTime;
            float newScale = Mathf.Lerp(scale, targetScale, pct);
            transform.localScale = new Vector3(newScale, newScale, newScale);

            eatTimer += Time.deltaTime;
            yield return null;
        }

        // Scale down
        while (eatTimer < eatTime)
        {
            float pct = eatTimer / eatTime;
            float newScale = Mathf.Lerp(targetScale, scale, pct);
            transform.localScale = new Vector3(newScale, newScale, newScale);

            eatTimer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Mushroom"))
        {
            other.gameObject.GetComponent<Mushroom>().DestroyMushroom();
            EatMushroom();
        }
    }
}
