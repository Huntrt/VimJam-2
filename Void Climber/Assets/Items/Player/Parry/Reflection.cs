using UnityEngine;

public class Reflection : MonoBehaviour
{
	public BulletStats stats = new BulletStats();
	float travelled; [SerializeField] Rigidbody2D rb;
	[SerializeField] TrailRenderer trail;

	void OnEnable()
	{
		//Get the stats from parry
		stats = Player.i.parry.projectile;
		//Re assign the trail parent and reset it position then clear it
		trail.transform.position = transform.position;trail.transform.parent = transform;trail.Clear();
		//Reset the travelles distance
		travelled -= travelled;
		//Reset old position
		oldPos = transform.position;
	}

	void Update()
    {
		//Update the projectile velocity as the speed stats at transfrom up
		rb.velocity = transform.up * stats.speed;
    }

	Vector2 oldPos; void FixedUpdate()
	{
		//Get how many unit this frame has travel
		float frameTravel = Vector2.Distance(rb.position, oldPos);
		//Adding the time has travel per frame
		travelled += frameTravel;
		//Remove the gameobject if has travel all the range
		if(travelled >= stats.range) {Remove();}
		//Update the old position
		oldPos = transform.position;
	}

	private void OnCollisionEnter2D(Collision2D other)
	//If it collide with boss deal damage to it and remove the attack
	{if(other.collider.CompareTag("Boss")) {other.gameObject.GetComponent<BossHeath>().Hurt(1);Remove();}}

	void Remove()
	{
		//Deacitve trial
		trail.transform.parent = null;
		//Deactive the attack;
		gameObject.SetActive(false);
	}
}
