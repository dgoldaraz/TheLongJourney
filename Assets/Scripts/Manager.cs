using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

	public static bool autoPlay = false;
	public float scoreIncrementPerSecond = 10.0f;
	public float scoreHighVelocityMultiplier = 10.0f; //This is a multiplier for the score when it's on High velocity


	private static Manager m_mpInstance = null;
	private static int m_lastLevel = 1;
	private static int m_inputEntry = 0;
	private static float m_points = 0;
	private static GameObject m_playerGO;
	private static bool m_createPlayer = false;
    private float m_scoreMultiplier = 1.0f;
	private bool m_isHIghVelocity = false;
	private bool m_gameStarted = false; //This variable sets if the game has started its first level
	private bool m_initNotifies = false;

	//-----------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Awake()
	{
		//Singleton Pattern, if the sysem creates an object that has been created (it's static), destroy the current instance
		if( m_mpInstance != null)
		{
			Destroy (gameObject);
		}
		else
		{
			m_mpInstance = this;
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{
		//Connect update point to the StartField High/slow methods
		StarFieldManager.onIncreasingSpeed += HighVelocityScore;
		StarFieldManager.onDecreaseSpeed += LowVelocityScore;
	}
	//-----------------------------------------------------------------------------------------------------
	void onDestroy()
	{
		StarFieldManager.onIncreasingSpeed -= HighVelocityScore;
		StarFieldManager.onDecreaseSpeed -= LowVelocityScore;
	}
	//-----------------------------------------------------------------------------------------------------
	void Update()
	{
		//If R it's pressed, reload the level
		if (Input.GetKeyDown (KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevel);
		}
		else if(Input.GetKeyDown(KeyCode.T))
		{
			//if T it's pressed, the game will autoplay
			autoPlay = !autoPlay;
		}
		createPlayerIfNecessary();
		updateScore();
		if(!m_initNotifies && GameObject.FindObjectOfType<PlayerController>()!= null)
		{
			PlayerController.onNotifyGUI += UpdateGUI;
			PlayerController.onRegenerate += UpdateRegenerateGUI;
			m_initNotifies = true;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method creates the player if it's necessary
    void createPlayerIfNecessary()
	{
		//Create the initial player
		if(m_createPlayer && m_playerGO)
		{
			//reset the score if there is a new player
			resetScore();
			if(GameObject.FindObjectOfType<PlayerController>()!= null)
			{
				m_createPlayer = false;
				return;
			}
			Vector3 position = new Vector3(0.0f, -4.0f, 0.0f);
			GameObject player = Instantiate(m_playerGO, position, Quaternion.identity) as GameObject;
			if(player.GetComponent<PlayerController>())
			{
				m_scoreMultiplier = player.GetComponent<PlayerController>().getScoreMultiplier();
			}
			m_gameStarted = true;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//Updates the score based on the velocity of the ship
	void updateScore()
	{
		if(m_gameStarted)
		{
			float pointsToAdd = scoreIncrementPerSecond * m_scoreMultiplier;
			if(m_isHIghVelocity)
			{
				pointsToAdd *= scoreHighVelocityMultiplier;
			}
			addPointsToScore(pointsToAdd * Time.deltaTime);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	public int getLastLevel()
	{
		return m_lastLevel;
	}
	//-----------------------------------------------------------------------------------------------------
	//Saves last level
	public void setLastLevel(int level)
	{
		m_lastLevel = level;
	}
	//-----------------------------------------------------------------------------------------------------
	public int getInputEntry()
	{
		return m_inputEntry;
	}
	//-----------------------------------------------------------------------------------------------------
	public void setInputEntry(int input)
	{
		m_inputEntry = input;
	}
	//-----------------------------------------------------------------------------------------------------
	//Handles the score
	public void addPointsToScore(float newPoints)
	{
		m_points += newPoints;

		Text[] txt = GameObject.FindObjectsOfType<Text>() as Text[];
		foreach( Text t in txt)
		{
			if(t.gameObject.name == "ScoreText")
			{
				t.text = getScore().ToString() + " Km";
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//Resets the Score
	public void resetScore()
	{
		m_points = 0;
	}
	//-----------------------------------------------------------------------------------------------------
	public int getScore()
	{
		return (int)m_points;
	}
	//-----------------------------------------------------------------------------------------------------
	// This method sets the main player (that will be created after the first awake
	public void setPlayer(GameObject player)
	{
		m_playerGO = player;
		m_createPlayer = true;
	}

	//-----------------------------------------------------------------------------------------------------
	//This method is called when the velocity is high. The score will increment faster than normal
	void HighVelocityScore()
	{
		m_isHIghVelocity = true;
	}
	//-----------------------------------------------------------------------------------------------------
	//This method is called when the velocity is high. This is the normal velocity
	void LowVelocityScore()
	{
		m_isHIghVelocity = false;

		//Remove GUI Text when speed lower down
		Text[] txt = GameObject.FindObjectsOfType<Text>() as Text[];
		foreach( Text t in txt)
		{
			if(t.gameObject.name == "GUIText" || t.gameObject.name == "RegenerateGUI")
			{
				t.text = "";
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//Update the GUI text
	public void UpdateGUI(string text)
	{
		Text[] txt = GameObject.FindObjectsOfType<Text>() as Text[];
		foreach( Text t in txt)
		{
			if(t.gameObject.name == "GUIText")
			{
				t.text = text;
			}
		}
		Invoke ("ClearGUI", 2.0f);
	}
	//-----------------------------------------------------------------------------------------------------
	//Clear the GUI text
	public void ClearGUI()
	{
		Text[] txt = GameObject.FindObjectsOfType<Text>() as Text[];
		foreach( Text t in txt)
		{
			if(t.gameObject.name == "GUIText")
			{
				t.text = "";
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//Update the Regenerating text
	public void UpdateRegenerateGUI(string text)
	{
		Text[] txt = GameObject.FindObjectsOfType<Text>() as Text[];
		foreach( Text t in txt)
		{
			if(t.gameObject.name == "RegenerateGUI")
			{
				t.text = text;
			}
		}
		Invoke ("ClearRegenGUI", 2.0f);
	}
	//-----------------------------------------------------------------------------------------------------
	//Clear the Regenerating text
	public void ClearRegenGUI()
	{
		Text[] txt = GameObject.FindObjectsOfType<Text>() as Text[];
		foreach( Text t in txt)
		{
			if(t.gameObject.name == "RegenerateGUI")
			{
				t.text = "";
			}
		}
	}
}
