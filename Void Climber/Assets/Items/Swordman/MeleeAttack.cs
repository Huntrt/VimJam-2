using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
	[SerializeField] float duration;

	void OnEnable()
	{
		//Over hitbox after set duration
		Invoke("AttackOver", duration);
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
		//Hurt player cancel invoke and deactive object if hit player
		if(other.CompareTag("Player")) {Player.i.Hurt(); CancelInvoke(); gameObject.SetActive(false);}
	}

	//No longer melee attack and deactive object
	void AttackOver() {gameObject.SetActive(false);}
}
