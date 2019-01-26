using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnRadius = 30.0f;
    public float spawnTimer = 1.0f;
    public float enemyRadius = 1.0f;
    public float minSpawnDistanceFromPlayer = 20.0f;
    public int maxNumberOfEnemies = 20;

    public GameObject player;
    public GameObject enemyPrefab;
    public LayerMask obstructedLayerMask;
    private static int m_numEnemies = 0;
    private float m_currentSpawnTime = 0.0f;
    private GameObject m_enemyContainer;

    private void Start()
    {
       m_enemyContainer = new GameObject("EnemyContainer");
    }

    public static void RemoveEnemy()
    {
        m_numEnemies -= 1;
    }

    void Update()
    {
        m_currentSpawnTime += Time.deltaTime;
        if ( m_currentSpawnTime > spawnTimer )
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
        GameObject enemy = Instantiate( enemyPrefab, position, Quaternion.identity );
        enemy.transform.SetParent( m_enemyContainer.transform );
        m_numEnemies += 1;
    }

    private bool IsValidPlacement( Vector3 position )
    {
        if( !(Physics.CheckSphere( position, enemyRadius, obstructedLayerMask) ))
        {
            return Vector3.SqrMagnitude( player.transform.position - position ) > ( minSpawnDistanceFromPlayer * minSpawnDistanceFromPlayer );
        }
        return false;
    }

    void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, spawnRadius);
    }
}
