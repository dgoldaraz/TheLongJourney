using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	//This class handles the creation of Squadrons depending on level and control when all the squadron has died
	public GameObject[] SquadronTypes; 
	public float maxTimeOnHighSpeed = 10.0f;
	public float minTimeOnHighSpeed = 5.0f;
	public int level = 0;
	public float obstaclesPerSecond = 0.7f;
	public GameObject[] obstacles;
	public int addToObstacleNumber = 3; //This number is added to the current level to choose a max number of obstacles in high velocity
	public float width = 10.0f;
	public float height = 5.0f;
	public int maxSquadrons = 3;


	private float m_offsetTime = 0.0f;
	private float m_timeHighSpeed = 0.0f;
	private bool m_waiting = false;
	private float m_xmax,m_xmin;

	public GameObject m_currentSquadron;
	private StarFieldManager m_starfield;
	public int m_squadrons = 0;
	private int m_maxNumberOfObstacles = 0;
	//--------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		//Get StarField
		m_starfield = GameObject.FindObjectOfType<StarFieldManager>();
		float distance = transform.position.z - Camera.main.transform.position.z; 
		float halfWidth = width*0.5f;
		Vector3 leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0,0,distance));
		Vector3 rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1,0,distance));
		m_xmax = rightMost.x - halfWidth;
		m_xmin = leftMost.x + halfWidth;

		//Start in High velocity
		m_maxNumberOfObstacles = Random.Range (level, level + addToObstacleNumber);
		m_starfield.HighVelocity();
		m_timeHighSpeed = Random.Range(minTimeOnHighSpeed,maxTimeOnHighSpeed);
	}
	//--------------------------------------------------------------
	// Update is called once per frame
	void Update () 
	{
		if(m_timeHighSpeed > 0.0f)
		{
			m_offsetTime += Time.deltaTime;
			if(m_offsetTime > m_timeHighSpeed)
			{
				//Low velocity again and reset time values
				m_starfield.LowVelocity();
				m_timeHighSpeed = 0.0f;
				m_offsetTime = 0.0f;
				//Waits an amount of time until the creation of a new squadron
				m_waiting = true;
			}
			else
			{
				//Check if the we can create some obstacles
				if(m_maxNumberOfObstacles >= 0)
				{
					//Create a random Obstacle!
					float probability = Time.deltaTime * obstaclesPerSecond;
					if(Random.value < probability)
					{
						Vector3 position = new Vector3( Random.Range (m_xmin, m_xmax), m_starfield.transform.position.y, 0.0f);
						GameObject obs = Instantiate(obstacles[Random.Range(0, obstacles.Length)], position, Quaternion.identity) as GameObject;
						Destroy(obs, 5.0f);
						m_maxNumberOfObstacles--;
					}
				}
			}
		}

		if(m_waiting)
		{
			//Check if the starField has stop changing
			if(m_starfield.isChanging())
			{
				m_offsetTime += Time.deltaTime;
				m_waiting = false;
				level++;
				//increase squadron each 2 levels
				if(level > 0 && (level % 2) == 0)
				{
					maxSquadrons++;
				}
				//calculate how many squadrosn we have until high speed
				m_squadrons = Random.Range(1, maxSquadrons);
				createSquadron();
			}
		}
	}
	//--------------------------------------------------------------
	public void createSquadron()
	{
		//Create squadron and set notify method
		int randonSquadron = Random.Range (0, SquadronTypes.Length);
		m_currentSquadron = Instantiate(SquadronTypes[randonSquadron], this.transform.position, Quaternion.identity) as GameObject;
		m_currentSquadron.GetComponent<EnemySquadron>().initSquadron(width, height, m_xmax, m_xmin, level);
		m_currentSquadron.transform.SetParent(this.transform);
		EnemySquadron.onEmptySquadron += SquadronDead;
		m_currentSquadron.GetComponent<EnemySquadron>().spawUntilFull();
	}
	//--------------------------------------------------------------
	void SquadronDead()
	{
		//Reduce squadrons and destroy current one. If we don't need more squadron creation, jump to high speed
		m_squadrons--;
		EnemySquadron.onEmptySquadron -= SquadronDead;
		Destroy (m_currentSquadron);
		if(m_squadrons <= 0)
		{
			m_maxNumberOfObstacles = Random.Range (level, level + addToObstacleNumber);
			m_starfield.HighVelocity();
			m_timeHighSpeed = Random.Range(minTimeOnHighSpeed,maxTimeOnHighSpeed);
		}
		else
		{
			createSquadron();
		}
	}
}
