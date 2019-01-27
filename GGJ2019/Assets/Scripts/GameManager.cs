using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int numLevels = 3;
    public float homeGrowthTime = 1.0f;
    public float screenExpandTime = 1.0f;
    public float vignetteInTime = 0.75f;
    public float vignetteOutTime = 0.75f;
    public float pauseTime = 0.75f;


    private MushroomHome mushroomHome;
    private CameraStretcher cameraStretcher;
    private int currentLevel = 0;
    private float scaleFactor;
    private int swapMeshLevel;

    private void Start()
    {
        cameraStretcher = GetComponent<CameraStretcher>();
        scaleFactor = 1 + 1.0f / numLevels;
    }

    private void Update()
    {
        if (mushroomHome == null)
        {
            mushroomHome = GameObject.Find("MushroomHome").GetComponent<MushroomHome>();
            swapMeshLevel = numLevels / mushroomHome.meshes.Length;
            mushroomHome.scaleFactor = scaleFactor;
            cameraStretcher.MushroomHome = mushroomHome.gameObject;
        }

        cameraStretcher.scaleFactor = scaleFactor;
        cameraStretcher.stretchTime = screenExpandTime;
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

            yield return new WaitForSeconds(pauseTime);
            mushroomHome.SwapMesh();
            yield return new WaitForSeconds(pauseTime);

            cameraStretcher.VignetteOut();
            yield return new WaitForSeconds(vignetteOutTime);
        }
    }
}
