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
    public float spawnTimer = 1.0f;
    public float mushroomRadius = 1.0f; // replace with mushroom collider bounds?
    public int maxNumberOfMushrooms = 40;
    public GameObject mushroomPrefab;
    public LayerMask obstructedLayerMask;

    private static int m_numMushrooms = 0;
    private float m_currentSpawnTime = 0.0f;

    private void Start()
    {
        mushroomContainer = new GameObject( "ShroomContainer" );
    }

    void Update()
    {
		if( !GameManager.ShouldSpawnMushrooms() )
			return;

		float maxNumMushrooms = GameManager.GetState() == GameManager.GameState.Tutorial ? 5 : maxNumberOfMushrooms;
        m_currentSpawnTime += Time.deltaTime;
        if( m_currentSpawnTime > spawnTimer )
        {
            if( m_numMushrooms < maxNumMushrooms )
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
                while ( !IsValidPlacement( spawnWorldPosition ) || numberOfAttempts > 4 );
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
        return !( Physics.CheckSphere( position, mushroomRadius, obstructedLayerMask) );
    }

    private void SpawnMushroom( Vector3 position )
    {
        GameObject mushroom = Instantiate(mushroomPrefab, Vector3.zero, Quaternion.identity);
        mushroom.transform.parent = mushroomContainer.transform;
        mushroom.transform.position = position;
        m_numMushrooms += 1;
    }

    public static void RemoveMushroom()
    {
        m_numMushrooms -= 1;
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
