using UnityEngine;

public class Parry : MonoBehaviour
{
	Animator animator;
	Transform point;
	public float delay, radius, grace;
	[SerializeField] GameObject reflect, muzzle;
	[SerializeField] AudioClip parryAudio;
	public BulletStats projectile = new BulletStats();
	bool ready = true;

	void Start()
	{
		//Get the player firepoint
		point = Player.i.firepoint;
		//Get the player animator
		animator = Player.i.animator;
	}

    void Update()
    {
		//Parry if pressed it key
        if(Input.GetKey(GameManager.ins.parry))
		{
			//Ready to parry
			if(ready)
			{
				//Play the parry animation
				animator.SetTrigger("Parrying");
				//Begin parry
				StartParry();
				//No longer ready then reset the parry after set delay
				ready = false; Invoke("ResetParry", delay);
			}
		}
    }

	//Reseting the parry
	void ResetParry() {ready = true;}

	void StartParry()
	{
		//Create an hitbox using overlap circle at this position with set radius
		Collider2D[] hitbox = Physics2D.OverlapBoxAll(transform.position, new Vector2(radius,radius), 0);
		//If hit box hit some thing
		if(hitbox != null)
		{
			bool playParry = false;
			//Go through all the collider that got hit inside hitbox
			foreach (Collider2D hit in hitbox)
			{
				//If hit an boss attack and there are boss
				if(hit.CompareTag("Boss Attack") && BossManager.i.boss != null)
				{
					if(!playParry) {GameManager.ins.sound.PlayOneShot(parryAudio); playParry = true;}
					//Give an short amount of invincible
					Player.i.Invincible(grace);
					//Deactive it
					hit.gameObject.SetActive(false);
					//Get the hit position
					Vector3 pos = hit.transform.position;
					//Create the reflect at hit with point rotation then save it transform
					Transform created = Pool.get.Object(reflect, pos, point.rotation).transform;
					//Make the created transform looking at current boss
					created.up = BossManager.i.boss.position - pos;
					//Active the created object
					created.gameObject.SetActive(true);
					//Create the muzzle flash at hit position with the created rotation
					Pool.get.Object(muzzle, pos, created.rotation, true);
				}
			}
		}
	}
}
