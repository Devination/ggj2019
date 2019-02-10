using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
	enum PlayerState {
		Normal,
		Picking,
		Damaged
	};
	const float SPEED = 25f;
	const float SLOW_DURATION = 0.25f;
	const float MAX_PICKUP_DISTANCE = 100f;
	const float PICKING_TIME = 0.5f;
	const float DAMAGE_TIME = 1.0f;
	const float FLASH_TIME = 0.05f;
	const float NO_MOVE_TIME = 0.3f;

	public LayerMask PickMask;
	public AudioClip ThrowAudio;
	public AudioClip DamageAudio;
	public AudioClip PickupAudio;

	private PlayerState m_state;
	
	private Rigidbody m_body;
	private Animator m_animator;
	private BoxCollider m_collider;
	private GameObject m_mushroomPosition;
	private SkinnedMeshRenderer m_meshRenderer;
	private AudioSource m_audioSource;

	private float m_slowStartTime;
	private float m_damageStartTime;
	private bool m_flashOn = true;
	private float m_flashTime = -1;

	private Collider m_pickingShroom;
	private float m_pickStartTime;
	public Stack<GameObject> PickedMushrooms { get; private set; }

    private float m_defaultY;

    private bool m_wrapNorthSouth = false;
    private float m_wrapTimer = 0.0f;

    void Start () {
		m_body = GetComponent<Rigidbody>();
		m_animator = GetComponentInChildren<Animator>();
		m_audioSource = GetComponent<AudioSource>();
		m_collider = GetComponent<BoxCollider>();
		m_mushroomPosition = GameObject.Find( "MushroomPosition" );
		PickedMushrooms = new Stack<GameObject>();
		m_meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		SetState( PlayerState.Normal );
        m_defaultY = transform.position.y;
	}

	void Throw() {
		m_audioSource.clip = ThrowAudio;
		m_audioSource.Play();
		GameObject throwMushroom = PickedMushrooms.Pop();
		throwMushroom.transform.SetParent( null, true );
		throwMushroom.transform.position = m_collider.bounds.center + transform.forward * 2;

		Mushroom mushScript = throwMushroom.GetComponent<Mushroom>();
		mushScript.playerThrew = true;
		mushScript.Throw( transform.forward );
		m_animator.SetTrigger("Throw");
	}

	void Pick () {
		Collider[] colliders = Physics.OverlapSphere( m_collider.bounds.center + transform.forward * 5, 5, PickMask );
		if( colliders.Length <= 0 ) {
			return;
		}

		Collider firstMushroom = colliders[0];
        if( firstMushroom == null )
        {
            return;
        }
		Mushroom mushroomScript = firstMushroom.gameObject.GetComponent<Mushroom>();
		if( mushroomScript != null && mushroomScript.State != Mushroom.MushroomState.Idle )
		{
			return;
		}
		SetState( PlayerState.Picking );
		m_pickingShroom = firstMushroom;
	}

	void UpdateIdle () {
		// Handle player movement
		Vector3 moveInput = new Vector3( Input.GetAxisRaw( "Horizontal" ), 0, Input.GetAxisRaw( "Vertical" ) );
		Vector3 velocity = moveInput * SPEED;
		// Slow player movement if there is no input.
		if( moveInput.x == 0 && moveInput.z == 0 ) {
			if( m_body.velocity != Vector3.zero ) {
				m_slowStartTime = m_slowStartTime == -1 ? Time.time : m_slowStartTime;
				float slowElapsedTime = Time.time - m_slowStartTime;
				m_body.velocity = Vector3.Lerp( m_body.velocity, Vector3.zero, ( slowElapsedTime / SLOW_DURATION ) );
				m_animator.SetFloat( "MoveSpeed", Mathf.Max( 1 - ( slowElapsedTime / SLOW_DURATION ), 0 ) );
			}
			else {
				m_animator.SetFloat( "MoveSpeed", 0 );
			}
		}
		else {
			// TODO: Probably want some sort of lerp here.
			Vector3 newDir = Vector3.RotateTowards( transform.forward, moveInput, 90, 0.0f );
			transform.rotation = Quaternion.LookRotation( newDir );

			m_body.velocity = velocity;
			m_slowStartTime = -1;
			if( moveInput != Vector3.zero ) {
				// TODO: Walk support ???
				m_animator.SetFloat( "MoveSpeed", 1 );
			}
		}

		bool pickPressed = Input.GetButtonDown( "Fire1" );
		if( pickPressed && m_state == PlayerState.Normal ) {
			Pick();
		}

		bool throwPressed = Input.GetButtonDown( "Fire2" );
		if( throwPressed && m_state == PlayerState.Normal && PickedMushrooms.Count > 0 ) {
			Throw();
		}
	}


	void UpdatePicking () {
		float pickElapsedTime = Time.time - m_pickStartTime;
		if( pickElapsedTime > PICKING_TIME ) {
			SetState( PlayerState.Normal );
		}
	}


	void UpdateDamaged() {
		float timeSinceDamage = Time.time - m_damageStartTime;
		if( timeSinceDamage > NO_MOVE_TIME ) {
			UpdateIdle();
		}

		if( m_flashTime == -1 || Time.time - m_flashTime > FLASH_TIME ) {
			m_flashTime = Time.time;
			m_flashOn = !m_flashOn;
			m_meshRenderer.enabled = m_flashOn;
		}

		if( timeSinceDamage > DAMAGE_TIME ) {
			SetState( PlayerState.Normal );
		}
	}


	void OnEnterPicking () {
		m_animator.SetTrigger("Pickup");
		m_pickStartTime = Time.time;
	}

	void OnExitNormal() {
		m_body.velocity = new Vector3( 0, 0, 0 );
		m_animator.SetFloat( "MoveSpeed", 0 );
	}

	void OnExitPicking() {
		m_pickStartTime = -1;
		GameObject pickedShroom = m_pickingShroom.gameObject;
        if( pickedShroom != null )
        {
            PickUp(pickedShroom);
        }
    }


	void OnEnterDamaged() {
		m_damageStartTime = Time.time;
		// TODO: Play Damage Anim
		int numDroppedMushrooms = (int)Mathf.Ceil( PickedMushrooms.Count / 2f );
		for( int i = 0; i < numDroppedMushrooms; i++ )
        {
            if( PickedMushrooms.Count > 0 ) 
            {
                GameObject dropMush = PickedMushrooms.Pop();
                Mushroom mushScript = dropMush.GetComponent<Mushroom>();
                dropMush.transform.SetParent(null, true);
                mushScript.SetState(Mushroom.MushroomState.Throw);
            }
		}
		m_audioSource.clip = DamageAudio;
		m_audioSource.Play();
	}


	void OnExitDamaged() {
		m_damageStartTime = -1;
		m_flashTime = -1;
		m_flashOn = true;
		m_meshRenderer.enabled = true;
	}


	void PickUp( GameObject mushroom ) {
		PickedMushrooms.Push( mushroom );
		mushroom.transform.rotation = Quaternion.identity;
		Mushroom mushroomScript = mushroom.GetComponent<Mushroom>();
        if( mushroomScript )
        {
            mushroomScript.SetState(Mushroom.MushroomState.Picked);
            int headMushCount = PickedMushrooms.Count;
            float mushroomHeight = headMushCount * mushroom.GetComponentInChildren<MeshRenderer>().bounds.extents.y;
            Vector3 mushroomPosition = m_mushroomPosition.transform.position;
            mushroom.transform.rotation = Quaternion.identity;
            mushroom.transform.SetParent(transform);
            mushroom.transform.position = new Vector3(mushroomPosition.x, mushroomPosition.y + mushroomHeight, mushroomPosition.z);
            m_audioSource.clip = PickupAudio;
            m_audioSource.Play();
        }
	}


	void OnCollisionEnter ( Collision collision ) {
        if( collision.gameObject != null )
        {
            string collisionTag = collision.gameObject.tag;
            if (collisionTag == "Mushroom")
            {
                GameObject mushroom = collision.gameObject;
                Mushroom mushroomScript = mushroom.GetComponent<Mushroom>();
                if (mushroomScript.State == Mushroom.MushroomState.OnGround)
                {
                    PickUp(mushroom);
                }
            }

            if (m_state != PlayerState.Damaged && collisionTag == "Enemy")
            {
                SetState(PlayerState.Damaged);
            }
        }
	}


	void SetState ( PlayerState state ) {
		if( m_state != state ) {
			OnExitState( m_state );
			OnEnterState( state );
			m_state = state;
		}
	}

	void OnEnterState ( PlayerState state ) {
		switch( state ) {
			case PlayerState.Normal:
				break;
			case PlayerState.Picking:
				OnEnterPicking();
				break;
			case PlayerState.Damaged:
				OnEnterDamaged();
				break;
		}
	}

	void OnExitState ( PlayerState state ) {
		switch( state ) {
			case PlayerState.Normal:
				OnExitNormal();
				break;
			case PlayerState.Picking:
				OnExitPicking();
				break;
			case PlayerState.Damaged:
				OnExitDamaged();
				break;
		}
	}

	void UpdateState () {
		switch( m_state ) {
			case PlayerState.Normal:
				UpdateIdle();
				break;
			case PlayerState.Picking:
				UpdatePicking();
				break;
			case PlayerState.Damaged:
				UpdateDamaged();
				break;
		}
	}

	private void Update() {
		UpdateState();
        WrapEdges();

        // HACK: Something is repositioning player after damaged by enemy,
        //       clamping position to starting y position as temp fix
        ClampYPosition();
    }

    private void ClampYPosition()
    {
        Vector3 position = transform.position;
        position.y = m_defaultY;
        transform.position = position;
    }

    private void WrapEdges()
    {
        if (m_wrapNorthSouth)
        {
            m_wrapTimer += Time.deltaTime;
            if (m_wrapTimer >= 0.2f)
            {
                m_wrapTimer = 0.0f;
                m_wrapNorthSouth = false;
            }
            return;
        }

        Camera cam = Camera.main;
        float bound = 0.1f;
        Vector3 pos = transform.position;
        Vector3 vPos = cam.WorldToScreenPoint(pos);
        bool willChangePos = false;

        if (vPos.x < -(bound*cam.pixelWidth))
        {
            vPos.x = cam.pixelWidth + (bound * cam.pixelWidth);
            willChangePos = true;
        }

        else if (vPos.x > cam.pixelWidth + (bound * cam.pixelWidth))
        {
            vPos.x = -(bound * cam.pixelWidth);
            willChangePos = true;
        }

        else if (vPos.y < -(bound * cam.pixelHeight))
        {
            vPos.y = cam.pixelHeight + (cam.pixelHeight * bound);
            Ray ray = cam.ScreenPointToRay(vPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Water")))
            {
                Vector3 newPos = hit.point;
                newPos.y = pos.y;
                transform.position = newPos;
                m_wrapNorthSouth = true;
                return;
            }
        }

        else if (vPos.y > cam.pixelHeight + (bound * cam.pixelHeight) )
        {
            vPos.y = -(bound * cam.pixelHeight);
            Ray ray = cam.ScreenPointToRay(vPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Water")))
            {
                Vector3 newPos = hit.point;
                newPos.y = pos.y;
                transform.position = newPos;
                m_wrapNorthSouth = true;
                return;
            }
        }

        if (willChangePos)
        {
            transform.position = cam.ScreenToWorldPoint(vPos);
        }
    }
}
