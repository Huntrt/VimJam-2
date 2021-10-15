using UnityEngine;

public class Spotlight : MonoBehaviour
{
	public LaserStats laser = new LaserStats();
	[SerializeField] bool selfDefense = false;
	[SerializeField] Transform target;
	[SerializeField] Laser beam;
	[SerializeField] float beamRate;

	void Start()
	{
		//Player are target
		target = Player.i.transform;
		//Get first child as beam
		beam = transform.GetChild(0).GetComponent<Laser>();
		//Invoke beam  if not self defense
		if(!selfDefense)InvokeRepeating("Beaming", beamRate, beamRate);
		//Start beam when got hurt if needed
		if(selfDefense)BossManager.i.heath.hurt.AddListener(Beaming);
	}

	void Beaming()
	{
		//Update the beam stats and target
		beam.stats = laser; beam.stats.target = target.position;
		//Acitve beam
		beam.gameObject.SetActive(true);
	}
}
