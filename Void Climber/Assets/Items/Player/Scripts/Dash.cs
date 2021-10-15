using UnityEngine;

public class Dash : MonoBehaviour
{
	public float distance, speed, cooldown;
	float travelled;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] bool ready = true;
	[SerializeField] SpriteRenderer dashing;
	[SerializeField] AudioClip dashAudio;
	Quaternion preRotation;
	Player player;
	public bool isDash;

    void Start()
    {
		//Get the player
		player = Player.i;
    }

	void Update()
	{
		//Dash if pressed it key
        if(Input.GetKey(GameManager.ins.dash)) {Dashing();}
	}

	Vector2 oldPos; void FixedUpdate()
	{
		if(isDash)
		{
			//Move rigidbody from current position at player direction with speed
			rb.MovePosition(rb.position + (Vector2)transform.up * speed * Time.fixedDeltaTime);
			//Get the distance has travelled
			travelled += Vector2.Distance(rb.position, oldPos);
			//Stop dash if has travelled
			if(travelled >= distance) {StopDash();}
			//Update previous position
			oldPos = rb.position; 
		}
	}

	void Dashing()
	{
		//If ready to dash
		if(ready)
		{
			GameManager.ins.sound.PlayOneShot(dashAudio);
			//Reset travelled and previous position
			travelled -= travelled; oldPos = rb.position;
			//Save the players rotation before dash
			preRotation = transform.rotation;
			//Get the direction to dash
			Vector2 dir = Vector2.zero;
			//Dash toward the moving direction if the player are moving
			if(player.inputDirection != Vector2.zero) {dir = player.inputDirection;}
			//Dash toward mouse if the player are not moving
			else {dir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)-rb.position).normalized;}
			//Player look toward dash direction
			transform.up = dir;
			//Enable dash sprite
			dashing.enabled = true; player.head.enabled = false; player.leg.enabled = false;
			//Reset bool
			isDash = true; ready = false;
		}
	}

	public void StopDash()
	{
		//Prevent invoke overlap
		CancelInvoke("ResetReady");
		//Disable dash sprite
		dashing.enabled = false; player.head.enabled = true; player.leg.enabled = true;
		//Reseting the plyer rotation back to pre dash
		transform.rotation = preRotation;
		//No longer dash
		isDash = false;
		//Reseting ready
		Invoke("ResetReady", cooldown);
	}

	void ResetReady() {ready = true;}
}