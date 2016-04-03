using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpaceShipUIController : MonoBehaviour {

	//This class controls the select spaceship UI

	public GameObject[] spaceShips;
	//public Text fastSpaceShip;
	//public Text bomberSpaceShip;

	public bool showDefault = true;
	public bool showFast = true;
	public bool showBomber = true;


	public Color disableTextColor;
	//---------------------------------------------------------------------------------
	// Show UI 
	void Start () 
	{
		int i = 0;
		foreach(GameObject t in spaceShips)
		{
			if(t.name == "Default")
			{
				if(!showDefault)
				{
					hideUI(i);
				}
			}
			if(t.name == "Fast")
			{
				if(!showFast)
				{
					hideUI(i);
				}
			}
			if(t.name == "Bomber")
			{
				if(!showBomber)
				{
					hideUI(i);
				}
			}
			i++;
		}
		LevelManager lvl = GameObject.FindObjectOfType<LevelManager>();
		lvl.setPlayer(spaceShips[0]);
	}

	//---------------------------------------------------------------------------------
	// Hide UI related to the Text position. Also show Image under the Text (if exist)
	void hideUI(int textPosition)
	{
		//Check that the position exists
		if(textPosition < spaceShips.Length)
		{
			//Get text
			GameObject currentGO = spaceShips[textPosition];
			if(currentGO)
			{

				Text currentText = currentGO.GetComponent<Text>();
				if(currentText)
				{
					currentText.color = disableTextColor;
					//Disable Button if exist

					Button btn = currentGO.GetComponent<Button>();
					if(btn)
					{
						btn.enabled = false;
					}
				}

				//Disable Image that will be a child of the gameObject
				Image img = currentGO.GetComponentInChildren<Image>();
				if(img)
				{
					img.color = disableTextColor;
				}
			}
		}
	}
	//---------------------------------------------------------------------------------
	// Call when the user select a spaceShip
	public void SelectSpaceship(string shipName)
	{
		LevelManager lvl = GameObject.FindObjectOfType<LevelManager>();
		if(lvl)
		{
			if(shipName ==  "Default")
			{
				lvl.setPlayer(spaceShips[0]);
			}
			else if(shipName == "Fast")
			{
				lvl.setPlayer(spaceShips[1]);
			}
			else if(shipName == "Bomber")
			{
				lvl.setPlayer(spaceShips[2]);
			}
			else
			{
				lvl.setPlayer(spaceShips[0]);
				Debug.LogError("SpaceShip name unknow!");
			}
		}
		else
		{
			Debug.LogError("No Level Manager");
		}
	}
}
