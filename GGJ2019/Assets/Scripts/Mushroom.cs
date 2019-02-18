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
    [HideInInspector]
    public float currentDisolveTime = 0.0f;

    public float timeToDissolve = 10.0f;
	public static float THROW_SPEED = 35f;
    public MushroomState State { get; private set; }
	public BoxCollider Collider;
	public BoxCollider Trigger;

	public Material onGroundMat;
	public Material onEnemyMat;
	public int slimeHitCounter;
	public bool playerThrew;
    private Material defaultMat;

	private Rigidbody m_body;

    private void Start()
    {
        State = MushroomState.Idle;
        isEnemyTracking = false;
		m_body = GetComponent<Rigidbody>();
        defaultMat = transform.GetComponentInChildren<Renderer>().material;
		slimeHitCounter = 0;
		playerThrew = false;
		Collider = GetComponent<BoxCollider>();
    }

	private void OnDestroy () {
		MushroomSpawner.RemoveMushroom();
	}

	private void OnExitIdle()
    {
        MushroomSpawner.RemoveIdleMushroom();
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


	public void IncrementHitCounter() {
		if( playerThrew )
			slimeHitCounter++;
	}


	public void Throw( Vector3 direction ) {
		SetState( MushroomState.Throw );
		m_body.velocity = THROW_SPEED * direction;
	}


	void OnEnterThrow() {
		m_body.isKinematic = false;
		m_body.useGravity = true;
	}


	void OnExitThrow () {
		m_body.isKinematic = true;
		m_body.useGravity = false;
	}


	void OnEnterPicked() {
		Collider.enabled = false;
	}
	

	void OnExitPicked() {
		Collider.enabled = true;
	}

    void OnEnterGround() {
        transform.GetComponentInChildren<Renderer>().material = onGroundMat;
		GameObject.Find( "Main Camera" ).GetComponent<GameManager>().PlayMultiHit( slimeHitCounter );
		slimeHitCounter = 0;
	}

    void OnExitGround() {
        transform.GetComponentInChildren<Renderer>().material = defaultMat;
    }

	void OnEnterInsideEnemy () {
		transform.GetComponentInChildren<Renderer>().material = onEnemyMat;
	}

	void OnExitInsideEnemy () {
		transform.GetComponentInChildren<Renderer>().material = defaultMat;
	}

    void OnEnterIdle(){
        isEnemyTracking = false;
    }

    void OnEnterState( MushroomState state )
    {
        switch ( state )
        {
            case MushroomState.Idle:
                OnEnterIdle();
                break;
            case MushroomState.Picked:
                OnEnterPicked();
				break;
            case MushroomState.Throw:
                OnEnterThrow();
				break;
            case MushroomState.OnGround:
                OnEnterGround();
                break;
			case MushroomState.InsideEnemy:
				OnEnterInsideEnemy();
				break;
        }
    }

    void OnExitState( MushroomState state )
    {
        switch ( state )
        {
            case MushroomState.Idle:
				OnExitIdle();
                break;
            case MushroomState.Picked:
                OnExitPicked();
                break;
            case MushroomState.Throw:
				OnExitThrow();
                break;
            case MushroomState.OnGround:
                OnExitGround();
                break;
			case MushroomState.InsideEnemy:
				OnExitInsideEnemy();
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


	private void OnCollisionEnter ( Collision collision ) {
		if( collision.gameObject == null )
			return;

		string collisionTag = collision.gameObject.tag;
		if( collisionTag == "Ground" ) {
			SetState( MushroomState.OnGround );
		} 
	}


	private void OnTriggerEnter ( Collider collision ) {
		string collisionTag = collision.gameObject.tag;
		 if( State == MushroomState.Throw ) {
			if( collisionTag == "Mushroom" ) {
				GameObject mushroom = collision.gameObject;
				Mushroom mushroomScript = mushroom.GetComponent<Mushroom>();
				if( mushroomScript.State == MushroomState.Idle ) {
					mushroomScript.SetState( MushroomState.Throw );
				}
			} else if( collisionTag == "Enemy" ) {
				GameObject enemy = collision.gameObject;
				Enemy enemyScript = enemy.GetComponent<Enemy>();
				enemyScript.ThrowMushroomCollide( this );
				IncrementHitCounter();
			}
		}
	}


	private void OnGroundUpdate()
    {
        // bob up and down and rotate 
        float centerY = 5.0f;
        float totalHeight = 2.0f;
        transform.Rotate( new Vector3( Random.value, Random.value, Random.value ) );
        transform.position = new Vector3( transform.position.x, centerY + Mathf.PingPong( ( Time.time / 4 ) * centerY, totalHeight ) - totalHeight / 2f, transform.position.z );
    }

    private void Update()
    {
        UpdateState();
    }

    private void OnEnable() {
    	
    }
}
