using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


	public delegate void NotifyGUI(string text);
	public static event NotifyGUI onNotifyGUI;
	public static event NotifyGUI onRegenerate;

	public float speed = 1.0f;
	public float padding = 1.0f;
	public GameObject healthBar;
	public GameObject projectile;
	public float proyectileSpeed = 10.0f;
	public float fireRate = 0.2f;
	public float health = 100.0f;
	public AudioClip fireSound;
	public AudioClip explosionSound;
	public AudioClip hitSound;
	public AudioClip regenSound;
	public AudioClip powerUpSound;
	public float regenerateRateSeconds = 1.0f; // Seconds to regenerate health
	public float regenerationRate = 0.1f; // % of increasing health
	public float scoreMultiplier = 1.0f; //This method establish how the score is going to increase/decrease.
	public Vector2 minMaxPowerupProb = new Vector2(20.0f, 40.0f); //Define the min/max probability of increase in the powerupProbability
	public Vector3 offsetDoubleShoot;

	private float m_xmax,m_xmin;
	private bool m_isFiring = false;
	private bool m_regenerate = false;
	private float m_maxHealth;
	private float m_regenerationRate;
	private float m_regenrationTime = 0.0f;
	private bool m_destroyCalled = false;
	private float m_powerUpPercent = 0;
	private bool m_doubleShoot = false;
	private float m_damage;
	private float m_damageIncrease;
	private float m_healthIncrease;


	//-----------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start () {
		float distance = transform.position.z - Camera.main.transform.position.z; 
		Vector3 leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0,0,distance));
		Vector3 rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1,0,distance));
		m_xmax = rightMost.x - padding;
		m_xmin = leftMost.x + padding;
		m_maxHealth = health;
		m_regenerationRate = m_maxHealth * regenerationRate;

		if(healthBar)
		{
			healthBar.GetComponent<HealthController>().setInitialHealth(health);
		}
		StarFieldManager.onIncreasingSpeed += StartRegeneration;
		StarFieldManager.onIncreasingSpeed += CalculatePowerUp;
		StarFieldManager.onDecreaseSpeed += StopRegeneration;
		m_powerUpPercent = 0;
		m_damage = projectile.GetComponent<Projectile>().getDamage();
		m_damageIncrease = m_damage * 0.5f;
		m_healthIncrease = m_maxHealth * 0.5f;
	}
	//-----------------------------------------------------------------------------------------------------
	void OnDestroy() 
	{
		StarFieldManager.onIncreasingSpeed -= StartRegeneration;
		StarFieldManager.onIncreasingSpeed -= CalculatePowerUp;
		StarFieldManager.onDecreaseSpeed -= StopRegeneration;
	}
	//-----------------------------------------------------------------------------------------------------
	void Fire()
	{
		if(m_doubleShoot)
		{

			//Creates a laser object and destroys it after one second
			GameObject laser = Instantiate(projectile, this.transform.position + offsetDoubleShoot, Quaternion.identity) as GameObject;
			laser.rigidbody2D.velocity = new Vector2(0.0f, proyectileSpeed);
			laser.GetComponent<Projectile>().setDamage(m_damage);
			Destroy (laser, 2.0f);

			Vector3 xInverseOffset = offsetDoubleShoot;
			xInverseOffset.x *= -1.0f;
			laser = Instantiate(projectile, this.transform.position + xInverseOffset, Quaternion.identity) as GameObject;
			laser.rigidbody2D.velocity = new Vector2(0.0f, proyectileSpeed);
			laser.GetComponent<Projectile>().setDamage(m_damage);
			Destroy (laser, 2.0f);
			//sound
			AudioSource.PlayClipAtPoint(fireSound, this.transform.position, 0.25f);

		}
		else
		{
			//Creates a laser object and destroys it after one second
			GameObject laser = Instantiate(projectile, this.transform.position, Quaternion.identity) as GameObject;
			laser.GetComponent<Projectile>().setDamage(m_damage);
			laser.rigidbody2D.velocity = new Vector2(0.0f, proyectileSpeed);
			AudioSource.PlayClipAtPoint(fireSound, this.transform.position, 0.25f);
			Destroy (laser, 2.0f);
		}
	}
	//-----------------------------------------------------------------------------------------------------
	// Update is called once per frame
	void Update () {

		Vector3 newPosition = this.transform.position;
		if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			newPosition += Vector3.right * speed * Time.deltaTime;
		}
		else if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			newPosition += Vector3.left * speed * Time.deltaTime;
		}
		newPosition.x = Mathf.Clamp(newPosition.x, m_xmin, m_xmax);
		this.transform.position = newPosition;

		//Fire (or stop) depending of the keys/mouse
		if(!m_isFiring && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
		{
			//Calls the method Fire using the fireRate
			InvokeRepeating("Fire",Mathf.Epsilon, fireRate);
			m_isFiring = true;
		}

		if(m_isFiring && (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0)))
		{
			CancelInvoke("Fire");
			m_isFiring = false;
		}
		//Regeneration stuff
		if(m_regenerate && health < m_maxHealth)
		{
			if(m_regenrationTime > regenerateRateSeconds)
			{

				//Update health with regeneration
				health += m_regenerationRate;
				healthBar.GetComponent<HealthController>().updateHealth(health);
				m_regenrationTime = 0.0f;
				//AudioSource.PlayClipAtPoint(regenSound, this.transform.position, 0.5f);
				float healthPercent = (health /m_maxHealth ) * 100;
				int healthPercentInt = (int)healthPercent;
				if(healthPercentInt > 100)
				{
					healthPercentInt = 100;
				}
				string regenerateText = "Fixing Spaceship " + healthPercentInt.ToString ();
				if(onRegenerate != null)
				{
					onRegenerate(regenerateText);
				}
			}
			else
			{
				m_regenrationTime += Time.deltaTime;
			}
		}
	}
	//-----------------------------------------------------------------------------------------------------
	void OnTriggerEnter2D(Collider2D collider)
	{
		Projectile playerProjectile = collider.gameObject.GetComponent<Projectile>();
		if(playerProjectile != null)
		{
			//If the enemy it's hit by a laser, destroy the laser and substract the damage to the enemy life
			float damage = playerProjectile.getDamage();
			playerProjectile.Hit();
			HitDamage(damage);
			return;
		}

		Meteor meteorHit = collider.gameObject.GetComponent<Meteor>();
		if(meteorHit != null)
		{
			HitDamage(meteorHit.getDamage());
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method handles what happend when the enemy it's being hit
	void HitDamage(float damage)
	{
		AudioSource.PlayClipAtPoint(hitSound, this.transform.position, 0.5f);
		health -= damage;
		if(healthBar)
		{
			healthBar.GetComponent<HealthController>().updateHealth(health);
		}
		//If the enemy has less than 0, destroy it
		if(health <= 0 && !m_destroyCalled)
		{
			StartCoroutine(Die());
			m_destroyCalled = true;

		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method handles when the player Dies
	IEnumerator Die()
	{
		AudioSource.PlayClipAtPoint(explosionSound, this.transform.position, 0.25f);
		SpriteRenderer[] sprR = this.GetComponentsInChildren<SpriteRenderer>();
		foreach( SpriteRenderer sp in sprR)
		{
			sp.enabled = false;
		}
		yield return new WaitForSeconds(1);
		LevelManager lvl = GameObject.FindObjectOfType<LevelManager>();
		Destroy (this.gameObject);
		if(lvl)
		{
			lvl.LoadNextLevel();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method starts the regeneration of the health in the spaceship
	void StartRegeneration()
	{
		m_regenerate = true;
		m_regenrationTime = 0.0f;
	}
	//-----------------------------------------------------------------------------------------------------
	//This method stops the regeneration of the health in the spaceship
	void StopRegeneration()
	{
		m_regenerate = false;
		m_regenrationTime = 0.0f;
	}
	//-----------------------------------------------------------------------------------------------------
	//Getter for scoreMultiplier
	public float getScoreMultiplier()
	{
		return scoreMultiplier;
	}
	//-----------------------------------------------------------------------------------------------------
	//This method is called everytime the speed high up.In this method we increase the powerUp % until you reach 100. In that case we apply a powerup to the player
	void CalculatePowerUp()
	{
		//First, check if the percent it's highest than 100
		if(m_powerUpPercent >= 100.0)
		{
			m_powerUpPercent = 0.0f;
			//Apply one random powerup
			int minIndex = m_doubleShoot ? 1:0;
			int powerupRandom = Random.Range(minIndex, 2);

			switch(powerupRandom)
			{
			case 0:
				//Double Shoot
				ApplyDoubleShoot();
				break;
			case 1:
				ApplyIncreaseDamage();
				break;
			case 2:
				ApplyIncreaseHealth();
				break;
			default:
				Debug.Log("Power up type undefined");
				break;
			}
			AudioSource.PlayClipAtPoint(powerUpSound, this.transform.position, 0.5f);
			//Increase a little the probability of having powerUps
			minMaxPowerupProb.x += minMaxPowerupProb.x * 0.2f;
			minMaxPowerupProb.y += minMaxPowerupProb.y * 0.1f;
		}
		else
		{
			//Increase the powerup percent randomly
			float increase = Random.Range(minMaxPowerupProb.x, minMaxPowerupProb.y);
			m_powerUpPercent += increase;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method apply the doubleShoot powerup
	void ApplyDoubleShoot()
	{
		m_doubleShoot = true;
		if(onNotifyGUI != null)
		{
			onNotifyGUI("Double Gun PowerUp!");
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method is called everytime the speed high up.In this method we increase the powerUp % until you reach 100. In that case we apply a powerup to the player
	void ApplyIncreaseHealth()
	{
		m_maxHealth += m_healthIncrease;
		if(healthBar)
		{
			healthBar.GetComponent<HealthController>().setInitialHealth(m_maxHealth);
		}
		if(onNotifyGUI != null)
		{
			onNotifyGUI("Increase Health PowerUp!");
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method is called everytime the speed high up.In this method we increase the powerUp % until you reach 100. In that case we apply a powerup to the player
	void ApplyIncreaseDamage()
	{
		m_damage += m_damageIncrease;
		if(onNotifyGUI != null)
		{
			onNotifyGUI("Increase Damage Powerup!");
		}
	}
}
