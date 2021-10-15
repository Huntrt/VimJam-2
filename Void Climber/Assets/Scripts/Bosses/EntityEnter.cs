using UnityEngine;

public class EntityEnter : MonoBehaviour
{
    public GameObject entity;
	public Camera main, cutscene;

	void Start()
	{
		//Reset the camera tag
		cutscene.tag = "MainCamera"; main.tag = "Untagged";
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
		//Reset the camera tag
		cutscene.tag = "Untagged"; main.tag = "MainCamera";
		//Disable cut scene camera
		main.enabled = true; cutscene.enabled = false; 
		//Create the enity then destroy this object
		Instantiate(entity, transform.position, Quaternion.identity); gameObject.SetActive(false);
	}
}
