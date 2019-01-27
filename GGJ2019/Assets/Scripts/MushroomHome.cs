using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomHome : MonoBehaviour
{
    public float scale;
    public float minScale = 5.0f;
    public float maxScale = 70.0f;
    public float growthTime;
    public float scaleFactor;

    private void Start()
    {
        // Assuming uniform scaling
        scale = minScale;
    }

    public void IncreaseSize()
    {
        StartCoroutine("IncreaseSizeOverTime");
    }

    IEnumerator IncreaseSizeOverTime()
    {
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
    }

}
