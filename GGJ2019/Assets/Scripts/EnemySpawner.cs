using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour
{
    public float spawnRadius = 30.0f;
    public float enemyRadius = 1.0f;
    public float minSpawnDistanceFromPlayer = 20.0f;
    public int maxNumberOfEnemies = 35;

    public GameObject player;
    public GameObject enemyPrefab;
    private static GameObject m_enemyContainer;
    public LayerMask obstructedLayerMask;

    private static int m_numEnemies = 0;
    private float m_currentSpawnTime = 0.0f;
	private float BASE_SPAWN_TIMER = 5.0f;
	private float SPAWN_TIMER2 = 3.0f;
	private float SPAWN_TIMER3 = 2.0f;
	private float SPAWN_TIMER4 = 1.0f;

	private void Start()
    {
        m_enemyContainer = new GameObject("EnemyContainer");
    }

    public static void RemoveEnemy()
    {
        m_numEnemies -= 1;
    }

	private float GetSpawnTimer() {
		float spawnTimer = BASE_SPAWN_TIMER;
		float rotationSoFar = DayNightCycle.RotationSoFar;
		
		if ( rotationSoFar >= DayNightCycle.DUSK_END_TIME ) {
			spawnTimer = SPAWN_TIMER4;
		} else if ( DayNightCycle.IsDusk() ) {
			spawnTimer = SPAWN_TIMER3;
		}
		else if ( rotationSoFar >= 50 ) {
			spawnTimer = SPAWN_TIMER2;
		}

		return spawnTimer;
	}

    void Update()
    {
		if( !GameManager.ShouldSpawnEnemies() )
			return;

		m_currentSpawnTime += Time.deltaTime;
        if ( m_currentSpawnTime > GetSpawnTimer() )
        {
            if ( m_numEnemies < maxNumberOfEnemies )
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
                while ( !IsValidPlacement(spawnWorldPosition) && numberOfAttempts < 4 );
                if ( numberOfAttempts < 4 )
                {
                    m_currentSpawnTime = 0.0f;
                    SpawnEnemy(spawnWorldPosition);
                }
            }
        }
    }

    private void SpawnEnemy( Vector3 position )
    {
        GameObject enemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
        enemy.transform.parent = m_enemyContainer.transform;
        enemy.transform.position = position;
        m_numEnemies += 1;
    }

    private bool IsValidPlacement( Vector3 position )
    {
        // make sure the world point is inside the viewport bounds
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);
        if ( viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1 )
        {
            // fire ray through that point into the world and make sure it is not ocluded by something 
            Ray ray = Camera.main.ViewportPointToRay(viewportPoint);
            if (Physics.Raycast( ray, out RaycastHit hit ) )
            {
                // for some reason the house has a tag of Ground, so im checking directly against the colliders name 
                if (hit.collider.name != "GroundPlane")
                {
                    return false;
                }
                if (!(Physics.CheckSphere(position, enemyRadius, obstructedLayerMask)))
                {
                    return Vector3.SqrMagnitude(player.transform.position - position) > (minSpawnDistanceFromPlayer * minSpawnDistanceFromPlayer);
                }
            }
        }
        return false;
    }

    public void LevelUp(float scaleFactor)
    {
        spawnRadius *= scaleFactor;

        // Increase enemy speeds
        foreach (Enemy enemy in m_enemyContainer.GetComponentsInChildren<Enemy>())
        {
            enemy.moveSpeed *= scaleFactor;
        }
    }

    void OnDrawGizmos()
    {
        //UnityEditor.Handles.color = Color.red;
        //UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, spawnRadius);
    }
}
