using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MushroomHome mushroomHome;

    public int numLevels = 10;
    public float homeGrowthTime = 1.0f;
    public float screenExpandTime = 1.0f;

    private CameraStretcher cameraStretcher;
    private int currentLevel;
    private float scaleFactor;

    private void Start()
    {
        cameraStretcher = GetComponent<CameraStretcher>();
        currentLevel = 1;
        scaleFactor = 1 + 1.0f / numLevels;
        mushroomHome.scaleFactor = scaleFactor;
        cameraStretcher.scaleFactor = scaleFactor;
        cameraStretcher.stretchTime = screenExpandTime;


    }

    private void Update()
    {
        mushroomHome.growthTime = homeGrowthTime;
    }

    // Called by MushroomHome when ready to upgrade
    public void UpgradeHome()
    {
        // TODO: Mesh Flicker,

        //mushroomHome.IncreaseSize();

        // TODO: Flash/VFX,
        // TODO: Replace model,

        //cameraStretcher.Stretch();

        StartCoroutine("UpgradeHomeCoroutine");
    }

    IEnumerator UpgradeHomeCoroutine()
    {
        mushroomHome.IncreaseSize();
        yield return new WaitForSeconds(homeGrowthTime);

        cameraStretcher.Stretch();
        yield return new WaitForSeconds(screenExpandTime);
    }
}
