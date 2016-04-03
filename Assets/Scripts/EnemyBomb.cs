using UnityEngine;
using System.Collections;
//-----------------------------------------------------------------------------------------------------
//This class shoots two types -> one random bomb that will explode in small projectiles or a burst of X bombs in some directions
public class EnemyBomb : EnemyShooter {

	public float timeAfterFire = 0.5f;
	public int bombBurst = 4;
	public float maxRange = 45.0f;
	public float burstPerSecond = 0.2f;
	public float bombTimeOfLife = 4.0f;
	public float burstTime = 0.3f;

	private float m_angleRange = 0.0f;
	private float m_currenAngle = 0.0f;
	private int m_bombBurstFired = 0;
	private bool m_canFireBomb = true;

	//-----------------------------------------------------------------------------------------------------
	//Start Method
	void Start()
	{
		//Always at least 2 bombs
		if( bombBurst < 2)
		{
			bombBurst = 2;
		}
	}

	//-----------------------------------------------------------------------------------------------------
	//This method shots or a simple bomb vertical or a burst of bombs
	protected override void Fire()
	{
		if(!m_canFireBomb)
		{
			return;
		}
		else
		{
			m_canFireBomb = false;
		}

		float probability = Time.deltaTime * burstPerSecond;
		if(Random.value < probability && m_canFire)
		{
			//Calculate the maximum range (we are going to fire from max + to max -) and divide by the number of bombs - 2 (we always lunch at +maxRnge and -range)
			m_angleRange = (maxRange * 2.0f);
			if(bombBurst > 2)
			{
				m_angleRange = m_angleRange / (bombBurst-2);
			}
			//Start at max angle and substract the angle range shooting bombs
			m_currenAngle = maxRange;
			m_bombBurstFired = bombBurst;
			FireBurstBomb();
		}
		else
		{
			FireBomb();
		}

	}

	//-----------------------------------------------------------------------------------------------------
	//Fire a bomb vertically
	void FireBomb()
	{
		Quaternion quat = Quaternion.Euler(0,0,180);
		Vector2 vel = new Vector2(0.0f, -projectileSpeed);
		GameObject projectile = shotProjectile(this.transform.position, quat, vel, bombTimeOfLife+1.0f);
		projectile.GetComponent<BombProjectile>().setTimeOfLife(Random.Range(2.0f,bombTimeOfLife));
		Invoke("AllowFire", timeAfterFire);
	}
	//-----------------------------------------------------------------------------------------------------
	// Fire a burst of x bombs in a ragne define by maxAngle
    void FireBurstBomb()
	{
		m_bombBurstFired--;
		if(m_bombBurstFired >= 0)
		{
			Vector2 velocity = new Vector2((m_currenAngle / maxRange) * projectileSpeed, -projectileSpeed);
			//Set a timeOfLife greater than the real one, so the bomb can explode
			GameObject projectile = shotProjectile(this.transform.position, Quaternion.identity, velocity,bombTimeOfLife + 1.0f);
			projectile.GetComponent<BombProjectile>().setTimeOfLife(Random.Range(2.0f,bombTimeOfLife));
			m_currenAngle -= m_angleRange;
			Invoke ("FireBurstBomb", burstTime);
		}
		else
		{
			//Finish the brust, we can shoot again
			Invoke("AllowFire", timeAfterFire);
		}
	}

	//-----------------------------------------------------------------------------------------------------
	// This method allow to fire again
	void AllowFire()
	{
		m_canFireBomb = true;
	}
}
