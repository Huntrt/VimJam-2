using UnityEngine; using System.Collections;
using System.Collections.Generic;

public class Warden : MonoBehaviour
{
	[SerializeField] BossHeath heath;
	[SerializeField] int stage;
	// [SerializeField][Tooltip("Small to Big")] int[] stages; int curretnStage;
	// The compare if heath smaller than an stage then return and mark that as current stage
	[SerializeField] ProjectileAttack projectileAttack1 = new ProjectileAttack();
	[SerializeField] ProjectileAttack projectileAttack2 = new ProjectileAttack();
	[SerializeField] BulletStats bullet = new BulletStats();
	[SerializeField] GameObject gradenadeAttack;
	[SerializeField] BombStats bomb = new BombStats();
	[SerializeField] GameObject laserAttack;
	[SerializeField] LaserStats laser = new LaserStats(); 
	[SerializeField] Laser lasering;
 	[SerializeField] Animator animator;
	[SerializeField] Transform firepoint;
	[SerializeField] CreateProjectile creator;
	float counter; bool isProjectileAttack;

	void Start()
	{
		//Mark warden as boss upon create
		BossManager.i.boss = gameObject.transform;
		//Send boss heath to manager
		BossManager.i.heath = heath;
		//Stage 1 when enter
		stage = 1;
	}

    void Update()
    {
		//Make the firepoint looking at player
		firepoint.up = (Player.i.transform.position - firepoint.position).normalized;
		//Begin automatic attack
		ProjectileAttacking();
		//If heath are below or equal and enter stage 2 then enter stage and update stats
		if(heath.heath <= 5 && stage != 2) {stage = 2; Stage2Stats();}
	}

	void ProjectileAttacking()
	{
		//Counting when counter not reached projectile rate counted with delay
        if(counter < projectileAttack1.rate) {counter += Time.deltaTime;}
		//If counter has reached rate while not attacking
		else if(!isProjectileAttack)
		{
			//Begin burst attack
			StartCoroutine(BurstAttack());
			//Are now currently attacking
			isProjectileAttack = true;
		}
	}
	IEnumerator BurstAttack()
	{
		//The time has burst attack
		int bursted = 0;
		//If still currnt burst attack 
		while (bursted < projectileAttack1.burst)
		{
			//Wait for an delay
			yield return new WaitForSeconds(projectileAttack1.delay);
			//Create empty list of attack has create
			List<GameObject> created = new List<GameObject>();
			//Make an shortcut for projectile stats
			ProjectileAttack stat = projectileAttack1;
			//Begin creating attack with spread and focus at firepoint position and rotation then get it
			creator.CreateGet(stat.attack, (int)stat.spread, stat.focus, firepoint.position, firepoint, out created);
			//Go through all the attack has been create
			foreach (GameObject create in created)
			{
				//Get the bullet stats of bullet created
				BulletStats stats = create.GetComponent<Bullet>().stats;
				//Update the bullet stats of bullet created once
				stats.speed = bullet.speed; stats.range = bullet.range;
				//Active the bullet and rename it to be reuse by pool
				create.SetActive(true); create.name = create.name.Replace(" Temp", "");
			}
			//Start projectile attack animation 
			animator.SetTrigger("Projectile Animation");
			//Go to the next burst
			bursted++;
		}
		//Reset counter and projectile no longer attacking
		counter -= counter; isProjectileAttack = false;
	}

	//Running animation toggle
	public void RunningAnim() {animator.SetBool("Is Running", !animator.GetBool("Is Running"));}

	public void BodyAttacking()
		{
			//Grenade attack if in stage 1 | Beam attack if in stage 2
			if(stage == 1) {GrenadeAttack();} if(stage == 2) {BeamAttack();}
		}
	#region Stage 1
		void GrenadeAttack()
		{
			//Create empty list of attack has create
			List<GameObject> created = new List<GameObject>();
			//Begin creating attack with spread and focus at firepoint position and rotation then get it
			creator.CreateGet(gradenadeAttack, 1, 0, firepoint.position, firepoint, out created);
			//Get the first created object then get it bomb stats of bomb create
			GameObject create = created[0]; BombStats stats = create.GetComponent<Bomb>().stats;
			//Update the bomb stats of bomb created once
			stats.speed = bomb.speed; stats.radius = bomb.radius;
			//Mark the bomb destination as player and set it explosion
			stats.destination = Player.i.transform.position; stats.explosion = bomb.explosion;
			//Active the bomb and rename it to be reuse by pool
			create.SetActive(true); create.name = create.name.Replace(" Temp", "");
		}
	#endregion

	#region Stage 2
		void Stage2Stats()
		{
			//Use stage 2 stats of projectile attack
			projectileAttack1 = projectileAttack2;
		}

		public void BeamAttack()
		{
			//Update the laser attack stats
			lasering.stats.size = laser.size;lasering.stats.range = laser.range;
			lasering.stats.windUp = laser.windUp;
			//Mark the laser target as player
			lasering.stats.target = Player.i.transform.position;
			//Active the laser attack
			laserAttack.SetActive(true);
		}
	#endregion

	void OnDisable() 
	{
		//Destroy all the boss projectile upon die
		foreach (GameObject obj in Pool.get.objectsPool){if(obj.CompareTag("Boss Attack") && obj != null){Destroy(obj);}}
		//Unlock dash when warden die
		if(gameObject != null && heath.heath <= 0) AbilityManager.i.UnlockDash();
	}
}
