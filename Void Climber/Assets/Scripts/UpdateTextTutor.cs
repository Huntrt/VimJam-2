using UnityEngine;

public class UpdateTextTutor : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI tutor;

	void Awake()
	{
		//Get the tutor text
		tutor = GetComponent<TMPro.TextMeshProUGUI>();
	}

	void OnEnable()
	{
		//Get the ability name
		string ability = tutor.name.Replace(" turtorial", "");
		//Display the key dynamicly and it name base on current tutor 
		tutor.text = "Press " + Keybinds.i.UpdateTutor(ability) + " to use " + ability;
	}
}
