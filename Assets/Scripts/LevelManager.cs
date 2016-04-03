using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public string mainLevelName;
	public float spaceShipYposition = -4.0f;

	private Manager m_Player;
	private GameObject m_playerGO;
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{
		m_Player = GameObject.FindObjectOfType<Manager>();
	}	
	//-----------------------------------------------------------------------------------------------------
	//Load the level by it's name
	public void LoadLevel(string name)
	{
		if(m_Player)
		{
			m_Player.setLastLevel( Application.loadedLevel);
			if(m_playerGO && name == mainLevelName)
			{
				m_Player.setPlayer(m_playerGO);
			}
		}
		Debug.Log ("Load this level: " + name);


		Application.LoadLevel(name);

	}
	//-----------------------------------------------------------------------------------------------------
	//Request to quit application
	public void QuitRequest()
	{
		Debug.Log ("Quit Game Requested");
		Application.Quit ();
	}
	//-----------------------------------------------------------------------------------------------------
	//Load the next level defined on the build settings (list)
	public void LoadNextLevel()
	{
		if(m_Player)
		{
			m_Player.setLastLevel( Application.loadedLevel);
		}
		Application.LoadLevel(Application.loadedLevel + 1);
	}
	//-----------------------------------------------------------------------------------------------------
	//Reload the last level 
	public void LoadLastLevel()
	{
		if(m_Player)
		{
			int lastLevel = m_Player.getLastLevel();
			Application.LoadLevel(lastLevel);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method sets the player that will be instiantiate when the mainLevel it's loaded
	public void setPlayer(GameObject obj)
	{
		m_playerGO = obj;
	}
}
