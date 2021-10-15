using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public KeyCode parry, dash, bow, blade;
	public AudioSource sound;
	public static GameManager ins;
	public bool isPause;

	void Awake()
	{
		//Create singelton and don't destroy on load when neede
		if(GameManager.ins == null) {ins = this;DontDestroyOnLoad(this);}
		//If the game mananger are not in dontdestroyonload than destroy it
		if(gameObject.scene.name != "DontDestroyOnLoad") {Destroy(gameObject);}
	}

	public void SoundOn() {sound.volume = 1;}public void SoundOff() {sound.volume = 0;}

	public void Pause(){Time.timeScale = 0; isPause = true;}
	public void Continue() {Time.timeScale = 1; isPause = false;}
	public void Menu() 
	//Reset time
	{Time.timeScale = 1; isPause = false;
	//Reload scene
	SceneManager.LoadScene("Game", LoadSceneMode.Single);}
}
