using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public enum MushroomState
    {
        Idle,
        Picked,
        OnGround,
        Throw,
        InsideEnemy
    };

    [HideInInspector]
    public int mushroomIndex;
    [HideInInspector]
    public bool isEnemyTracking;

    public MushroomState State { get; private set; }

    private void Start()
    {
        State = MushroomState.Idle;
        mushroomIndex = -1;
        isEnemyTracking = false;
    }

    // never actually remove the mushroom gameobject, just call this instead, it will return it to the pool
    void DestroyMushroom()
    {
        MushroomSpawner.RemoveMushroom( mushroomIndex );
    }

    public void SetState( MushroomState state )
    {
        if ( State != state )
        {
            OnExitState( State );
            OnEnterState( state );
            State = state;
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
        switch ( State )
        {
            case MushroomState.Idle:
                break;
            case MushroomState.Picked:
                break;
            case MushroomState.Throw:
                break;
            case MushroomState.OnGround:
                OnGroundUpdate();
                break;
        }
    }

    private void OnGroundUpdate()
    {
        // bob up and down and rotate 
        float centerY = 5.0f;
        float totalHeight = 10.0f;
        transform.Rotate( new Vector3( Random.value * 2.0f, Random.value * 2.0f, Random.value * 2.0f) );
        transform.position = new Vector3( transform.position.x, centerY + Mathf.PingPong( Time.time * 5.0f, totalHeight ) - totalHeight / 2f, transform.position.z );
    }

    private void Update()
    {
        UpdateState();
    }
}
