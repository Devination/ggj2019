﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSpawner : MonoBehaviour
{
    public float spawnRadius = 30.0f;
    public float spawnTimer = 1.0f;
    public float mushroomRadius = 1.0f; // replace with mushroom collider bounds?
    public int maxNumberOfMushrooms = 40;
    public GameObject mushroomPrefab;
    public List<LayerMask> obstructedLayers;

    private static int m_numMushrooms = 0;
    private float m_currentSpawnTime = 0.0f;
    private LayerMask m_obstructedMask;
    private List<GameObject> m_mushroomPool;

    public enum SPAWN_RESULT
    {
        SUCESS,
        FAIL
    };

    private void Start()
    {
        m_mushroomPool = new List<GameObject>( maxNumberOfMushrooms);
        GameObject mushroomContainer = new GameObject("ShroomContainer");

        for ( int i = 0; i < maxNumberOfMushrooms; ++i )
        {
            m_mushroomPool.Insert(i, Instantiate( mushroomPrefab, Vector3.zero, Quaternion.identity ) );
            m_mushroomPool[i].SetActive( false );
            m_mushroomPool[i].transform.SetParent( mushroomContainer.transform );
        }
        for ( int i = 0; i < obstructedLayers.Count; ++i )
        {
            m_obstructedMask |= obstructedLayers[i];
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
        return !( Physics.CheckSphere( position, mushroomRadius, m_obstructedMask) );
    }

    private int FindFreeMushroom()
    {
        int freeIndex = -1;
        for (int i = 0; i < m_mushroomPool.Count; ++i)
        {
            if( !m_mushroomPool[i].activeSelf )
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
            Renderer mushroomRenderer = m_mushroomPool[index].GetComponent<Renderer>();
            float height = mushroomRenderer.bounds.center.y + mushroomRenderer.bounds.extents.y;
            m_mushroomPool[index].transform.position = new Vector3( position.x, position.y + height, position.z );
            m_mushroomPool[index].SetActive( true );
            m_numMushrooms += 1;
            return SPAWN_RESULT.SUCESS;
        }
        return SPAWN_RESULT.FAIL;
    }

    public static void RemoveMushroom()
    {
        m_numMushrooms -= 1;
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
            if ( m_mushroomPool[i].activeSelf )
            {
                UnityEditor.Handles.DrawWireDisc( m_mushroomPool[i].transform.position, transform.up, mushroomRadius );
            }
        }
    }
}
