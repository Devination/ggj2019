using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public enum GameState {
		TitleScreen,
		Tutorial,
		MainGame,
		Results,
	}

	public int numLevels = 3;
    public int incrementPerLevel = 10;
	public float homeGrowthTime = 1.0f;
	public float screenExpandTime = 1.0f;
	public float vignetteInTime = 0.75f;
	public float vignetteOutTime = 0.75f;
	public float pauseTime = 0.75f;
	public float eatTime = 0.25f;
	public AudioClip Tutorial1Audio;
	public AudioClip Tutorial2Audio;
	public AudioSource TutorialSource;
    public static GameState CurrentGameState = GameState.TitleScreen;
    
    // stats
    public static int numMushroomsCollected = 0;
    public static int mushroomHouseIndex = 0;
    public static int mushroomHouseSize = 0;

    public static void ResetStats()
    {
        numMushroomsCollected = 0;
        mushroomHouseIndex = -1;
        mushroomHouseSize = 0;
    }

	public static void SetState ( GameState newState ) {
		CurrentGameState = newState;
	}

	public static GameState GetState () {
		return CurrentGameState;
	}

	public static bool InUI() {
		return GetState() == GameState.TitleScreen || GetState() == GameState.Results;
	}

	public static bool ShouldSpawnMushrooms() {
		return !InUI();
	}

	public static bool ShouldSpawnEnemies() {
		return !InUI() && GetState() != GameState.Tutorial;
	}

	public void PlayTutorial ( int index ) {
		if( index == 0 ) {
			TutorialSource.clip = Tutorial1Audio;
		}
		else {
			TutorialSource.clip = Tutorial2Audio;
		}

		TutorialSource.Play();
	}

    private static MushroomHome mushroomHome;
    private CameraStretcher cameraStretcher;
    private MushroomSpawner shroomSpawner;
    private EnemySpawner enemySpawner;
    private int currentLevel = 0;
    private float scaleFactor;
    private int swapMeshLevel;
    private int mushroomsToCollect = 0;

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
            swapMeshLevel = numLevels / (mushroomHome.meshes.Length + 1);
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
        mushroomsToCollect += (currentLevel <= 1) ? incrementPerLevel / 2 : incrementPerLevel;
        mushroomHome.mushroomsToCollect = mushroomsToCollect;
    }


    // Called by MushroomHome when ready to upgrade
    public void UpgradeHome()
    {
        currentLevel++;
		if( currentLevel == 2 ) {
			GameManager.SetState( GameState.MainGame );
			PlayTutorial( 1 );
		}
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
