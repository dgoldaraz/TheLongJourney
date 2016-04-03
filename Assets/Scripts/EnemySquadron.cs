using UnityEngine;
using System.Collections;

public class EnemySquadron : MonoBehaviour {

	public delegate void NotifyEmpty();
	public static event NotifyEmpty onEmptySquadron;

	public GameObject[] enemyPrefab;

	public float speed = 10.0f;
	public float maxSpeed = 15.0f;
	public float minSpeed = 2.0f;
	public bool randomSpeed = true;
	public float padding = 3.0f;
	public float spawDelay = 0.5f;
	

	private float m_xmax,m_xmin;
	private bool m_reverseX;
	private float m_width = 10.0f;
	private float m_height = 5.0f;
	private int m_level = 1; //This level modifies the algorithm to spawn different (and more difficult) ships
	private float m_totalWeight = 0.0f;
	private bool m_spawing = false;
	private bool m_emptyCalled = false;

	//-----------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		if(randomSpeed)
		{
			speed = Random.Range(minSpeed,maxSpeed);
		}
		//SpawEnemies();
	}
	//-----------------------------------------------------------------------------------------------------
	public void initSquadron( float width, float height, float xMax, float xMin, int level)
	{
		m_width = width;
		m_height = height;
		m_xmax = xMax;
		m_xmin = xMin;
		m_level = level;

		updateTotalWeight();

	}
	//-----------------------------------------------------------------------------------------------------
	//This method Respaw all the formation enemies
	void spawEnemies()
	{
		if(enemyPrefab.Length > 0)
		{
			//Get the spawn enemy based on level
			GameObject enemyP = getEnemy();
			if(enemyP != null)
			{
				//Draw all the enemy positions
				foreach(Transform child in this.transform)
				{
					Quaternion quat = Quaternion.Euler(0,0,180);
					GameObject enemy = Instantiate(enemyP, child.position, quat) as GameObject;
					enemy.transform.parent = child;
				}
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//Spaws the enmies one by one with a delay
	public void spawUntilFull()
	{
		m_spawing = true;
		int numSquadronChildren = this.transform.childCount;
		int currentEnemiesCreated = 0;
		Transform freePosition = NextFreePosition();
		if(freePosition)
		{
			GameObject enemyP = getEnemy();
			if(enemyP != null && currentEnemiesCreated < numSquadronChildren)
			{
				Quaternion quat = Quaternion.Euler(0,0,180);
				GameObject enemy = Instantiate(enemyP, freePosition.position, quat) as GameObject;
				enemy.transform.parent = freePosition;
				currentEnemiesCreated++;
			}
		}
		//Invoke the method again only if there is anther free position
		if(NextFreePosition()&& currentEnemiesCreated < numSquadronChildren)
		{
			Invoke ("spawUntilFull",spawDelay);
		}
		else
		{
			m_spawing = false;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method draws a square defining the enemySpawer space
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(this.transform.position,new Vector3(m_width, m_height));
	}

	//-----------------------------------------------------------------------------------------------------
	// Update is called once per frame
	void Update () {
		MoveHorizontally();

		if(!m_spawing && !m_emptyCalled && AllMembersAreDead())
		{
			onEmptySquadron();
			m_emptyCalled =true;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//Move the squadron horizontally
	void MoveHorizontally()
	{
		if(!m_reverseX)
		{
			//Move right
			if( this.transform.position.x <= m_xmax)
			{
				this.transform.position += Vector3.right * speed * Time.deltaTime;
			}
			else
			{
				m_reverseX = true;
			}
		}
		else
		{
			//Move Left
			if( this.transform.position.x >= m_xmin )
			{
				this.transform.position += Vector3.left * speed * Time.deltaTime;
			}
			else
			{
				m_reverseX = false;
			}
		} 
	}
	//-----------------------------------------------------------------------------------------------------
	//Returns the next free position (in order) depending of the child positions
	Transform NextFreePosition()
	{
		foreach(Transform child in this.transform)
		{
			if(child.childCount == 0)
			{
				return child;
			}
		}
		return null;
	}
	//-----------------------------------------------------------------------------------------------------
	// Checks if all the enemies are dead
	bool AllMembersAreDead()
	{
		foreach(Transform child in this.transform)
		{
			if(child.childCount != 0)
			{
				return false;
			}
		}
		return true;
	}
	//-----------------------------------------------------------------------------------------------------
	//Return the enemy depending of a random value and the level. If something fail, it will return null
	GameObject getEnemy()
	{
		//Return the enemy based on the level.
		//This means that when the level increase, there probabilities to get heavier enemies increase too
		//The idea is create ranges between the enemies types. Depending of the type and the level, the range will increase/decrease
		//To do this, return weight of each enemy (between 0 and 1). in an unifor distributuion (each enemy return 1) each enemy will have the same probability 
		//if there are 10 enemies, each will return 0.1, and depending of the random (that will be increasing) you select the enemy.
		//If the weights are different, each enemy will have different probability

		//First get one random Value between 0 and 1
		float randomRange = Random.value;
		//Create range, and check if is bigger than the random value (it will increase differently depending of the level/weights) 
		float range = 0.0f;
		for(int i = 0; i < enemyPrefab.Length; i++)
		{
			Enemy cEnemy = enemyPrefab[i].GetComponent<Enemy>();
			//Get the weight and distribute between 0 and 1 
			range += cEnemy.getWeight(m_level) / m_totalWeight;
			//If the range pass the random, it's the selected one
			if(range >= randomRange)
				return enemyPrefab[i];
		}
		return null;
	}
	//-----------------------------------------------------------------------------------------------------
	void updateTotalWeight()
	{
		m_totalWeight = 0.0f;
		for(int i = 0; i < enemyPrefab.Length; i++)
		{
			m_totalWeight += enemyPrefab[i].GetComponent<Enemy>().getWeight(m_level);
		}
	}
	//-----------------------------------------------------------------------------------------------------
}
