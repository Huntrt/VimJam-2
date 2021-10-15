using UnityEngine;

public class Blade : MonoBehaviour
{
	[SerializeField] GameObject swing;
	[SerializeField] float attackRate, distance; bool hasAttack = false;
	[SerializeField] AudioClip bladeAudio;

    void Update()
    {
		//If pressing key blade while not attack
        if(Input.GetKey(GameManager.ins.blade) && !hasAttack)
		{
			GameManager.ins.sound.PlayOneShot(bladeAudio);
			//Get the mouse position
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//Get the direction toward mouse
			transform.up = (mousePos - (Vector2)transform.position).normalized;
			//Get the point to create the swing my move it slightly upward from this
			Vector2 point = transform.position + transform.up * distance;
			//Create the swing at point with this rotation then set it parent as this
			Pool.get.Object(swing, point,transform.rotation, true).transform.parent = transform;
			//Resdet attack and has attack
			Invoke("ResetAttack", attackRate); hasAttack = true;
		}
    }
	//Reseting has attacking
	void ResetAttack() {hasAttack = false;}
}
