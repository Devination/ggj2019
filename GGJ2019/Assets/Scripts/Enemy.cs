using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        HUNGRY,
        EATING,
        ANGRY,
        SEEKING_MUSHROOM,
    };

    public EnemyState State { get; private set; }
    public float eatTimer = 5.0f;
    public float moveSpeed = 5.0f;

    public GameObject myMushroomPosition;
    public Stack<GameObject> PickedMushrooms { get; private set; }
    private GameObject m_player;
    private GameObject m_currentTargetMushroom;
    private NavMeshAgent m_agent;
    private float m_currentEatTime;

    private void Start()
    {
        PickedMushrooms = new Stack<GameObject>();
        State = EnemyState.HUNGRY;
        m_agent = GetComponent<NavMeshAgent>();
        m_player = GameObject.Find("Player");
        m_currentEatTime = 0.0f;
    }

    public void SetState(EnemyState state )
    {
        if ( State != state )
        {
            OnExitState( State );
            OnEnterState( state );
            State = state;
        }
    }

    void OnEnterState(EnemyState state )
    {
        switch ( state )
        {
            case EnemyState.HUNGRY:
                break;
            case EnemyState.EATING:
                break;
            case EnemyState.ANGRY:
                break;
        }
    }

    void OnExitState(EnemyState state )
    {
        switch ( state )
        {
            case EnemyState.HUNGRY:
                break;
            case EnemyState.EATING:
                OnExitStateEating();
                break;
            case EnemyState.ANGRY:
                break;
        }
    }

    void UpdateState()
    {
        switch ( State )
        {
            case EnemyState.HUNGRY:
                OnEnemyStateUpdateHungry();
                break;
            case EnemyState.SEEKING_MUSHROOM:
                OnEnemyStateSeekingMushroom();
                break;
            case EnemyState.EATING:
                OnEnemyStateEating();
                break;
            case EnemyState.ANGRY:
                break;
        }
    }

    private void OnDestroy()
    {
        EnemySpawner.RemoveEnemy();
    }

    private void OnEnemyStateUpdateHungry()
    {
        m_currentTargetMushroom = MushroomSpawner.FindClosestIdleMushroom(transform.position);
        if ( m_currentTargetMushroom == null ) return;
        m_currentTargetMushroom.GetComponent<Mushroom>().isEnemyTracking = true;
        m_agent.destination = m_currentTargetMushroom.transform.position;
        m_agent.speed = moveSpeed;
        SetState( EnemyState.SEEKING_MUSHROOM );
    }

    private void OnEnemyStateSeekingMushroom()
    {
        float distanceToPlayer = Vector3.Distance( transform.position, m_player.transform.position );
        float distanceToTarget = Vector3.Distance( transform.position, m_currentTargetMushroom.transform.position );

        if( distanceToPlayer < distanceToTarget )
        {
            // check number of picked mushrooms, if its more than 1, consider seeking player ?
        }

        float sizeOfMushroom = m_currentTargetMushroom.GetComponentInChildren<Renderer>().bounds.size.x;
        if( distanceToTarget < sizeOfMushroom )
        {
            m_agent.destination = transform.position; // stop movement
            SetState( EnemyState.EATING );
        }
    }

    private void OnEnemyStateEating()
    {
        m_currentEatTime += Time.deltaTime;
        if( m_currentEatTime > eatTimer )
        {
            SetState( EnemyState.HUNGRY );
        }
        float newScale = 3.0f + Mathf.PingPong( Time.time, 0.5f );
        transform.localScale = new Vector3( newScale, transform.localScale.y, newScale );
    }

    private void OnExitStateEating()
    {
        transform.localScale = Vector3.one * 3.0f;
        m_currentEatTime = 0.0f;
        PickedMushrooms.Push( m_currentTargetMushroom );
        Mushroom mushroomScript = m_currentTargetMushroom.GetComponent<Mushroom>();
        mushroomScript.SetState( Mushroom.MushroomState.InsideEnemy );

        int headMushCount = PickedMushrooms.Count;
        float mushroomHeight = headMushCount * m_currentTargetMushroom.GetComponentInChildren<MeshRenderer>().bounds.extents.y;
        Vector3 mushroomPosition = myMushroomPosition.transform.position;
        m_currentTargetMushroom.transform.SetParent( transform );
        m_currentTargetMushroom.transform.position = new Vector3(mushroomPosition.x, mushroomPosition.y + mushroomHeight, mushroomPosition.z);
    }

    private void Update()
    {
        UpdateState();
    }
}


