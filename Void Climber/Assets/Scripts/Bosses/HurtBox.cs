using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) 
	{
		if(other.collider.CompareTag("Player")) {Player.i.Hurt();}
	}
}