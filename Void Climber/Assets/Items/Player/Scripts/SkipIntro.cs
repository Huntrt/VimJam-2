using UnityEngine;

public class SkipIntro : MonoBehaviour
{
	[SerializeField] Animator animator;
	bool started;

	//Skip intro if has started
	void OnEnable() {if(started)animator.Play("Player Enter", 0, 1f);}

	void Start()
	{
		//Has start 
		started = true;
	}
}
