using UnityEngine;

public class Laser : MonoBehaviour
{
	public LaserStats stats = new LaserStats();
	[SerializeField] [Tooltip("At which % progress change from wait to flash (use 0.X)")] float peak;
	[SerializeField] [Tooltip("How many % has windup")]float progress; float winding;
	[SerializeField] Color waitColor, flashColor;
	[SerializeField] float flashSize;
	[SerializeField] LineRenderer line;
	[SerializeField] AudioClip chargeAudio, completeAudio;
	Vector2 direction;

    void OnEnable()
	{
		//Reset the wind up counter
		winding -= winding;
		//Reset progress
		progress -= progress;
		//Begin lasering
		Lasering();
		GameManager.ins.sound.PlayOneShot(chargeAudio);
	}

    void LateUpdate()
    {
		//Increase the wind up time
		winding += Time.deltaTime;
		//Get the wind up progress
		progress = winding / stats.windUp;	
		//Begin lasering
		Lasering();
    }

	void Lasering()
	{
		//Get ther direction to the target
		direction = (stats.target - (Vector2)transform.position).normalized;
		//Change btween the wait and flash color base on progress
		Color color = new Color(); if(progress < peak) {color = waitColor;} else {color = flashColor;}
		//Update the line start and end color
		line.startColor = color; line.endColor = color;
		//Set the start and end width as size
		line.startWidth = stats.size; line.endWidth = stats.size;
		//Make the laser look at it target
        transform.up = direction;
		//Set the line start position at current 
		line.SetPosition(0, transform.position); 
		//Set end point at direction up with range
		line.SetPosition(1, transform.TransformPoint(Vector2.up * stats.range));
		//Complete laser when complete wind up
		if(winding >= stats.windUp) {CompleteLaser();}
		//Get the length between 2 point of line
		float length = Vector2.Distance(line.GetPosition(0), line.GetPosition(1));
		//Create an circle cast at this position with radius of size/2
		RaycastHit2D hit = Physics2D.CircleCast(transform.position, stats.size/2
		//With set direction and length only on player' layer then danger the player if prepare to hit
		,direction , length, 1<<Player.i.layer); if(hit) {Player.i.panic.inDanger = true;}
	}

	void CompleteLaser()
	{
		GameManager.ins.sound.PlayOneShot(completeAudio);
		//Get the length between 2 point of line
		float length = Vector2.Distance(line.GetPosition(0), line.GetPosition(1));
		//Create an circle cast at this position with radius of size/2
		RaycastHit2D hit = Physics2D.CircleCast(transform.position, stats.size/2
		//With set direction and length only on player' layer then hurt player if hit it
		,direction , length, 1<<Player.i.layer); if(hit) {Player.i.Hurt();}
		//Set the start and end width as size that will increase with flash size
		line.startWidth = stats.size + flashSize; line.endWidth = stats.size + flashSize;
		//Deacitve the when complete laser
		gameObject.SetActive(false);
	}
}
