using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MushroomHome mushroomHome;

    public int numLevels = 10;
    public float homeGrowthTime = 1.0f;
    public float screenExpandTime = 1.0f;
    public float vignetteInTime = 0.25f;
    public float vignetteOutTime = 0.25f;
    public float pauseTime = 0.25f;


    private CameraStretcher cameraStretcher;
    private int currentLevel = 0;
    private float scaleFactor;
    private int swapMeshLevel;

    private void Start()
    {
        cameraStretcher = GetComponent<CameraStretcher>();
        scaleFactor = 1 + 1.0f / numLevels;
        mushroomHome.scaleFactor = scaleFactor;
        cameraStretcher.scaleFactor = scaleFactor;
        cameraStretcher.stretchTime = screenExpandTime;
        swapMeshLevel = numLevels / mushroomHome.meshes.Length;
    }

    private void Update()
    {
        mushroomHome.growthTime = homeGrowthTime;
        cameraStretcher.vignetteInTime = vignetteInTime;
        cameraStretcher.vignetteOutTime = vignetteOutTime;
    }

    // Called by MushroomHome when ready to upgrade
    public void UpgradeHome()
    {
        // TODO: Mesh Flicker,

        // TODO: Flash/VFX,

        // TODO: Replace model,

        StartCoroutine("UpgradeHomeCoroutine");
    }

    IEnumerator UpgradeHomeCoroutine()
    {
        mushroomHome.IncreaseSize();
        yield return new WaitForSeconds(homeGrowthTime);

        cameraStretcher.Stretch();
        yield return new WaitForSeconds(screenExpandTime);

        currentLevel++;
        if (currentLevel % swapMeshLevel == 0)
        {
            cameraStretcher.VignetteIn();
            yield return new WaitForSeconds(vignetteInTime);

            mushroomHome.SwapMesh();
            yield return new WaitForSeconds(pauseTime);

            cameraStretcher.VignetteOut();
            yield return new WaitForSeconds(vignetteOutTime);
        }
    }
}
