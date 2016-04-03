using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	//Attribute definition
	public float health = 30.0f;
	public AudioClip explosion;
	public GameObject healthBar;
	public float minWeight = 0.1f; //Variable that stores the minimun wight between 0 and 1
	public int minLevelOfApperance = 0; //This stablish the minimum level of possible apperance of this object
	public int levelOfProbability = -1; //This variable changes the weight depending of the current level. It's suppose that this level is when the enemy it's going to return a maximum weight 

	private float m_initialHealth;
	//-----------------------------------------------------------------------------------------------------
	void Start()
	{
		m_initialHealth = health;
		if(healthBar)
		{
			healthBar.GetComponent<HealthController>().setInitialHealth(health);
		}
		if(minWeight < 0.0f)
		{
			minWeight *= -1.0f;
		}
		if( levelOfProbability == -1)
		{
			levelOfProbability = minLevelOfApperance + 2;
		}
	}

	//-----------------------------------------------------------------------------------------------------
	void OnTriggerEnter2D(Collider2D collider)
	{
		//This method is called when a Trigger collides with the enemy
		Projectile playerProjectile = collider.gameObject.GetComponent<Projectile>();;
		if(playerProjectile != null)
		{
			//If the enemy it's hit by a laser, destroy the laser and substract the damage to the enemy life
			float damage = playerProjectile.getDamage();
			playerProjectile.Hit();
			HitDamage(damage);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method handles what happend when the enemy it's being hit
	void HitDamage(float damage)
	{
		health -= damage;
		if(healthBar)
		{
			healthBar.GetComponent<HealthController>().updateHealth(health);
		}

		//If the enemy has less than 0, destroy it
		if(health <= 0)
		{
			Die();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method handles when the enemy dies
	void Die()
	{
		AudioSource.PlayClipAtPoint(explosion, this.transform.position,0.25f);
		Destroy (this.gameObject);
	}
	//-----------------------------------------------------------------------------------------------------
	// This function return the weight of this enemy depending of the level
	public float getWeight(int currentLevel)
	{
		if(currentLevel < minLevelOfApperance)
		{
			return 0.0f;
		}

		float dif = Mathf.Abs(currentLevel - levelOfProbability) * 1.0f;
		float weight = 1.0f - (dif * 0.1f);

		if(weight > 1.0)
		{
			//It's suppose that the currentLevel it's the maximum value
			weight /= currentLevel;
			if(weight > 1.0f)
			{
				weight = 1.0f;
			}
		}

		//Always return the minimum value
		if(weight < minWeight)
		{
			weight = minWeight;
		}
		if(weight < 0.0f )
		{
			weight = 0.0f;
		}
		return weight;
	}
	//-----------------------------------------------------------------------------------------------------
	// This method return the health of the enemy
	public float getHealth()
	{
		return health;
	}

	//-----------------------------------------------------------------------------------------------------
	// This method return the initial health of the enemy
	public float getMaxHealth()
	{
		return m_initialHealth;
	}

}
