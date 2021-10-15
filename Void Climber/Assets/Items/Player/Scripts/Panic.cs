using UnityEngine;

public class Panic : MonoBehaviour
{
	public float slow, radius;
	[SerializeField] GameObject indicator;
	[SerializeField] AudioClip panicSound;
		float prevTime;
	public bool inDanger;

    void Update()
    {
		//Cast an circle at current position with radius with no direction
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero);
		//For each of object got hit
		foreach (RaycastHit2D hit in hits)
		//If hit are boss attack then in danger
		{if(hit.collider.CompareTag("Boss Attack")){inDanger = true;}}
		//If if not pausing
		if(!GameManager.ins.isPause)
		{
			//If in danger and time scale are not slown down then slow down time and save previous time
			if(inDanger && Time.timeScale != slow) {Time.timeScale = slow;GameManager.ins.sound.PlayOneShot(panicSound);}
			//Stop slown down time if not in danger
			if(!inDanger) {Time.timeScale = 1;}
		}
		//Display indicator base on when in danger
		indicator.SetActive(inDanger);
		//Not in danger yet
		inDanger = false;
    }

	//In danger
	public void Danger() {inDanger = true;} 
}
