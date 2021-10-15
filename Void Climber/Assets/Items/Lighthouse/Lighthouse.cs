using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Lighthouse : MonoBehaviour
{
	[SerializeField] BossHeath heath;
	[SerializeField] Transform firepoint;
	[SerializeField] GameObject projectile;
	[SerializeField] float attackRate;
	[SerializeField] string[] attackPattern;
	[SerializeField] BombStats bomb = new BombStats();
	[SerializeField] GameObject bombProjectile;
	[SerializeField] Transform bombArea;
	[SerializeField] float bombRate, bombFrequent;
	[SerializeField] LineRenderer laser;
	[SerializeField] CreateProjectile creator;

	void Start()
	{
		//Start main attack ervey set time
		InvokeRepeating("MainAttack", attackRate, attackRate);
		//Start main attack ervey set time
		InvokeRepeating("Bombing", bombRate, bombRate);
		//Mark warden as boss upon create
		BossManager.i.boss = gameObject.transform;
		//Send boss heath to manager
		BossManager.i.heath = heath;
		//Enter stage 4 of total boss
		BossManager.i.stage = 3;
	}

	void Update()
	{
		//Make firepoint look toward mouse position
		firepoint.right = Player.i.transform.position - firepoint.position;
	}

	void MainAttack()
	{
		//Chose between all pattern
		int chosed = Random.Range(0, attackPattern.Length);
		//Start the pattern has chose
		Invoke(attackPattern[chosed]+"Attack", 0);
	}

	void ConeAttack()
	{
		//Create projectile in an cone pattern
		creator.Create(projectile, 11, 90, transform.position, firepoint);
	}

	void CircleAttack()
	{
		//Create projectile in an circle pattern
		creator.Create(projectile, 25, 360, transform.position, firepoint);
	}

	void BurstAttack()
	{
		//Begin burst
		StartCoroutine("Burst");
	}

	IEnumerator Burst()
	{
		//The time has burst attack
		int bursted = 0;
		//If still currnt burst attack 
		while (bursted < 10)
		{
			//Wait for an delay
			yield return new WaitForSeconds(0.2f);
			//Create an send projectile by pool at point position with firepoint rotation
			Pool.get.Object(projectile, firepoint.position, firepoint.rotation, true);
			//Go to the next burst
			bursted++;
		}
	}

	void Bombing()
	{
		//Create multiple bomb
		for (int i = 0; i < bombFrequent; i++)
		{
			//Arena follow player
			bombArea.position = Player.i.transform.position;
			//Set the bomb position
			Vector2 bombPosition = new Vector2
			//Randomly chose X position using the area scale and it current position
			(bombArea.position.x + Random.Range(-bombArea.localScale.x/2, bombArea.localScale.x/2),
			//Randomly chose Y position using the spawnerpoint scale and it current position
			bombArea.position.y + Random.Range(-bombArea.localScale.y/2, bombArea.localScale.y/2));
			//Create empty list of attack has create
			List<GameObject> created = new List<GameObject>();
			//Begin creating attack with spread and focus at firepoint position and rotation then get it
			creator.CreateGet(bombProjectile, 1, 0, firepoint.position, firepoint, out created);
			//Get the first created object then get it bomb stats of bomb create
			GameObject create = created[0]; BombStats stats = create.GetComponent<Bomb>().stats;
			//Update the bomb stats of bomb created once
			stats.speed = bomb.speed; stats.radius = bomb.radius;
			//Mark the bomb destination as bomb position and set it explosion
			stats.destination = bombPosition; stats.explosion = bomb.explosion;
			//Active the bomb and rename it to be reuse by pool
			create.SetActive(true); create.name = create.name.Replace(" Temp", "");
		}
	}

	void OnDisable() 
	{
		//Destroy all the boss projectile upon die
		foreach (GameObject obj in Pool.get.objectsPool) {if(obj.CompareTag("Boss Attack") && obj != null){Destroy(obj);}}
		//Win the game if the boss death
		if(heath.heath <= 0) {BossManager.i.Victory();}
	}
}
