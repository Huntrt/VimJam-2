using UnityEngine;

public class Bow : MonoBehaviour
{
	[SerializeField] LineRenderer beam;
	public float attackRate;
	[SerializeField] bool isCharge = false, hasAttack = false;
	[SerializeField] Animator animator;
	public float lengthIncrease, length, delay;
	public float width, slow; Vector2 direction;
	[SerializeField] SpriteRenderer sprite;
	[SerializeField] Color chargeColor, releaseColor;
	[SerializeField] AudioClip chargeAudio, relaseAudio;
	float defaulSpeed; Player player;

	void Start()
	{
		//Get the player
		player = Player.i;
	}

    void Update()
    {
		//If no longer pressed bow key while is charge, release the it and start the release animation
		if(Input.GetKeyUp(GameManager.ins.bow) && isCharge) {Release(); animator.SetTrigger("Release");}
		//If press bow key when not attack recently
		if(Input.GetKey(GameManager.ins.bow) && !hasAttack)
		{
			//Start charging animation once and save the default speed
			if(!isCharge)
			{animator.SetTrigger("Charge"); defaulSpeed = player.speed;GameManager.ins.sound.PlayOneShot(chargeAudio);}
			//Begin charging and enable beam
			isCharge = true; beam.enabled = true;
		}
		//Show nowsprite when only needed
		sprite.enabled = beam.enabled;
    }

	void LateUpdate()
	{
		//Get the direction toward mouse
		direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition)-transform.position).normalized;
		//Make the transfrom look toward direction
		transform.up = direction;
		//If is charging
		if(isCharge)
		{
			//Begin charging
			Charging();
			//Slowing down player
			player.speed = slow;
		}
	}

	void Charging()
	{
		//Increase length over time
		length += lengthIncrease;
		//Set the beam color as charge
		Color useColor = new Color(chargeColor.r, chargeColor.g, chargeColor.b, chargeColor.a);
		//Update the line start and end color
		beam.startColor = useColor; beam.endColor = useColor;
		//Set the start and end width as size
		beam.startWidth = width; beam.endWidth = width;
		//Set the line start position at current position
		beam.SetPosition(0, transform.position);
		//Set an end position as the transfrom with up vector that modified with length
		///(make transfrom up look at transfrom.up)
		beam.SetPosition(1, transform.TransformPoint(Vector2.up * length));
	}

	void Release()
	{
		GameManager.ins.sound.PlayOneShot(relaseAudio);
		//No longer charge
		isCharge = false;
		//Reset player speed
		player.speed = defaulSpeed;
		//Set the beam color as release
		Color useColor = new Color(releaseColor.r, releaseColor.g, releaseColor.b, releaseColor.a);
		//Update the line start and end color
		beam.startColor = useColor; beam.endColor = useColor;
		//Get the beam length by using 2 of it position instead raw length cause it longer for raycast
		float beamLength = Vector2.Distance(beam.GetPosition(0), beam.GetPosition(1));
		//Cast circle in this position with radius of size/2 toward direction with length of beam
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position,width/2,direction,beamLength);
		//has hit boss?
		bool hasHit = false;
		//Go throught all the object got hit
		foreach (RaycastHit2D hit in hits)
		//If hitting boss the first time then hurt it and no longer able to hit it
		{if(hit.collider.CompareTag("Boss") && !hasHit){BossManager.i.heath.Hurt(1); hasHit = true;}}
		//Clear the beam after delay and has attack
		Invoke("Clear", delay); hasAttack = true;
	}

	void Clear()
	{
		//Reset the beam end point
		beam.SetPosition(1, transform.position);
		//Reset length and has attack
		length -= length; Invoke("ResetAttack", attackRate);
		//Deactive beam
		beam.enabled = false;
	}

	void ResetAttack() {hasAttack = false;}
}
