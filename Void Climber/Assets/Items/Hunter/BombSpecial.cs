using UnityEngine;

public class BombSpecial : MonoBehaviour
{
	public BombStats stats = new BombStats();
	[SerializeField] GameObject arrow;
	float travelled; float range; [SerializeField] float goal;
	[SerializeField] bool drop, fade;
	[SerializeField] SpriteRenderer sprite;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] CreateProjectile creator;
	//Save the player
	Player player; void Awake() {player = Player.i;}

	void OnEnable()
	{
		//Bomb looking at the desitnation
		transform.up = stats.destination - (Vector2)transform.position;
		//Range are distance between current bomb position and it desination
		range = Vector2.Distance(transform.position, stats.destination);
		//Reset the travelles distance
		travelled -= travelled;
		//Reset old position
		oldPos = transform.position;
	}

	void Update()
    {
		//Update the projectile velocity as the speed stats at transfrom up
		rb.velocity = transform.up * stats.speed;
		//If using drop effect
		if(drop)
		{
			//Get the percented progress of travelled half of the range
			float progress = travelled / (range/2);
			//Increase to 1 if still reaching half range then decrease to 0 when go over half range
			if(travelled < range/2) {goal = progress;} else {goal = 1 - (progress - 1);}
			//Example:
			// [0++++++1------0] = total range
			// + to 1   - to 0
			//Increase the size to make an fake height
			transform.localScale = new Vector2(goal + 0.2f, goal + 0.2f);
		}
		if(fade) //If using fade effect
		//Make the sprite less transparent the more it closer to target
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, (travelled/range)-0.2f);
    }

	Vector2 oldPos; void FixedUpdate()
	{
		//Get how many unit this frame has travel
		float frameTravel = Vector2.Distance(rb.position, oldPos);
		//Adding the time has travel per frame
		travelled += frameTravel;
		//Explode if has travel all the range
		if(travelled >= range) {Explode();}
		//Update the old position
		oldPos = rb.position;
	}

	void Explode()
	{
		//Create an circle of arrow
		creator.Create(arrow, 30, 360, transform.position, transform);
		//Destroy gameobject
		Destroy(gameObject);
	}
}
