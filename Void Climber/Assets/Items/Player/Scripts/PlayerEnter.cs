using UnityEngine;

public class PlayerEnter : MonoBehaviour
{
    public GameObject player;
	public Camera main, cutscene;

	void OnEnable()
	{
		//Disable main camera to enable cutscene
		cutscene.enabled = true; main.enabled = false;
	}

	void LateUpdate()
	{
		//Cutscene camera following enter
		cutscene.transform.position = new Vector3(transform.position.x, transform.position.y,-10);
	}

	public void Entered() 
	{
		//Disable cut scene camera
		main.enabled = true; cutscene.enabled = false; 
		//Set player transfrom position and active the player
		player.transform.position = transform.position; player.SetActive(true);
		//Deactive the enter
		gameObject.SetActive(false);
	}
}
