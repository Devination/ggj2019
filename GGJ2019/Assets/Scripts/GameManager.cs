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
	private int incrementPerLevel = 4;
	public float homeGrowthTime = 1.0f;
	public float screenExpandTime = 1.0f;
	public float vignetteInTime = 0.75f;
	public float vignetteOutTime = 0.75f;
	public float pauseTime = 0.75f;
	public float eatTime = 0.25f;
	public AudioClip Tutorial1Audio;
	public AudioClip Tutorial2Audio;
	public AudioClip Tutorial3Audio;
	public AudioSource TutorialSource;
	public AudioClip[] MultiKillAudio;
	public AudioSource MultiKillSource;
    public static GameState CurrentGameState = GameState.TitleScreen;
    
    // stats
    public static int numMushroomsCollected = 0;
    public static int mushroomHouseIndex = -1;
    public static int mushroomHouseSize = 0;

	private static MushroomHome mushroomHome;
    private CameraStretcher cameraStretcher;
    private MushroomSpawner shroomSpawner;
    private EnemySpawner enemySpawner;
    private HUD hud;
    private static int currentLevel = 0;
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

        if (hud == null)
        {
            hud = GameObject.Find("HUD").GetComponent<HUD>();
        }

        UpdateHUD();

        cameraStretcher.scaleFactor = scaleFactor;
        cameraStretcher.stretchTime = screenExpandTime;
        mushroomHome.growthTime = homeGrowthTime;
        mushroomHome.eatTime = eatTime;
        cameraStretcher.vignetteInTime = vignetteInTime;
        cameraStretcher.vignetteOutTime = vignetteOutTime;
    }

	public static int GetLevel() {
		return currentLevel;
	}

	public static void ResetStats () {
		DayNightCycle.RotationSoFar = 0.0f;
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

	public static bool InUI () {
		return GetState() == GameState.TitleScreen || GetState() == GameState.Results;
	}

	public static bool ShouldSpawnMushrooms () {
		return !InUI();
	}

	public static bool ShouldSpawnEnemies () {
		return !InUI() && GetState() != GameState.Tutorial;
	}

	public void PlayTutorial ( int index ) {
		if( index == 0 ) {
			TutorialSource.clip = Tutorial1Audio;
		}
		else if( index == 2 ) {
			TutorialSource.clip = Tutorial2Audio;
		}
		else {
			TutorialSource.clip = Tutorial3Audio;
		}

		TutorialSource.Play();
	}

	public void PlayMultiHit ( int numHits ) {
		if( numHits < 2 )
			return;

		if( numHits == 2 ) {
			int random = Random.Range( 0, 2 );
			MultiKillSource.clip = MultiKillAudio[2 + random];
		}
		else if( numHits == 3 ) {
			int random = Random.Range( 0, 2 );
			MultiKillSource.clip = MultiKillAudio[random];
		}
		else {
			MultiKillSource.clip = MultiKillAudio[4];
		}

		MultiKillSource.Play();
	}

	private void UpdateHUD()
    {
        hud.UpdateShroomsRemaining(mushroomHome.GetShroomsRemaining());
    }

    public void SetShroomsCollect()
    {
		if( currentLevel == 0 ) {
			mushroomsToCollect = 3;
		} else if ( currentLevel == 1 ) {
			mushroomsToCollect = 5;
		} else {
			mushroomsToCollect += incrementPerLevel;
		}
        
        mushroomHome.mushroomsToCollect = mushroomsToCollect;
    }


    // Called by MushroomHome when ready to upgrade
    public void UpgradeHome()
    {
        currentLevel++;
		if( currentLevel <= 2 ) {
			if( currentLevel == 2 )
				GameManager.SetState( GameState.MainGame );
			PlayTutorial( currentLevel );
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
