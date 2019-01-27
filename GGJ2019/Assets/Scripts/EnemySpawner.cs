using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour
{
    public float spawnRadius = 30.0f;
    public float enemyRadius = 1.0f;
    public float minSpawnDistanceFromPlayer = 20.0f;
    public int maxNumberOfEnemies = 20;

    public GameObject player;
    public GameObject enemyPrefab;
    private static GameObject m_enemyContainer;
    public LayerMask obstructedLayerMask;

    private static List<GameObject> mEnemyPool;
    private static int m_numEnemies = 0;
    private float m_currentSpawnTime = 0.0f;
	private float m_spawnTimer = 3.0f;

	private void Start()
    {
        mEnemyPool = new List<GameObject>();
        m_enemyContainer = new GameObject("EnemyContainer");
        for (int i = 0; i < maxNumberOfEnemies; ++i)
        {
            mEnemyPool.Insert(i, Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity));
            mEnemyPool[i].name = "Enemy " + i;
            mEnemyPool[i].SetActive(false);
            mEnemyPool[i].transform.SetParent( m_enemyContainer.transform );
            mEnemyPool[i].GetComponent<Enemy>().enemyIndex = i;
        }
    }

    public static void RemoveEnemy( int index )
    {
        Assert.IsTrue(index >= 0 && index < mEnemyPool.Count);
        mEnemyPool[index].transform.SetParent( m_enemyContainer.transform );
        mEnemyPool[index].SetActive(false);
        m_numEnemies -= 1;
    }

    private int FindFreeEnemy()
    {
        int freeIndex = -1;
        for (int i = 0; i < mEnemyPool.Count; ++i)
        {
            if ( !mEnemyPool[i].active )
            {
                return i;
            }
        }
        return freeIndex;
    }

    void Update()
    {
		if( !GameManager.ShouldSpawnEnemies() )
			return;

		m_currentSpawnTime += Time.deltaTime;
        if ( m_currentSpawnTime > m_spawnTimer )
        {
            if ( m_numEnemies < maxNumberOfEnemies )
            {
                Vector2 spawnPosition;
                Vector3 spawnWorldPosition;
                do
                {
                    spawnPosition = Random.insideUnitCircle * spawnRadius;
                    spawnWorldPosition = new Vector3( spawnPosition.x, 0.0f, spawnPosition.y );
                }
                while (!IsValidPlacement( spawnWorldPosition ) );
                m_currentSpawnTime = 0.0f;
                SpawnEnemy( spawnWorldPosition );
            }
        }
    }

    private void SpawnEnemy( Vector3 position )
    {
        int index = FindFreeEnemy();
        if (index >= 0)
        {
            mEnemyPool[index].transform.position = position;
            mEnemyPool[index].SetActive(true);
            m_numEnemies += 1;
        }
    }

    private bool IsValidPlacement( Vector3 position )
    {
        if( !(Physics.CheckSphere( position, enemyRadius, obstructedLayerMask) ))
        {
            return Vector3.SqrMagnitude( player.transform.position - position ) > ( minSpawnDistanceFromPlayer * minSpawnDistanceFromPlayer );
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
