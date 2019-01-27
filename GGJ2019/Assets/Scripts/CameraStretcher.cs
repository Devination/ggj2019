using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraStretcher : MonoBehaviour
{
    [HideInInspector]
    public float stretchTime;
    [HideInInspector]
    public float scaleFactor;
    [HideInInspector]
    public float vignetteInTime;
    [HideInInspector]
    public float vignetteOutTime;
    [HideInInspector]
    public GameObject MushroomHome;

    private Camera mainCamera;
    private float startSize;
    private float vignetteValue;

    private DepthOfField depthOfField = null;
    private Vignette vignette = null;

    private void Start()
    {
        PostProcessVolume volume = transform.GetComponentInChildren<PostProcessVolume>();
        volume.profile.TryGetSettings(out depthOfField);
        volume.profile.TryGetSettings(out vignette);
        mainCamera = GetComponent<Camera>();
        startSize = mainCamera.orthographicSize;
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

    public void VignetteIn()
    {
        StartCoroutine("VignetteInOverTime");
    }

    IEnumerator VignetteInOverTime()
    {
        float vignetteTimer = 0.0f;
        float targetVignette = 1.0f;
        while (vignetteTimer < vignetteInTime)
        {
            vignetteTimer += Time.deltaTime;
            float pct = vignetteTimer / vignetteInTime;
            vignette.intensity.value = Mathf.Lerp(vignetteValue, targetVignette, pct);
            yield return null;
        }
    }

    public void VignetteOut()
    {
        StartCoroutine("VignetteOutOverTime");
    }

    IEnumerator VignetteOutOverTime()
    {
        float vignetteTimer = 0.0f;
        float resultVignette = 1.0f;
        while (vignetteTimer < vignetteOutTime)
        {
            vignetteTimer += Time.deltaTime;
            float pct = vignetteTimer / vignetteOutTime;
            vignette.intensity.value = Mathf.Lerp(resultVignette, vignetteValue, pct);
            yield return null;
        }
    }
}
