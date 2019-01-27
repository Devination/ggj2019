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

    public float spawnRadius = 40.0f;
    public float spawnTimer = 1.0f;
    public float mushroomRadius = 1.0f; // replace with mushroom collider bounds?
    public int maxNumberOfMushrooms = 40;
    public GameObject mushroomPrefab;
    public LayerMask obstructedLayerMask;

    private static GameObject m_mushroomContainer;
    private static int m_numMushrooms = 0;
    private static List<GameObject> m_mushroomPool;
    private float m_currentSpawnTime = 0.0f;

    private void Start()
    {
        m_mushroomPool = new List<GameObject>( maxNumberOfMushrooms );
        m_mushroomContainer = new GameObject( "ShroomContainer" );

        for ( int i = 0; i < maxNumberOfMushrooms; ++i )
        {
            m_mushroomPool.Insert( i, Instantiate( mushroomPrefab, Vector3.zero, Quaternion.identity ) );
            m_mushroomPool[i].name = "Mushroom " + i;
            m_mushroomPool[i].SetActive( false );
            m_mushroomPool[i].transform.SetParent( m_mushroomContainer.transform );
            m_mushroomPool[i].GetComponent<Mushroom>().mushroomIndex = i;
        }
    }

    void Update()
    {
        m_currentSpawnTime += Time.deltaTime;
        if( m_currentSpawnTime > spawnTimer )
        {
            if( m_numMushrooms < maxNumberOfMushrooms )
            {
                Vector2 spawnPosition;
                Vector3 spawnWorldPosition;
                do
                {
                    spawnPosition = Random.insideUnitCircle * spawnRadius;
                    spawnWorldPosition = new Vector3( spawnPosition.x, 0.0f, spawnPosition.y );
                }
                while ( !IsValidPlacement( spawnWorldPosition ) );
                m_currentSpawnTime = 0.0f;
                SpawnMushroom( spawnWorldPosition );
            }
        }
    }
  
    private bool IsValidPlacement( Vector3 position )
    {
        return !( Physics.CheckSphere( position, mushroomRadius, obstructedLayerMask) );
    }

    private int FindFreeMushroom()
    {
        int freeIndex = -1;
        for ( int i = 0; i < m_mushroomPool.Count; ++i )
        {
            if( !m_mushroomPool[i].active )
            {
                return i;
            }
        }
        return freeIndex;
    }

    private SPAWN_RESULT SpawnMushroom( Vector3 position )
    {
        int index = FindFreeMushroom();
        if( index >= 0 )
        {
            m_mushroomPool[index].transform.position = position;
            m_mushroomPool[index].SetActive( true );
            m_numMushrooms += 1;
            return SPAWN_RESULT.SUCESS;
        }
        return SPAWN_RESULT.FAIL;
    }

    public static void RemoveMushroom( int index )
    {
        Assert.IsTrue( index >= 0 && index < m_mushroomPool.Count );
        m_mushroomPool[index].transform.SetParent( m_mushroomContainer.transform );
        m_mushroomPool[index].SetActive( false );
        m_numMushrooms -= 1;
    }

    public static GameObject GetMushroomFromIndex( int index )
    {
        Assert.IsTrue(index >= 0 && index < m_mushroomPool.Count);
        return m_mushroomPool[index];
    }

    public static GameObject FindClosestMushroom( Vector3 position )
    {
        GameObject closestMush = null; 
        Collider[] colliders = Physics.OverlapSphere( position, float.MaxValue, ( 1 << LayerMask.NameToLayer( "Mushroom") ) );
        if ( colliders.Length > 0 )
        {
            float distance = float.MaxValue;
            for ( int i = 0; i < colliders.Length; ++i )
            {
                Mushroom mushroom = colliders[i].GetComponent<Mushroom>();
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
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc( transform.position, transform.up, spawnRadius );

        if ( m_mushroomPool == null )
        {
            return;
        }
        UnityEditor.Handles.color = Color.green;
        for (int i = 0; i < m_mushroomPool.Count; ++i)
        {
            if (m_mushroomPool[i] != null && m_mushroomPool[i].activeSelf )
            {
                UnityEditor.Handles.DrawWireDisc( m_mushroomPool[i].transform.position, transform.up, mushroomRadius );
            }
        }
    }
}
