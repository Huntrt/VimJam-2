using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossManager : MonoBehaviour
{
	public static BossManager i; void Awake() {i = this;}
	public Transform boss;
	public BossHeath heath;
	[SerializeField] GameObject heathUI;
	[SerializeField] Image bar;
	public TextMeshProUGUI heathTitle;
	public int stage;
	[SerializeField] GameObject[] bossEnter;
	[SerializeField] GameObject playerEnter;
	[SerializeField] GameObject victoryScreen;
	[SerializeField] TextMeshProUGUI victoryInfo;
	[SerializeField] AudioClip playerDie;

	void Update()
	{
		//If there is heath
		if(heath != null)
		{
			//Active heath Ui
			heathUI.SetActive(true);
			//Change the bar color
			bar.color = heath.heathbarColor;
			//Update bar heath value
			bar.fillAmount = (float)heath.heath / (float)heath.maxHeath;
			//Update the boss text
			heathTitle.text = boss.name.Replace("(Clone)", "");
			//Update the heath title color
			heathTitle.color = heath.heathbarColor;
		}
		//Deactive heath Ui if there is heath
		else {heathUI.SetActive(false);}
	}

	public void Respawn()
	{
		//Destroy current boss
		Destroy(GameObject.FindWithTag("Boss"));
		//Active current boss holder of current stage
		bossEnter[stage].SetActive(true);
		//Active the player holder
		playerEnter.SetActive(true);
		GameManager.ins.sound.PlayOneShot(playerDie);
	}

	public void Victory()
	{
		//Start victory screen
		victoryScreen.gameObject.SetActive(true);
		//Get the time has dies
		int die = Player.i.dies;
		//Display if die 0 time
		if(die == 0) victoryInfo.text = "You win with 0 death, congrats!";
		//Display if die 1 time
		if(die == 1) victoryInfo.text = "You win with 1 death";
		//Display if di more than 1 time
		if(die > 1) victoryInfo.text = "You win with " + die + " deaths";
	}
}
