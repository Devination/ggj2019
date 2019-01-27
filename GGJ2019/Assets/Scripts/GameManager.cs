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
    public float eatTime = 0.25f;


    private MushroomHome mushroomHome;
    private CameraStretcher cameraStretcher;
    private MushroomSpawner shroomSpawner;
    private EnemySpawner enemySpawner;
    private int currentLevel = 0;
    private float scaleFactor;
    private int swapMeshLevel;
    private int mushroomsToCollect;

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
            mushroomHome.gm = this;
            swapMeshLevel = numLevels / mushroomHome.meshes.Length;
            mushroomHome.scaleFactor = scaleFactor;
            cameraStretcher.MushroomHome = mushroomHome.gameObject;
            SetShroomsCollect();
        }

        if (shroomSpawner == null)
        {
            shroomSpawner = GameObject.Find("MushroomSpawner").GetComponent<MushroomSpawner>();
        }

        if (enemySpawner == null)
        {
            enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        }

        cameraStretcher.scaleFactor = scaleFactor;
        cameraStretcher.stretchTime = screenExpandTime;
        mushroomHome.growthTime = homeGrowthTime;
        mushroomHome.eatTime = eatTime;
        cameraStretcher.vignetteInTime = vignetteInTime;
        cameraStretcher.vignetteOutTime = vignetteOutTime;
    }

    public void SetShroomsCollect()
    {
        mushroomsToCollect = (int)Mathf.Pow(2.0f, currentLevel);
        mushroomHome.mushroomsToCollect = mushroomsToCollect;
    }

    // Called by MushroomHome when ready to upgrade
    public void UpgradeHome()
    {
        currentLevel++;
        StartCoroutine("UpgradeHomeCoroutine");
        SetShroomsCollect();
        shroomSpawner.IncreaseRadius(scaleFactor);
        enemySpawner.LevelUp(scaleFactor);
    }

    IEnumerator UpgradeHomeCoroutine()
    {
        mushroomHome.Grow();
        yield return new WaitForSeconds(homeGrowthTime);

        cameraStretcher.Stretch();
        yield return new WaitForSeconds(screenExpandTime);

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
