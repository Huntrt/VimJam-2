using UnityEngine;
using System.Collections.Generic;

public class CreateProjectile : MonoBehaviour
{
	[SerializeField] AudioClip createAudio;
	///Call to create projectile with set data
    public void Create(GameObject projectile, int spread, float focus, Vector2 point, Transform emitter)
	{
		GameManager.ins.sound.PlayOneShot(createAudio);
		//Create and send projectile by bool at point position and emitter rotation if there only one
		if(spread == 1) {Pool.get.Object(projectile, point, emitter.rotation, true);}
		//If there is multiple projectile to spread
		else
		{
			//Range are the focus got divide by 2 since it affect 2 direction
			float range = focus / 2;
			//Get the 180 to -180 rotation of the emitter
			float rot = emitter.localEulerAngles.z; float center = (rot > 180) ? rot-360 : rot;
			//Get the start and end rotation by decrease and increase the center with range
			float start = center - range; float end = center + range;
			//Get the distance between each spread using the total focus (-1 there an note about it)
			float step = focus / (spread-1);
			//Begin the first angle at start
			float angle = start;
			//For each of the projectile need to create
			for (int i = 0; i < spread; i++)
			{
				//Create an send projectile by pool at point position with the rotation of spread angle
				Pool.get.Object(projectile, point, Quaternion.Euler(0,0,angle), true);
				//Proceed to the next step
				angle += step;
			}
		}
	}

	///Call to create projectile with set data then get it
    public void CreateGet(GameObject projectile, int spread, float focus,
	Vector2 point, Transform emitter, out List<GameObject> created)
	{
		//Create empty list of object has cretae
		created = new List<GameObject>();
		//Create and send projectile by bool at point position and emitter rotation if there only one
		if(spread == 1) {created.Add(Pool.get.Object(projectile, point, emitter.rotation));}
		//If there is multiple projectile to spread
		else
		{
			//Range are the focus got divide by 2 since it affect 2 direction
			float range = focus / 2;
			//Get the 180 to -180 rotation of the emitter
			float rot = emitter.localEulerAngles.z; float center = (rot > 180) ? rot-360 : rot;
			//Get the start and end rotation by decrease and increase the center with range
			float start = center - range; float end = center + range;
			//Get the distance between each spread using the total focus (-1 there an note about it)
			float step = focus / (spread-1);
			//Begin the first angle at start
			float angle = start;
			//For each of the projectile need to create
			for (int i = 0; i < spread; i++)
			{
				//Create an send projectile by pool at point position with the rotation of spread angle
				created.Add(Pool.get.Object(projectile, point, Quaternion.Euler(0,0,angle)));
				//Rename the created projectile to prevent pool reusing since it not auto active
				created[i].name += " Temp";
				//Proceed to the next step
				angle += step;
			}
		}
	}
}
