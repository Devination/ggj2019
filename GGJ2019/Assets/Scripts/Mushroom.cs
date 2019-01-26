using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public enum MushroomState
    {
        Idle,
        Picked,
        Throw
    };

    [HideInInspector]
    public int mushroomIndex;

    private MushroomState m_state;
   
    private void Start()
    {
        m_state = MushroomState.Idle;
        mushroomIndex = -1;
    }

    // never actually remove the mushroom gameobject, just call this instead, it will return it to the pool
    void DestroyMushroom()
    {
        MushroomSpawner.RemoveMushroom( mushroomIndex );
    }

    public void SetState( MushroomState state )
    {
        if ( m_state != state )
        {
            OnExitState( m_state );
            OnEnterState( state );
            m_state = state;
        }
    }

    void OnEnterState( MushroomState state )
    {
        switch ( state )
        {
            case MushroomState.Idle:
                break;
            case MushroomState.Picked:
                break;
            case MushroomState.Throw:
                break;
        }
    }

    void OnExitState( MushroomState state )
    {
        switch ( state )
        {
            case MushroomState.Idle:
                break;
            case MushroomState.Picked:
                break;
            case MushroomState.Throw:
                break;
        }
    }

    void UpdateState()
    {
        switch ( m_state )
        {
            case MushroomState.Idle:
                break;
            case MushroomState.Picked:
                break;
            case MushroomState.Throw:
                break;
        }
    }

    private void Update()
    {
        UpdateState();
    }
}
