using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour 
{
	public PlayerController player;
	public Slider healthBarFillAmount;

	void Start()
	{
		player = PlayerController.player;
		healthBarFillAmount = GetComponent<Slider>();
	}

	void Update()
	{
		if(player)
			healthBarFillAmount.value = Mathf.Lerp(healthBarFillAmount.value, (float) player.getCurrentHealth() / (float) player.getMaxHealth(), 0.2f);
	}
}