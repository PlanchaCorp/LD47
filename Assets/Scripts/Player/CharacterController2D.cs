using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour, Living
{
    [SerializeField] private float m_JumpForce = 800f;
    [Range(0,1f)][SerializeField] private float airControlPenality = 0.3f; 							// Amount of force added when the player jumps.
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[Range(0, .3f)] [SerializeField] private float m_accelerateSmoothing = .03f;	// How much to smooth out the acceleration
	[Range(0, .3f)] [SerializeField] private float m_decelerateSmoothing = .01f;	// How much to smooth out the decelerate
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_WallCheck;

    [Range(0,1f)]
    [SerializeField]
	private float k_GroundedRadius = .3f; // Radius of the overlap circle to determine if grounded
    [Range(0,1f)]
    [SerializeField]
	private float k_WalledRadius = .6f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.

    private bool m_Walled;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

    private SpriteRenderer spriteRenderer;
    
    private TimeController timeController;

    private bool flipX;

    public bool blockedOnScreen = false;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

    private void Start() {
        timeController = GameObject.FindGameObjectWithTag("Rewinder").GetComponent<TimeController>();
    }

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
        m_Walled = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] groundColliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		Collider2D[] wallColliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < groundColliders.Length; i++)
		{
			if (groundColliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
		for (int i = 0; i < wallColliders.Length; i++)
		{
			if (wallColliders[i].gameObject != gameObject)
			{
				m_Walled = true;
			}
		}
	}


	public void Move(float move, bool jump){
    Vector3 targetVelocity;
    //only control the player if grounded or airControl is turned on
    if (m_Grounded){
        targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
    } else {
          //if character is not on ground, reduce the speed so he doesn't jump too far away
        float airControlAccelerationLimit = 0.7f;  // Higher = more responsive air control
        float airSpeedModifier = 0.7f; // the 0.7f in your code, affects max air speed
        float targetHorizVelocity = move * 10f * airSpeedModifier;  // How fast we are trying to move horizontally
        float targetHorizChange = targetHorizVelocity - m_Rigidbody2D.velocity.x; // How much we want to change the horizontal velocity
        float horizChange = Mathf.Clamp(
                targetHorizChange ,
                -airControlAccelerationLimit , 
                airControlAccelerationLimit ); // How much we are limiting ourselves 
                                               // to changing the horizontal velocit
        targetVelocity = new Vector2(m_Rigidbody2D.velocity.x + horizChange, m_Rigidbody2D.velocity.y);
    }
    if(move > 0.1f || move < -0.1f){
        m_Rigidbody2D.velocity =  targetVelocity;
    } else {
    m_Rigidbody2D.velocity = targetVelocity;
    }
    if(m_Rigidbody2D.velocity.magnitude > 30){
        m_Rigidbody2D.velocity = Vector3.ClampMagnitude(m_Rigidbody2D.velocity, 30);
    }
    // If the input is moving the player right and the player is facing left...
    if (move > 0 && !m_FacingRight)
    {
        // ... flip the player.
        Flip();
    }
    // Otherwise if the input is moving the player left and the player is facing right...
    else if (move < 0 && m_FacingRight)
    {
        // ... flip the player.
        Flip();
    }
    // If the player should jump...
    if (m_Grounded && jump)
        {
    // Add a vertical force to the player.
        m_Grounded = false;
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		spriteRenderer.flipX = m_FacingRight ;
        m_FacingRight = !m_FacingRight;
	}

    public void Death(GameObject cause)
    {
        Debug.Log("killed by" + cause.name);
        timeController.StartRewind();
    }

    public void Win()
    {
        GameObject.Find("GameMenu").GetComponent<InGameMenuBehaviour>().ToggleState("stop");
        GameObject.Find("LevelManager").GetComponent<LevelLoading>().canGoNext =true;
        GameObject.Find("CameraGroup").GetComponent<CameraController>().canMove = false;
        Destroy(gameObject);
    }
}
