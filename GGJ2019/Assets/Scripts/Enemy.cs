using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        HUNGRY,
        PICKING,
        CHASE_PLAYER,
        SEEKING_MUSHROOM,
        DAMAGE,
    };

    public EnemyState State { get; private set; }
    public float eatTimer = 5.0f;
    public float moveSpeed = 10.0f;
	public AudioClip DeadSound;

    [HideInInspector]
    public int enemyIndex;

    public GameObject myMushroomPosition;
    public List<GameObject> PickedMushrooms { get; private set; }
    private GameObject m_player;
    private GameObject m_currentTargetMushroom;
    private NavMeshAgent m_agent;
	private AudioSource m_audioSource;
    private float m_currentEatTime;
    private float m_deathTimer;

    private void Start()
    {
        m_deathTimer = 0.0f;
        PickedMushrooms = new List<GameObject>();
        State = EnemyState.HUNGRY;
        m_agent = GetComponent<NavMeshAgent>();
        m_player = GameObject.Find("Player");
        m_currentEatTime = 0.0f;
		m_audioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        EnemySpawner.RemoveEnemy();
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
        switch (state)
        {
            case EnemyState.HUNGRY:
                OnEnterStateHungry();
                break;
            case EnemyState.DAMAGE:
                OnEnterStateDamaged();
                break;
        }
    }

    void OnExitState(EnemyState state )
    {
        switch ( state )
        {
            case EnemyState.PICKING:
                OnExitStatePicking();
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
            case EnemyState.PICKING:
                OnEnemyStatePicking();
                break;
            case EnemyState.CHASE_PLAYER:
                OnEnemyStateChasePlayer();
                break;
            case EnemyState.DAMAGE:
                OnUpdateStateDamaged();
                break;
        }
    }

    private void OnEnterStateHungry()
    {
        if ( m_currentTargetMushroom != null )
        {
            m_currentTargetMushroom.GetComponent<Mushroom>().isEnemyTracking = false;
        }
    }

    private void OnEnemyStateUpdateHungry()
    {
        m_currentTargetMushroom = MushroomSpawner.FindClosestMushroom(transform.position);
        if (m_currentTargetMushroom == null) return;
        m_currentTargetMushroom.GetComponent<Mushroom>().isEnemyTracking = true;
        m_agent.destination = m_currentTargetMushroom.transform.position;
        m_agent.speed = moveSpeed;
        SetState(EnemyState.SEEKING_MUSHROOM);
    }

    private void CheckCurrentMushroomStateChanged()
    {
        if ( m_currentTargetMushroom == null)
        {
            return;
        }
        if ( m_currentTargetMushroom.GetComponent<Mushroom>().State == Mushroom.MushroomState.Picked )
        {
            SetState( EnemyState.HUNGRY );
        }
    }

    private void OnEnemyStateChasePlayer()
    {
        m_agent.destination = m_player.transform.position;
        m_agent.speed = moveSpeed * 1.5f;
        if ( m_player.GetComponent<Player>().PickedMushrooms.Count <= 1 )
        {
            SetState( EnemyState.HUNGRY );
        }

        GameObject mushroom = MushroomSpawner.FindClosestMushroom(transform.position);
        if( mushroom )
        {
            float distanceToTarget = Vector3.Distance( transform.position, mushroom.transform.position );
            float distanceToPlayer = Vector3.Distance( transform.position, m_player.transform.position );
            if( distanceToTarget < distanceToPlayer )
            {
                SetState( EnemyState.HUNGRY );
            }
        }
    }

    private void OnEnemyStateSeekingMushroom()
    {
        float distanceToPlayer = Vector3.Distance( transform.position, m_player.transform.position );
        float distanceToTarget = Vector3.Distance( transform.position, m_currentTargetMushroom.transform.position );

        if( !m_player )
        {
            m_player = GameObject.Find("Player");
        }

        if( distanceToPlayer < distanceToTarget )
        {
            if( m_player.GetComponent<Player>().PickedMushrooms.Count > 1 )
            {
                m_currentTargetMushroom.GetComponent<Mushroom>().isEnemyTracking = true;
                SetState( EnemyState.CHASE_PLAYER );
            }
        }

        CheckCurrentMushroomStateChanged();

        float sizeOfMushroom = m_currentTargetMushroom.GetComponentInChildren<Renderer>().bounds.size.x;
        if( distanceToTarget < sizeOfMushroom )
        {
            m_agent.destination = transform.position; // stop movement
            SetState( EnemyState.PICKING );
        }
    }

    private void OnEnemyStatePicking()
    {
        CheckCurrentMushroomStateChanged();

        m_currentEatTime += Time.deltaTime;
        if( m_currentEatTime > eatTimer || m_currentTargetMushroom.GetComponent<Mushroom>().State == Mushroom.MushroomState.OnGround)
        {
            m_currentEatTime = eatTimer;
            SetState( EnemyState.HUNGRY );
        }
        float newScale = 3.0f + Mathf.PingPong( Time.time, 0.5f );
        transform.localScale = new Vector3( newScale, transform.localScale.y, newScale );
    }

    private void OnExitStatePicking()
    {
        if( m_currentTargetMushroom == null )
        {
            return;
        }
        // we ate the mushroom, possibly to prematurely exit this state if the current mushroom state changes
        if( m_currentEatTime >= eatTimer )
        {
            PickedMushrooms.Add( m_currentTargetMushroom );
            Mushroom mushroom = m_currentTargetMushroom.GetComponent<Mushroom>();
            mushroom.SetState( Mushroom.MushroomState.InsideEnemy );

            int headMushCount = PickedMushrooms.Count;
            float mushroomHeight = headMushCount * m_currentTargetMushroom.GetComponentInChildren<MeshRenderer>().bounds.extents.y;
            Vector3 mushroomPosition = myMushroomPosition.transform.position;
            m_currentTargetMushroom.transform.SetParent( transform );
            m_currentTargetMushroom.transform.rotation = Quaternion.identity;
            m_currentTargetMushroom.transform.position = new Vector3( mushroomPosition.x, mushroomPosition.y + mushroomHeight, mushroomPosition.z );
        }
        m_currentTargetMushroom.GetComponent<Mushroom>().isEnemyTracking = false;
        transform.localScale = Vector3.one * 3.0f;
        m_currentEatTime = 0.0f;
    }

    private void OnEnterStateDamaged()
    {
        GetComponentInChildren<ParticleSystem>().Play();
        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponentInChildren<BoxCollider>().enabled = false;

		m_audioSource.clip = DeadSound;
		m_audioSource.Play();

		for ( int i = 0; i < PickedMushrooms.Count; ++i )
        {
            PickedMushrooms[i].transform.parent = MushroomSpawner.mushroomContainer.transform;
            Mushroom mushroom = PickedMushrooms[i].GetComponent<Mushroom>();
            mushroom.SetState( Mushroom.MushroomState.OnGround );
        }
    }   

    private void OnUpdateStateDamaged()
    {
        m_deathTimer += Time.deltaTime;
        if ( m_deathTimer > 2.0f )
        {
            Destroy( gameObject );
            m_deathTimer = 0.0f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        string collisionTag = collision.gameObject.tag;
        if ( collisionTag == "Mushroom" )
        {
            GameObject mushroom = collision.gameObject;
            Mushroom mushroomScript = mushroom.GetComponent<Mushroom>();
            if ( mushroomScript.State == Mushroom.MushroomState.Throw )
            {
                SetState( EnemyState.DAMAGE );
            }
        }
    }

    private void Update()
    {
        UpdateState();

        if ( State == EnemyState.DAMAGE )
        {
            return;
        }

        int prevCount = PickedMushrooms.Count;
        for( int i = 0; i < PickedMushrooms.Count; ++i )
        {
            Mushroom mushroom = PickedMushrooms[i].GetComponent<Mushroom>();
            if ( mushroom.State == Mushroom.MushroomState.InsideEnemy )
            {
                mushroom.currentDisolveTime += Time.deltaTime;
                if ( mushroom.currentDisolveTime > mushroom.timeToDissolve )
                {
                    Destroy( PickedMushrooms[i] );
                    PickedMushrooms.RemoveAt( i );
                }
            }
        }

        int newCount = PickedMushrooms.Count;
        if ( prevCount != newCount )
        {
            List<GameObject> tmpList = new List<GameObject>();
            Vector3 mushroomPosition = myMushroomPosition.transform.position;

            for (int i = 0; i < PickedMushrooms.Count; i++)
            {
                if ( PickedMushrooms[i] != null )
                {
                    float height = PickedMushrooms[i].GetComponentInChildren<MeshRenderer>().bounds.extents.y;
                    tmpList.Add(PickedMushrooms[i]);
                    PickedMushrooms[i].transform.position = new Vector3( mushroomPosition.x, mushroomPosition.y + ( i + 1 ) * height, mushroomPosition.z );
                }
            }
            PickedMushrooms = tmpList;
        }
    }
}

