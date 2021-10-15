using UnityEngine;

public class Bullet : MonoBehaviour
{
	public BulletStats stats = new BulletStats();
	[SerializeField] bool isUp;
	[SerializeField] TrailRenderer trail;
	float travelled; [SerializeField] Rigidbody2D rb;


	void OnEnable()
	{
		//Reset trail if there is one
		if(trail != null) {trail.Clear();}
		//Reset the travelles distance
		travelled -= travelled;
		//Reset old position
		oldPos = transform.position;
	}

	void Update()
    {
		//Update the projectile velocity as the speed stats at transfrom up if needed
		if(isUp){rb.velocity = transform.up * stats.speed;}
		//Update the projectile velocity as the speed stats at transfrom right if needed
		else {rb.velocity = transform.right * stats.speed;}
    }

	Vector2 oldPos; void FixedUpdate()
	{
		//Get how many unit this frame has travel
		float frameTravel = Vector2.Distance(rb.position, oldPos);
		//Adding the time has travel per frame
		travelled += frameTravel;
		//If has travel all the range
		if(travelled >= stats.range)
		//Deactive the gameobject
		{gameObject.SetActive(false);}
		//Update the old position
		oldPos = rb.position;
	}

	private void OnCollisionEnter2D(Collision2D other)
	//Deactive the bullet if it collide with player
	{if(other.collider.CompareTag("Player")) {gameObject.SetActive(false); Player.i.Hurt();}}
}
