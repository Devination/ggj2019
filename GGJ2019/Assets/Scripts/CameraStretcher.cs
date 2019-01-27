using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStretcher : MonoBehaviour
{
    public GameObject MushroomHome;
    public float stretchTime;
    public float scaleFactor;

    private Camera mainCamera;
    private float startSize;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        startSize = mainCamera.orthographicSize;
        mainCamera.transform.LookAt(MushroomHome.transform);
    }

    public void Stretch()
    {
        StartCoroutine("StretchOverTime");
    }

    IEnumerator StretchOverTime()
    {
        float stretchTimer = 0.0f;
        startSize = mainCamera.orthographicSize;
        float targetSize = startSize * scaleFactor;
        while (stretchTimer < stretchTime)
        {
            stretchTimer += Time.deltaTime;
            float pct = stretchTimer / stretchTime;
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, pct);
            mainCamera.gameObject.transform.LookAt(MushroomHome.transform.position);

            yield return null;
        }
    }
}
