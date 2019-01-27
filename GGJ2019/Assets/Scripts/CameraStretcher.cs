using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraStretcher : MonoBehaviour
{
    public GameObject MushroomHome;
    public float stretchTime;
    public float scaleFactor;

    private Camera mainCamera;
    private float startSize;

    DepthOfField depthOfField = null;

    private void Start()
    {
        PostProcessVolume volume = transform.GetComponentInChildren<PostProcessVolume>();
        volume.profile.TryGetSettings(out depthOfField);
        mainCamera = GetComponent<Camera>();
        startSize = mainCamera.orthographicSize;
        LookAtHome();
    }

    public void Stretch()
    {
        StartCoroutine("StretchOverTime");
    }

    public void LookAtHome()
    {
        Vector3 pos = MushroomHome.GetComponent<Renderer>().bounds.center;
        mainCamera.gameObject.transform.LookAt(pos);
    }

    IEnumerator StretchOverTime()
    {
        float stretchTimer = 0.0f;
        startSize = mainCamera.orthographicSize;
        float targetSize = startSize * scaleFactor;

        float focalLength = depthOfField.focalLength.value;
        float targetFocalLength = focalLength * ( 2.0f - scaleFactor );
        while (stretchTimer < stretchTime)
        {
            stretchTimer += Time.deltaTime;
            float pct = stretchTimer / stretchTime;
            depthOfField.focalLength.value = Mathf.Lerp(focalLength, targetFocalLength, pct);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, pct);

            yield return null;
        }
    }
}
