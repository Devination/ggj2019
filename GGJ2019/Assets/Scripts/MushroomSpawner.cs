using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MushroomSpawner : MonoBehaviour
{
    public enum SPAWN_RESULT
    {
        SUCESS,
        FAIL
    };

    [HideInInspector]
    public static GameObject mushroomContainer;

    public float spawnRadius = 40.0f;
    public float mushroomRadius = 1.0f; // replace with mushroom collider bounds?
    public int maxNumberOfMushrooms = 70;
    public GameObject mushroomPrefab;
    public LayerMask obstructedLayerMask;

    private static int m_numIdleMushrooms = 0;
	private static int m_totalMushrooms = 0;
    private float m_currentSpawnTime = 0.0f;

    private void Start()
    {
        mushroomContainer = new GameObject( "ShroomContainer" );
    }

	public float GetSpawnTimer() {
		int currentLevel = GameManager.GetLevel();
		switch( currentLevel ) {
			case 0:
			case 1:
			case 2:
			case 3:
				return 1.0f;
			case 4:
				return 0.8f;
			case 5:
				return 0.6f;
			case 6:
				return 0.5f;
			default:
				return 0.4f;
		}
	}

	public static void Reset () {
		m_numIdleMushrooms = 0;
	}

	void Update()
    {
		if( !GameManager.ShouldSpawnMushrooms() )
			return;

		float maxNumMushrooms = GameManager.GetState() == GameManager.GameState.Tutorial ? 5 : maxNumberOfMushrooms;
        m_currentSpawnTime += Time.deltaTime;
        if( m_currentSpawnTime > GetSpawnTimer() )
        {
			bool canSpawnMushroom = m_numIdleMushrooms < maxNumberOfMushrooms;
			bool stopSpawning = GameManager.GetState() == GameManager.GameState.Tutorial && m_totalMushrooms > 5;
			if( !stopSpawning && m_numIdleMushrooms < maxNumMushrooms )
            {
                Vector2 spawnPosition;
                Vector3 spawnWorldPosition;
                int numberOfAttempts = 0;
                do
                {
                    spawnPosition = Random.insideUnitCircle * spawnRadius;
                    spawnWorldPosition = new Vector3( spawnPosition.x, 0.0f, spawnPosition.y );
                    numberOfAttempts += 1;
                }
                while ( !IsValidPlacement( spawnWorldPosition ) && numberOfAttempts < 4 );
                if( numberOfAttempts < 4 )
                {
                    m_currentSpawnTime = 0.0f;
                    SpawnMushroom( spawnWorldPosition );
                }
            }
        }
    }
  
    private bool IsValidPlacement( Vector3 position )
    {
        // make sure the world point is inside the viewport bounds
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);
        if ( viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1 )
        {
            // fire ray through that point into the world and make sure it is not ocluded by something 
            Ray ray = Camera.main.ViewportPointToRay( viewportPoint );
            if ( Physics.Raycast( ray, out RaycastHit hit ) )
            {
                // for some reason the house has a tag of Ground, so im checking directly against the colliders name 
                if ( hit.collider.name != "GroundPlane" )
                {
                    return false;
                }
                return !(Physics.CheckSphere(position, mushroomRadius, obstructedLayerMask));
            }
        }
        return false;
    }

    private void SpawnMushroom( Vector3 position )
    {
        GameObject mushroom = Instantiate(mushroomPrefab, Vector3.zero, Quaternion.identity);
        mushroom.transform.parent = mushroomContainer.transform;
        mushroom.transform.position = position;
        m_numIdleMushrooms += 1;
		m_totalMushrooms += 1;
    }

    public static void RemoveIdleMushroom ()
    {
        m_numIdleMushrooms -= 1;
    }

	public static void RemoveMushroom() {
		m_totalMushrooms -= 1;
	}

    public static GameObject FindClosestMushroom( Vector3 position )
    {
        GameObject closestMush = null; 
        Collider[] colliders = Physics.OverlapSphere( position, 1000.0f, ( 1 << LayerMask.NameToLayer( "Mushroom" ) ) );
        if ( colliders.Length > 0 )
        {
            float distance = float.MaxValue;
            for ( int i = 0; i < colliders.Length; ++i )
            {
                Mushroom mushroom = colliders[i].GetComponent<Mushroom>();
                if( mushroom == null ) // shoud not happen
                {
                    continue;
                }
                if( mushroom.isEnemyTracking )
                {
                    continue;
                }
                if( mushroom.State == Mushroom.MushroomState.Idle || mushroom.State == Mushroom.MushroomState.OnGround )
                {
                    float currDist = Vector3.SqrMagnitude( position - colliders[i].transform.position );
                    if ( mushroom.State == Mushroom.MushroomState.OnGround )
                    {
                        currDist -= distance * 0.25f; // i dont even know man....just trying to give higher priority to grounded shrooms, nawww meeeann
                    }
                    if ( currDist < distance )
                    {
                        distance = currDist;
                        closestMush = colliders[i].gameObject;
                    }
                }
            }
        }
        return closestMush;
    }

    public void IncreaseRadius(float scaleFactor)
    {
        spawnRadius *= scaleFactor;
    }

    void OnDrawGizmos()
    {
        //UnityEditor.Handles.color = Color.yellow;
        //UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, spawnRadius);

        //if (m_mushroomPool == null)
        //{
        //    return;
        //}
        //UnityEditor.Handles.color = Color.green;
        //for (int i = 0; i < m_mushroomPool.Count; ++i)
        //{
        //    if (m_mushroomPool[i] != null && m_mushroomPool[i].activeSelf)
        //    {
        //        UnityEditor.Handles.DrawWireDisc(m_mushroomPool[i].transform.position, transform.up, mushroomRadius);
        //    }
        //}
    }
}
