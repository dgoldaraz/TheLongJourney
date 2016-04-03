using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//-----------------------------------------------------------------------------------------------------
// This class will chooe between different shoots randomly
// Will be a selection of all the shots in the current game - Simple/Double/Diagonal/Bomb
// Also, can be a burst random

public class EnemySpecial : EnemyShooter {

	private enum FireTypes{Simple, Double, Diagonal, Bomb, DoubleBomb}
	
	public float burstSpeed = 0.2f;

	//Simple Shoot controls
	public bool simpleShoot = true;
	public bool simpleShootBurst = true;
	public int maxSimpleBurst = 3;

	//Double shoots controls
	public GameObject doubleProjectile;
	public bool doubleShoot = true;
	public bool doubleShootBurst = true;
	public float offsetXDoubleShoot = 1.0f;
	public float offsetYDoubleShoot = 1.0f;
	public int maxDoubleBurst = 3;

	//Diagonal shoots controls
	public GameObject diagonalProjectile;
	public bool diagonalShoot = true;
	public bool diagonalShootBurst = true;
	public float offsetXDoubleDiagonalShoot = 1.0f;
	public float offsetYDoubleDiagonalShoot = 1.0f;
	public int maxDoubleDiagonalBurst = 3;
	public bool randomDoubleAngle = true;
	public float maxDiagonalAngle = 45.0f;


	//Bomb shoots controls
	public GameObject bombProjectile;
	public bool bombShoot = true;
	public bool doubleBombShoot = true;
	public bool bombShootBurst = true;
	public bool doubleBombShootBurst = true;
	public float offsetYBombShoot = 1.0f;
	public float offsetXDoubleBombShoot = 1.0f;
	public int maxBombBurst = 3;
	public int maxDoubleBombBurst = 3;
	public float bombTorque = 15.0f;
	public float bombTimeOfLife = 4.0f;
	public float bombSpeed = 1.0f;

	

	//private
	private int m_burstShooted = 0;
	private List<FireTypes> m_fires = new List<FireTypes>();

	void Start()
	{
		//Set the possible fires depending on the user specificactions
		int firesIndex = 0;
		if(simpleShoot)
		{
			m_fires.Add(FireTypes.Simple);
			firesIndex++;
		}
		if(doubleShoot)
		{
			m_fires.Add(FireTypes.Double);
			firesIndex++;
		}
		if(diagonalShoot)
		{
			m_fires.Add(FireTypes.Diagonal);
			firesIndex++;
		}
		if(bombShoot)
		{
			m_fires.Add(FireTypes.Bomb);
			firesIndex++;
		}
		if(doubleBombShoot)
		{
			m_fires.Add(FireTypes.DoubleBomb);
			firesIndex++;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	// Fire override method
	protected override void Fire()
	{
		if(m_canFire)
		{
			//By default no burst
			m_burstShooted = 0;
			m_canFire = false;

			//Decide the type of fire

			FireTypes currentFire = m_fires[Random.Range(0, m_fires.Count - 1)];
		
			switch(currentFire)
			{
			case FireTypes.Simple:

				//First decide if there is a burst or not
				if(simpleShootBurst)
				{
					m_burstShooted = Random.Range (1,maxSimpleBurst) - 1;
				}
				FireSimple();
				break;
			case FireTypes.Double:
				//First decide if there is a burst or not
				if(doubleShootBurst)
				{
					m_burstShooted = Random.Range (1,maxSimpleBurst) - 1;
				}
				FireDouble();
				break;
			case FireTypes.Diagonal:
				//First decide if there is a burst or not
				if(diagonalShootBurst)
				{
					m_burstShooted = Random.Range (1,maxSimpleBurst) - 1;
				}
				FireDiagonal();
				break;
			case FireTypes.Bomb:
				//First decide if there is a burst or not
				if(bombShootBurst)
				{
					m_burstShooted = Random.Range (1,maxSimpleBurst) - 1;
				}
				FireBomb();
				break;
			case FireTypes.DoubleBomb:
				//First decide if there is a burst or not
				if(doubleBombShootBurst)
				{
					m_burstShooted = Random.Range (1,maxSimpleBurst) - 1;
				}
				FireBombDouble();
				break;
			default:
				Debug.LogError( "No fire selected");
				FireSimple();
				break;
			}


		}

	}

	//-----------------------------------------------------------------------------------------------------
	// Fire simple

	void FireSimple()
	{
		Quaternion quat = Quaternion.Euler(0,0,180);
		Vector2 vel = new Vector2(0.0f, -projectileSpeed);
		shotProjectile(this.transform.position, quat, vel,2,enemyProjectile);
		FireBurst("FireSimple",burstSpeed);
	}

	//-----------------------------------------------------------------------------------------------------
	// Fire double
	void FireDouble()
	{
		//Double Shoot based on the offset
		Quaternion quat = Quaternion.Euler(0,0,180);
		Vector3 offsetX = new Vector3(offsetXDoubleShoot, 0.0f, 0.0f);
		Vector3 offsetY = new Vector3(0.0f, offsetYDoubleShoot, 0.0f);
		Vector3 newPosition1 = this.transform.position + offsetX + offsetY;
		Vector3 newPosition2 = this.transform.position - offsetX + offsetY;

		Vector2 newVelocity = new Vector2(0.0f, -projectileSpeed);
		
		//Shoot Projectiles
		shotProjectile(newPosition1, quat, newVelocity,2,doubleProjectile);
		shotProjectile(newPosition2, quat, newVelocity,2,doubleProjectile);
		FireBurst("FireDouble",burstSpeed);
	}
	//-----------------------------------------------------------------------------------------------------
	// Fire double diagonal -  simpleBurst defines if the user is shooting a burst in a range
	void FireDiagonal()
	{
		float angle = maxDiagonalAngle;
		if(randomDoubleAngle)
		{
			angle = Random.Range(5, maxDiagonalAngle);
		}

		Quaternion quat1 = Quaternion.Euler(0,0,180 + angle);
		Quaternion quat2 = Quaternion.Euler(0,0,180 - angle);
		Vector3 offsetX = new Vector3(offsetXDoubleDiagonalShoot, 0.0f, 0.0f);
		Vector3 offsetY = new Vector3(0.0f, offsetYDoubleDiagonalShoot, 0.0f);

		Vector3 newPosition1 = this.transform.position + offsetX + offsetY;
		Vector3 newPosition2 = this.transform.position - offsetX + offsetY;

		Vector2 newVelocity1 = new Vector2((angle / maxDiagonalAngle) * projectileSpeed, -projectileSpeed);
		Vector2 newVelocity2 =  new Vector2((-angle / maxDiagonalAngle) * projectileSpeed, -projectileSpeed);
		
		//Shot projectiles
		shotProjectile(newPosition1, quat1, newVelocity1,2,diagonalProjectile);
		shotProjectile(newPosition2, quat2, newVelocity2,2,diagonalProjectile);
		FireBurst("FireDiagonal", burstSpeed);
	}

	//-----------------------------------------------------------------------------------------------------
	// Fire bomb / Fire double bomb 
	void FireBomb()
	{
		Vector3 offsetY = new Vector3(0.0f, offsetYBombShoot, 0.0f);
		Vector3 newPosition1 = this.transform.position + offsetY;
		Vector2 velocity = new Vector2(0.0f, -bombSpeed);
		//Set a timeOfLife greater than the real one, so the bomb can explode
		GameObject projectile = shotProjectile(newPosition1, Quaternion.identity, velocity,bombTimeOfLife + 1.0f,bombProjectile);
		projectile.GetComponent<BombProjectile>().setTimeOfLife(Random.Range(0.5f,bombTimeOfLife));
		projectile.rigidbody2D.AddTorque(bombTorque);

		FireBurst("FireBomb", burstSpeed *2.0f);
	}

	void FireBombDouble()
	{
		Vector3 offsetX = new Vector3(offsetXDoubleBombShoot, 0.0f, 0.0f);
		Vector3 offsetY = new Vector3(0.0f, offsetYBombShoot, 0.0f);
		Vector3 newPosition1 = this.transform.position + offsetX + offsetY;
		Vector3 newPosition2 = this.transform.position - offsetX + offsetY;

		Vector2 velocity = new Vector2(0.0f, -bombSpeed);
		float bombTime = Random.Range(0.5f,bombTimeOfLife);
		//Set a timeOfLife greater than the real one, so the bomb can explode
		GameObject projectile = shotProjectile(newPosition1, Quaternion.identity, velocity,bombTimeOfLife + 1.0f,bombProjectile);
		projectile.GetComponent<BombProjectile>().setTimeOfLife(bombTime);
		projectile.rigidbody2D.AddTorque(bombTorque);

		//Second Bomb
		projectile = shotProjectile(newPosition2, Quaternion.identity, velocity,bombTimeOfLife + 1.0f,bombProjectile);
		projectile.GetComponent<BombProjectile>().setTimeOfLife(bombTime);
		projectile.rigidbody2D.AddTorque(bombTorque);

		FireBurst("FireBombDouble", burstSpeed * 3.0f);
	}

	//-----------------------------------------------------------------------------------------------------
	// Method to create a burst
	void FireBurst(string recallmethod, float invokeTime)
	{
		if(m_burstShooted > 0)
		{
			m_burstShooted--;
			Invoke(recallmethod,invokeTime);
		}
		else
		{
			m_canFire = true;
		}
	}
}
