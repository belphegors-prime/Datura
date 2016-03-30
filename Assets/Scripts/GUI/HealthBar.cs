using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
	public static HealthBar healthBar;
	public static Image healthBarFillAmount;
	public static Image healthBarBackground;
	public static Text enemyNameText;
	public static PlayerController player;

	// Use this for initialization
	void Start() 
	{
		player = PlayerController.player;
		healthBarFillAmount = GetComponent<Image>();
		healthBarBackground = transform.parent.Find("HealthBarBackground").GetComponent<Image>();
		enemyNameText = transform.parent.Find("EnemyName").GetComponent<Text>();
		disableHealthBar();
	}

	void Update()
	{
		if(player && player.target)
			healthBarFillAmount.fillAmount = Mathf.Lerp(healthBarFillAmount.fillAmount, (float) player.target.getCurrentHealth() / (float) player.target.getMaxHealth(), 0.2f);

		else disableHealthBar();
	}

	public static void enableHealthBar()
	{
		if(player)
		{
			healthBarFillAmount.fillAmount = (float) player.target.getCurrentHealth() / player.target.getMaxHealth();
			healthBarFillAmount.enabled = true;
			healthBarBackground.enabled = true;
			enemyNameText.enabled = true;
			enemyNameText.text = (player.target.getName()).ToUpper();
		}
	}

	public static void disableHealthBar()
	{
		healthBarFillAmount.enabled = false;
		healthBarBackground.enabled = false;
		enemyNameText.enabled = false;
	}
}