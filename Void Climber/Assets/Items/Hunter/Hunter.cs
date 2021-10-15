using UnityEngine;

public class Hunter : MonoBehaviour
{
	[SerializeField] BossHeath heath;
	[SerializeField] int stage; bool isSpecial;
	[SerializeField] float useSpeed; [SerializeField] float useDistance;
	[SerializeField] int useSpread; [SerializeField] float useFocus;
	[SerializeField] GameObject useProjectile; [SerializeField] float useRate;
	[SerializeField] float specialRate; [SerializeField] string useSpecial;
	[Header("Stage 2")]
	[SerializeField] float speed2; [SerializeField] float distance2;
	[SerializeField] GameObject projectile2; [SerializeField] float rate2;
	[SerializeField] int spread2; [SerializeField] float focus2;
	[SerializeField] GameObject special2Attack;
	[SerializeField] string special2;
	[SerializeField] Color color2;
	[SerializeField] Sprite head2;
	[Header("Misc")]
	[SerializeField] Transform firepoint, hand;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] SpriteRenderer spriteHand;
	[SerializeField] CreateProjectile creator;
	[SerializeField] Animator animator;
	
	Vector2 targetPos;

    void Start()
    {
		//Begin repeating attack using rate
        InvokeRepeating("Attacking", useRate, useRate);
		//Begin repeating special using rate
        InvokeRepeating("Specialing", specialRate, specialRate);
		//Mark warden as boss upon create
		BossManager.i.boss = gameObject.transform;
		//Send boss heath to manager
		BossManager.i.heath = heath;
		//Enter stage 2 of total boss
		BossManager.i.stage = 1;
		//Stage 1 when enter
		stage = 1;
    }

	void Update()
	{
		//If heath below 4 and not enter to 2
		if(heath.heath <= 5 && stage < 2)
		{
			//Enter stage 2
			stage = 2;
			//Change heah bar color to phase 2
			heath.heathbarColor = color2;
			//Up stats to stage 2 stats
			useSpeed = speed2; useSpread = spread2; useDistance = distance2;
			useFocus = focus2; useRate = rate2; useProjectile = projectile2; useSpecial = special2;
			//Update sprite color to color 2
			for (int c = 0; c < heath.defaultColor.Length; c++) {heath.defaultColor[c] = color2;}
			//Use sprite2 heath
			heath.parts[0].sprite = head2;
		}
	}

	void Attacking()
	{
		//Start attacking animation if not special
		if(!isSpecial) animator.SetTrigger("Attacking");
	}

	public void CreateAttack()
	{
		//If not is special
		if(!isSpecial)
		{
			//Randomly chose spread and focus
			int spread = Random.Range(1, useSpread+1); float focus = Random.Range(10, useFocus);
			//Create projectile with an spread and focus at firepoint
			creator.Create(useProjectile, spread, focus, firepoint.position, hand);
		}
	}

	public void Specialing()
	{
		//STart usae special animation
		animator.SetTrigger(useSpecial);
		//Are now using special
		isSpecial = true;
	}

	public void Special1()
	{
		//Create an massive wave of arrow
		creator.Create(useProjectile, 12, 150, firepoint.position, hand);
		//No longer use special
		isSpecial = false;
	}

	
	public void Special2()
	{
		//Create an bomb that create rrow
		creator.Create(special2Attack, 1, 0, firepoint.position, hand);
		//No longer use special
		isSpecial = false;
	}


	void FixedUpdate(){Moving();} void LateUpdate(){WeaponRotating();}

	void Moving()
	{
		//Get the distance between player position and distancer
		float playerDistance = Vector2.Distance(Player.i.transform.position, transform.position);
		//Save the player position
		Vector2 playerPos = Player.i.transform.position;
		//Vector for direction to player
		Vector3 direction = (Player.i.transform.position - transform.position).normalized;
		//Vector for movement
		Vector2 movement = Vector2.zero;
		//If the player got too close to distancer 
		if(playerDistance < useDistance)
		{
			//Keep distance away from player as opposite direction
			movement = transform.position - direction * Time.fixedDeltaTime * useSpeed;
		}
		//Approach if the distancer haven't got in range while not running
		if(playerDistance > useDistance+1)
		{
			//Moving toward player into range using direction
			movement = transform.position + direction * Time.fixedDeltaTime * useSpeed;
		}
		//Stop moving if has reach safe spot
		else {movement = rb.position;}
		//Moving rigidbody position
		rb.MovePosition(movement);
	}

	void WeaponRotating()
	{
		//Get the player position as the target position
		targetPos = Player.i.transform.position;
		//Make the anchor's green axis look at target position
		hand.right = (targetPos - (Vector2)hand.position).normalized;
		//If the target are behind the weapon (t -0)
		if(targetPos.x < hand.transform.position.x)
		{
			//Set the firepoint local X axis using opposite of offset
			firepoint.localPosition = new Vector2(firepoint.localPosition.x, 0);
			//Flipping the weapon
			spriteHand.flipY = true;
		}
		//If the target are infront the weapon (0- t)
		else
		{
			//Set the firepoint local X axis using offset
			firepoint.localPosition = new Vector2(firepoint.localPosition.x, 0);
			//Unflip the weapon
			spriteHand.flipY = false;
		}
	}

	void OnDisable() 
	{
		//Destroy all the boss projectile upon die
		foreach (GameObject obj in Pool.get.objectsPool){if(obj.CompareTag("Boss Attack") && obj != null) {Destroy(obj);}}
		//Only if die
		if(heath.heath <= 0)
		{
			//Unlock bow when hunter die
			if(gameObject != null && heath.heath <= 0) AbilityManager.i.UnlockBow();
		}
	}

}
