using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartMenu : MonoBehaviour
{
	public Canvas exitMenu;
	public Canvas characterSelectionMenu;
	public Canvas worldGenerationMenu1;
	public Canvas worldGenerationMenu2;
	public Button newGameButton;
	public Button loadGameButton;
	public Button exitButton;

	// Use this for initialization
	void Start() 
	{
		exitMenu.enabled = false;
		exitMenu.GetComponent<Canvas>();
		characterSelectionMenu.enabled = false;
		characterSelectionMenu.GetComponent<Canvas>();
		worldGenerationMenu1.enabled = false;
		worldGenerationMenu1.GetComponent<Canvas>();
		worldGenerationMenu2.enabled = false;
		worldGenerationMenu2.GetComponent<Canvas>();

		newGameButton = newGameButton.GetComponent<Button>();
		exitButton = exitButton.GetComponent<Button>();
	}
		
	public void NewGamePress()
	{
		characterSelectionMenu.enabled = true;
		ToggleMainMenuButtons(false);
	}

	public void ExitPress()
	{
		exitMenu.enabled = true;
		ToggleMainMenuButtons(false);
	}

	public void CancelExitPress()
	{
		exitMenu.enabled = false;
		ToggleMainMenuButtons(true);
	}
		
	public void OkExitPress()
	{
		Application.Quit();
	}

	public void MaleGenderPress()
	{
		characterSelectionMenu.enabled = false;
		worldGenerationMenu1.enabled = true;
	}

	public void LargeWorldPress()
	{
		worldGenerationMenu1.enabled = false;
		worldGenerationMenu2.enabled = true;
	}

	public void SmallWorldPress()
	{
		worldGenerationMenu1.enabled = false;
		worldGenerationMenu2.enabled = true;
	}

	public void FemaleGenderPress()
	{
		characterSelectionMenu.enabled = false;
		worldGenerationMenu1.enabled = true;
	}

	public void ReturnCharacterSelectionPress()
	{
		characterSelectionMenu.enabled = false;
		ToggleMainMenuButtons(true);
	}

	public void ReturnWorldGenerationOnePress()
	{
		worldGenerationMenu1.enabled = false;
		characterSelectionMenu.enabled = true;
	}

	public void ReturnWorldGenerationTwoPress()
	{
		worldGenerationMenu2.enabled = false;
		worldGenerationMenu1.enabled = true;
	}

	public void ThreeCitiesPress()
	{
		Application.LoadLevel(1);
	}

	public void SixCitiesPress()
	{
		Application.LoadLevel(1);
	}

	public void NineCitiesPress()
	{
		Application.LoadLevel(1);
	}
		
	// Enable or disable main menu buttons
	private void ToggleMainMenuButtons(bool buttonActivationBoolean)
	{
		newGameButton.enabled = buttonActivationBoolean;
		loadGameButton.enabled = buttonActivationBoolean;
		exitButton.enabled = buttonActivationBoolean;
	}
}