using UnityEngine;
using UnityEngine.Events;

public class BossHeath : MonoBehaviour
{
	public int maxHeath, heath;
	public Color heathbarColor;
	public SpriteRenderer[] parts;
	[SerializeField] float flickDuration, flickerRate;
	public Color[] defaultColor; [SerializeField] Color flickColor;
	[SerializeField] AudioClip hurtAudio, dieAudio;
	public UnityEvent hurt;

	void Start()
	{
		//List of parts color
		defaultColor = new Color[parts.Length];
		//Get the default color of each part
		for (int c = 0; c < parts.Length; c++){defaultColor[c] = parts[c].color;}
		//Update heath to max heath
		heath = maxHeath;
	}

    void Update()
    {
		//Destroy game object if it has 0 heart
        if(heath <= 0) {; Destroy(gameObject);GameManager.ins.sound.PlayOneShot(dieAudio);}
    }

	public void Hurt(int value) 
	{
		//Decrease heath
		heath -= value;
		//Start flicering
		StartFlickering(flickDuration);
		//Call even
		hurt.Invoke();
		GameManager.ins.sound.PlayOneShot(hurtAudio);
	}

	public void StartFlickering(float dur) 
	{
		//Cancel invoke to prevent overlap
		CancelInvoke("ClearFlicker"); CancelInvoke("Flickering");
		//Begin flicking sprite
		InvokeRepeating("Flickering", 0, flickerRate);
		//Clear flicker after set time
		Invoke("ClearFlicker", dur);
	}

	void Flickering()
	{
		//Go throught all the part's sprite
		for (int f = 0; f < parts.Length; f++)
		{
			//If part are not flickering then flicker it and reset to default if part are final
			if(parts[f].color != flickColor) {parts[f].color = flickColor; if(f == parts.Length-1) {return;}}
		}
		//Go throught all the part's sprite
		for (int d = 0; d < parts.Length; d++)
		{
			//If part are flickering then reset it to defaut color
			if(parts[d].color != defaultColor[d]) {parts[d].color = defaultColor[d];}
		}
	}

	void ClearFlicker() 
	{
		//Stop flickering
		CancelInvoke("Flickering");
		//Reset all of the part color to default color
		for (int c = 0; c < parts.Length; c++) {parts[c].color = defaultColor[c];}
	}
}
