using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Swordman : MonoBehaviour
{
	public BossHeath heath;
	int stage = 1;
	public float chasingSpeed, range;
	bool moving = true;
	[Header("Stage 1")]
	public float meleeRate;
	[SerializeField] GameObject lighting;
	[SerializeField] float dashSpeed;
	[SerializeField] GameObject meleeAttack; 
	bool readyMelee = true, isAttacking = false;
	[Header("Stage 2")]
	public float chasingSpeed_2; public float range_2;
	[SerializeField] float accuracy;
	[SerializeField] GameObject blade;
	[SerializeField] Sprite[] stage2visual;
	[SerializeField] Vector2 newFirepoint;
	[Header("Stage 3")]
	[SerializeField] float chargeSpeed; [SerializeField] float chargeLength, chargeWait;
	[SerializeField] GameObject trail, hitbox;
	[SerializeField] Sprite[] stage3visual;
	[SerializeField] Vector2 newBodyPos;
	[Header("Misc")]
	[SerializeField] Transform firepoint;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Animator animator;
	[SerializeField] AudioClip chargeAudio, strikeAudio, rangeAudio;

    void Start()
	{
		//Mark warden as boss upon create
		BossManager.i.boss = gameObject.transform;
		//Send boss heath to manager
		BossManager.i.heath = heath;
		//Start special attack every 5 seconds
		InvokeRepeating("SpecialAttack", 5, 5);
		//Enter stage 3 of total boss
		BossManager.i.stage = 2;
	}

	void Update()
	{
		//If heath below 5 and has not enter stage 2
		if(heath.heath <= 5 && stage < 2) 
		{	
			//No longer attacking
			isAttacking = false;
			//Enter stage 2
			stage = 2;
			//No longer able to use special attack
			CancelInvoke("SpecialAttack");
			//Enter stage 2 animator then the animtion will attack
			animator.SetInteger("Stage", 2);
			//Enter stage 2 stats
			chasingSpeed = chasingSpeed_2; range = range_2;
			//Move the firepoint to it new position
			firepoint.transform.localPosition = newFirepoint;
			//Use sprite 2 visual
			heath.parts[0].sprite = stage2visual[0]; heath.parts[1].sprite = stage2visual[1];
		}
		//If heath below 2 and has not enter stage 3
		if(heath.heath <= 2 && stage < 3) 
		{	
			//Reset firepoint rotation
			firepoint.localRotation = Quaternion.identity;
			//Enter stage 3
			stage = 3;
			//Enter stage 3 animator
			animator.SetInteger("Stage", 3);
			//Set layer to ignore all
			gameObject.layer = LayerMask.NameToLayer("Ignore All");
			//No longer need to move
			moving = false;
			//Move the body to middle
			transform.GetChild(1).transform.localPosition = newBodyPos;
			//Use sprite 3 visual
			heath.parts[0].sprite = stage3visual[0]; heath.parts[1].sprite = null;
			//Deactive the shadow and animator
			transform.GetChild(3).gameObject.SetActive(false); animator.enabled = false;
			//Active trial and hit box
			trail.SetActive(true); hitbox.SetActive(true);
			//Get the direction toward player
			direction = (Player.i.transform.position - transform.position).normalized;
			//Look toward direction
			transform.right = direction;
			//Reset charge after wait
			Invoke("ResetCharge", chargeWait);
		}
		//Charge function
		Charging();
		//Dash function
		Dashing();
	}

	void FixedUpdate()
	{
		//Only move if needed
		if(moving) Movement();
	}

	bool flip; void Movement()
	{
		//Get the player position
		Vector2 playerPos = Player.i.transform.position;
		//Only flip is not attacking
		if(!isAttacking)
		{
			//If the player are on the right of boss, flip the player Y rotation
			if(playerPos.x>transform.position.x) {transform.rotation=Quaternion.Euler(0,180,0);flip=true;}
			//If the player are on the left of boss, unflip the player Y rotation
			if(playerPos.x<transform.position.x) {transform.rotation=Quaternion.Euler(0,0,0);flip=false;}
		}
		//Get the distance between player position and boss
		float playerDistance = Vector2.Distance(playerPos, transform.position);
		//Vector for movement and direction of boss to player
		Vector2 movement = Vector2.zero;
		//Direction toward player
		Vector3 direction = (playerPos - (Vector2)transform.position).normalized;
		//Speed will to move
		float speed = 0;
		//If not current melee attack
		if(!isAttacking)
		{
			//Set speed to chaser speed
			speed = chasingSpeed;
			//Start walking animation
			animator.SetBool("Is Walking", true);
			//If in range
			if(playerDistance < range)
			{
				//No longer need to move
				speed = 0;
				//If in stage 1
				if(stage == 1)
				{
					//If ready to melee attack
					if(readyMelee)
					//Begin melee attack and it animation
					{readyMelee = false;isAttacking = true;animator.SetTrigger("Melee Attacking");}
				}			
				//Stop walking animation
				animator.SetBool("Is Walking", false);
			}
		}
		//No longer need to move if currently melee attack
		else {speed = 0;}
		//If not dashing
		if(!dashing)
		{
			//Moving toward player into distance using direction with modified speed 
			movement = transform.position + direction * (speed * Time.fixedDeltaTime);
			//Moving rigidbody position
			rb.MovePosition(movement);
		}
	}

	#region Stage 1

	public void StartMelee() ///Start melee via animation
	{
		GameManager.ins.sound.PlayOneShot(strikeAudio);
		//Reset melee bas on rate
		Invoke("ResetMelee", meleeRate);
		//Create 3 projectile
		CreateProjectile(3, 30);
		//Active melee attack
		meleeAttack.SetActive(true);
		//No longer melee attack
		isAttacking = false;
	}

	void CreateProjectile(int spread, float focus)
	{
		
		//Range are the focus got divide by 2 since it affect 2 direction
		float range = focus / 2;
		//Get create projectile in the horizontal base on the flip
		float center = 0; if(flip) {center = -90;} else {center = 90;}
		//Get the start and end rotation by decrease and increase the center with range
		float start = center - range; float end = center + range;
		//Get the distance between each spread using the total focus (-1 there an note about it)
		float step = focus / (spread-1);
		//Begin the first angle at start
		float angle = start;
		//For each of the projectile need to create
		for (int i = 0; i < spread; i++)
		{
			//Create an send projectile by pool at point position with the rotation of spread angle
			Pool.get.Object(lighting, firepoint.position, Quaternion.Euler(0,0,angle), true);
			//Proceed to the next step
			angle += step;
		}
	}

	//Ready to melee attack
	void ResetMelee() {readyMelee = true;}

	void SpecialAttack()
	{
		//Randomly chose between 3 attack
		int chose = Random.Range(1,4);
		//Are now melee
		isAttacking = true;
		//Start animation base on random chosed
		animator.SetInteger("Special", chose);
	}

	public void Thrust()
	{
		GameManager.ins.sound.PlayOneShot(strikeAudio);
		//Starting burst
		StartCoroutine(ThurstBurst());
	}

	IEnumerator ThurstBurst()
	{
		//The time has burst attack
		int bursted = 0;
		//If still currnt burst attack 
		while (bursted < 6)
		{
			//Wait for an delay
			yield return new WaitForSeconds(0.1f);
			//Get create projectile in the horizontal base on the flip
			float angle = 0; if(flip) {angle = -90;} else {angle = 90;}
			//Create an send projectile by pool at point position with the rotation of angle
			Pool.get.Object(lighting, firepoint.position, Quaternion.Euler(0,0,angle), true);
			//Go to the next burst
			bursted++;
		}
	}

	public void Semicircle()
	{
		GameManager.ins.sound.PlayOneShot(strikeAudio);
		//Range are the focus got divide by 2 since it affect 2 direction
		float range = 90 / 2;
		//Get create projectile in the horizontal base on the flip
		float center = 0; if(flip) {center = -90;} else {center = 90;}
		//Get the start and end rotation by decrease and increase the center with range
		float start = center - range; float end = center + range;
		//Get the distance between each spread using the total focus (-1 there an note about it)
		float step = 90 / (10-1);
		//Begin the first angle at start
		float angle = start;
		//For each of the projectile need to create
		for (int i = 0; i < 10; i++)
		{
			//Create an send projectile by pool at point position with the rotation of spread angle
			Pool.get.Object(lighting, firepoint.position, Quaternion.Euler(0,0,angle), true);
			//Proceed to the next step
			angle += step;
		}
	}

	Vector2 target; bool dashing = false; public void Dash()
	{
		GameManager.ins.sound.PlayOneShot(strikeAudio);
		//Target are 2 unit behind player
		target = Player.i.transform.position;
		//Are now dashing
		dashing = true;
	}

	void Dashing()
	{
		//If are dashing
		if(dashing)
		{
			//Dashing toward the target with dash speed
			transform.position = Vector2.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
			//If has reached target
			if(Vector2.Distance(transform.position, target) == 0) 
			{
				//Reset animation special chosed	
				animator.SetInteger("Special", 0);
				//No longer melee
				isAttacking = false;
				//End dash
				dashing = false; animator.SetTrigger("End Dash");
			}
		}
	}

	public void ResetSpecial()
	{
		//Reset animation special chosed	
		animator.SetInteger("Special", 0);
		//No longer melee
		isAttacking = false;
	}

	#endregion

	#region Stage 2
	
	public void RangeAttack() ///Start via animation when in stage 2 and end when in stage 3
	{
		//Fire point look at player
		firepoint.up = (Player.i.transform.position - transform.position).normalized;
		//Get the 180 to -180 rotation of the emitter
		float rot = firepoint.localEulerAngles.z; float center = (rot > 180) ? rot-360 : rot;
		//Get the start and end rotation by decrease and increase the center with range
		float aim = center + Random.Range(-accuracy, accuracy);
		//Create an send projectile by pool at point position and rotation
		Pool.get.Object(blade, firepoint.position, Quaternion.Euler(0, 0, aim), true);
		GameManager.ins.sound.PlayOneShot(rangeAudio);
	}

	#endregion

	#region Stage 3

	bool isCharge = false; float travelled; Vector2 direction; public void ResetCharge()
	{
		//Reseting charge info
		oldPos = rb.position; travelled -= travelled;
		//Begin charge
		isCharge = true;
	}

	Vector2 oldPos;bool hasAudio = false;void Charging()
	{
		//If is charging
		if(isCharge)
		{
			if(!hasAudio){GameManager.ins.sound.PlayOneShot(chargeAudio); hasAudio = true;}
			//Move rigidbody from current position at direction with speed
			rb.MovePosition(rb.position + direction * chargeSpeed * Time.fixedDeltaTime);
			//Get the distance has travelled
			travelled += Vector2.Distance(rb.position, oldPos);
			//If has travelled all the length
			if(travelled >= chargeLength) 
			{
				//No longer charge
				isCharge = false;
				hasAudio = false;
				//Get the direction toward player
				direction = (Player.i.transform.position - transform.position).normalized;
				//Look toward direction
				transform.right = direction;
				//Reset charge after wait
				Invoke("ResetCharge", chargeWait);
			}
			//Update previous position
			oldPos = rb.position; 
		}
	}

	#endregion

	void LateUpdate()
	{
		//Is attack or stage 3
		if(isAttacking || stage == 3)
		{
			//distance between sword man and player
			float distance = Vector2.Distance(Player.i.transform.position, transform.position);
			//If distance with size in panic radius thenthe player are in danger
			if(distance <= Player.i.panic.radius + 1) {Player.i.panic.inDanger = true;}
		}
	}

	void OnDisable() 
	{
		//Destroy all the boss projectile upon die
		foreach (GameObject obj in Pool.get.objectsPool){if(obj.CompareTag("Boss Attack") && obj != null)
		{Destroy(obj);}}
		//Unlock blade when swordman die
		if(gameObject != null && heath.heath <= 0) AbilityManager.i.UnlockBlade();
	}

}