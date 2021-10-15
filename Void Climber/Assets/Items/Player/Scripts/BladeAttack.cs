using UnityEngine;

public class BladeAttack : MonoBehaviour
{
	bool hasHit;

	//Able to hit when enable
	void OnEnable() {hasHit = false;}

	private void OnTriggerEnter2D(Collider2D other) 
	//Hurting the boss if able to then no longer able to it
	{if(other.CompareTag("Boss") && !hasHit) {BossManager.i.heath.Hurt(1);hasHit = true;}}

	//Deactive upon called
    public void Deactive() {gameObject.SetActive(false);}

}
