using UnityEngine;

public class AbilityManager : MonoBehaviour
{
	Player player; public static AbilityManager i;
	[SerializeField] GameObject parryUI, dashUI, bowUI, bladeUI;

    void Awake()
	{
		//Make the ability singleton
		i = this;
	}

	public void UnlockParry()
	{
		//Get player
		player = Player.i;
		//If there are player and parry are not unlock
		if(player != null && !player.parry.isActiveAndEnabled)
		{
			//Enable parry UI
			parryUI.SetActive(true);
			//Able to use parry 
		 	player.parry.enabled = true;
		}
	}
	
	public void UnlockDash()
	{
		//If there are player and dash are not unlock
		if(player != null && !player.dash.isActiveAndEnabled)
		{
			//Enable dash UI
			dashUI.SetActive(true);
			//Able to use dash
			player.dash.enabled = true;
		}
	}

	public void UnlockBow()
	{
		//If there are player and bow are not unlock
		if(player != null && !player.bow.isActiveAndEnabled)
		{
			//Enable bow UI
			bowUI.SetActive(true);
			//Able to use bow
			player.bow.enabled = true;
		}
	}

	public void UnlockBlade()
	{
		//If there are player and blade are not unlock
		if(player != null && !player.blade.isActiveAndEnabled)
		{
			//Enable blade UI
			bladeUI.SetActive(true);
			//Able to use blade
			player.blade.enabled = true;
		}
	}
}