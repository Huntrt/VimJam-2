using UnityEngine;

public class Player : MonoBehaviour
{
	public float speed;
	public int dies;
	public Vector2 inputDirection;
	[SerializeField] bool touchWall;
	public Transform firepoint;
	public Rigidbody2D Rigidbody; public static Player i;
	public Animator animator;
	public bool invincible;
	[SerializeField] Color defaultColor, flickColor;
	[SerializeField] float flickerRate;
	public SpriteRenderer shield ,head, leg;
	[SerializeField] Sprite[] look;
	public int layer;
	[SerializeField] Camera main;
	[SerializeField] TMPro.TextMeshProUGUI dieCounter;
	[Header("Ability")]
	public Panic panic;
	public Parry parry;
	public Dash dash;
	public Bow bow;
	public Blade blade;

	//Make this script into singleton
	void Awake() {i = this;}

	void Start()
	{
		//Unlock parry upon start
		AbilityManager.i.UnlockParry();
		//Save the player's layer index
		layer = LayerMask.NameToLayer("Player");
		//Get the main camera
		main = Camera.main;
	}
	
    void Update()
    {
		//Moving function
		MoveInput(); MoveAnimation();
		//DIsplay die amount
		dieCounter.text = dies.ToString();
	}

	public void Hurt()
	{
		//If not invincible
		if(!invincible && gameObject.activeInHierarchy) 
		//Increase die counter then respawn
		{dies++;BossManager.i.Respawn();gameObject.SetActive(false);}
	}

	public void Invincible(float dur) 
	{
		//Cancel invoke to prevent overlap
		CancelInvoke("Clearinvincible"); CancelInvoke("Flickering");
		//Begin invincible
		invincible = true;
		//Change the tag to invincible
		gameObject.tag = "Invincible";
		//Change the tag to only collide with border
		gameObject.layer = LayerMask.NameToLayer("Ignore All Except Border");
		//Reset then begin flicking sprite 
		shield.color = defaultColor; InvokeRepeating("Flickering", 0, flickerRate);
		//Start countdown to clear invincible after set duration
		Invoke("Clearinvincible", dur);
	}

	void Flickering()
	{
		//If shield are not flickering then flicker it and stop reset to default
		if(shield.color != flickColor) {shield.color = flickColor; return;}
		//If shield are flickering then reset it to defaut it to default
		if(shield.color != defaultColor) {shield.color = defaultColor; return;}
	}

	void Clearinvincible() 
	{
		//Stop flickering and reset shield color back to default
		CancelInvoke("Flickering"); shield.color = defaultColor;
		//Reset the tag
		gameObject.tag = "Player";
		//Reset the layer
		gameObject.layer = LayerMask.NameToLayer("Player");
		//No longer invincible
		invincible = false;
	}

	Vector2 velocity; void MoveInput()
	{
		//Set the input horizontal and vertical direction
		inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"),0);
		//Get the raw input direction if touch border (faster than set speed)
		if(touchWall) {velocity = inputDirection;}
		//Get the normalized direction if not touching border (equal set speed)
		else {velocity = inputDirection.normalized;}
        //Add the player speed to velocity
        velocity *= speed;
	}

	void MoveAnimation()
	{
		//If input direction are changing then start bool to walking animation
		if(inputDirection != Vector2.zero) {animator.SetBool("Walking", true);} 
		//If input direction are not changing then stop bool to walking animation
		else {animator.SetBool("Walking", false);}
	}

	void FixedUpdate()
	{
		//If not dashing
		if(!dash.isDash)
		//Moving the player
		Rigidbody.MovePosition(Rigidbody.position + velocity * Time.fixedDeltaTime);
	}

	void LateUpdate()
	{
		///Get mouse position in own line since there an delay
		//If there are main camera 
		if(main.isActiveAndEnabled)
		{
			//Make the weapon rotate toward mouse
			firepoint.up = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
			//Compare direction between player X axis and mouse Y axis
			bool flipHor = false; if(transform.position.x<=Camera.main.ScreenToWorldPoint(Input.mousePosition).x) 
			//Flip the horizontal of an sprite base on it
			{flipHor = true;} else {flipHor = false;} head.flipX = flipHor; leg.flipX = flipHor;
			//Compare direction between player Y axis and mouse Y axis
			if(transform.position.y <= Camera.main.ScreenToWorldPoint(Input.mousePosition).y)
			//Flip change the head sprite base on it  
			{head.sprite = look[1];} else {head.sprite = look[0];}
		}
	}

	private void OnCollisionEnter2D(Collision2D other) 
	{
		//Hurt if collide with an boss attack
		if(other.transform.CompareTag("Boss Attack")) {Hurt();}
	}

	private void OnCollisionStay2D(Collision2D other) 
	{
		//Is collide with wall
		if(other.transform.CompareTag("Wall")) {touchWall = true;}
		//Stop dash if collide with anything
		if(dash.isActiveAndEnabled){dash.StopDash();}
	}

	private void OnCollisionExit2D(Collision2D other) 
	{
		//Are no longer collision with wall
		if(other.transform.CompareTag("Wall")) {touchWall = false;}
	}
}