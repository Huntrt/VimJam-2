using UnityEngine;
using TMPro;

public class Keybinds : MonoBehaviour 
{
	public TextMeshProUGUI parryText, dashText, bowText, bladeText;
	string assignKey;
	bool allowAssign;
	GameManager game;
	public static Keybinds i;

	void Start()
	{
		//Mark into singleton
		i = this;
		//Get game manager
		game = GameManager.ins;
		//Reset text upon create
		ResetText();
	}

	public string UpdateTutor(string ability)
	{
		//Get the key of ability gonna tutor inside game manager
		return game.GetType().GetField(ability).GetValue(game).ToString();
	}
	
	void Update()
	{
		//Getting key
		GetKey();
	}

	public void StartAssign(string targetKey)
	{
		//Get the key gonna assign
		assignKey = targetKey;
		//Allow to assign key
		allowAssign = true;
	}

	void GetKey()
    {
        //If allow to assign
        if(allowAssign)
        {
			//Save the Text variable with key assign
			string assignText = assignKey + "Text";
			//Get the text gonna display by using text string has get
			TextMeshProUGUI display = (TextMeshProUGUI)this.GetType().GetField(assignText).GetValue(this);
			//Display change to waititng
			display.text = "waiting...";
            //Go though all the key to check if it there is any input
            foreach(KeyCode pressedKey in System.Enum.GetValues(typeof(KeyCode)))
            //if there is a input
            if(Input.GetKey(pressedKey))
            {
                //And it not esc key
                if(pressedKey != KeyCode.Escape)
                {
					//Get the keycode variable with the name of assign key then set it keycode to key press
					game.GetType().GetField(assignKey).SetValue(game, pressedKey);
					//Display change to pressed key
					display.text = pressedKey.ToString();
					//No longer allow to assign
					allowAssign = false;
                }
            }
        }
    }

	void ResetText()
	{
		//Reset parry key to be the key inputed
		parryText.text = game.parry.ToString();
		//Reset dash key to be the key inputed
		dashText.text = game.dash.ToString();
		//Reset bow key to be the key inputed
		bowText.text = game.bow.ToString();
		//Reset blade key to be the key inputed
		bladeText.text = game.blade.ToString();;
	}
}