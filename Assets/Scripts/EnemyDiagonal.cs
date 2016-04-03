using UnityEngine;
using System.Collections;
//-----------------------------------------------------------------------------------------------------
//This class create double diagonal shots with a random angle
//After shot in diagonal, it shot several times in vertical
public class EnemyDiagonal : EnemyShooter {

	public float maxAngle = 45f;
	public int diagonalBurstTimes = 3;//Number of shots in diagonal
	public int verticalBurstTimes = 3;//Number of shots in vertical
	public float offsetX; //Offset in x from the middle of the spaceship
	public float burstSpeed = 0.2f;

	private bool m_shootDiagonal = true; //This var stores if the spaceship will shoot diagonal or not
	private int m_burstShots = 0;
	//-----------------------------------------------------------------------------------------------------
	protected override void Fire()
	{
		if(m_shootDiagonal)
		{
			m_burstShots = diagonalBurstTimes;
			FireDiagonal();
		}
		else
		{
			m_burstShots = verticalBurstTimes;
			FireVertical();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method shots a diagonal burst
	private void FireDiagonal()
	{
		//We are going to shoot a burst so we don't want to be call to shoot again
		m_canFire = false;

		//Create two diagonal projectiles
		float angle = Random.Range(5, maxAngle);
		Quaternion quat1 = Quaternion.Euler(0,0,180 + angle);
		Quaternion quat2 = Quaternion.Euler(0,0,180 - angle);
		Vector3 offset = new Vector3(offsetX, 0.0f, 0.0f);
		Vector3 newPosition1 = this.transform.position + offset;
		Vector3 newPosition2 = this.transform.position - offset;
		Vector2 newVelocity1 = new Vector2((angle / maxAngle) * projectileSpeed, -projectileSpeed);
		Vector2 newVelocity2 =  new Vector2((-angle / maxAngle) * projectileSpeed, -projectileSpeed);

		//Shot projectiles
		shotProjectile(newPosition1, quat1, newVelocity1);
		shotProjectile(newPosition2, quat2, newVelocity2);

		//Update burst and check end condition
		if(m_burstShots > 0)
		{
			m_burstShots--;
			Invoke("FireDiagonal",burstSpeed);
		}
		else
		{
			m_canFire = true;
			m_shootDiagonal = false;
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//This method shots a vertical burst
	private void FireVertical()
	{
		//We are going to shoot a burst so we don't want to be call to shoot again
		m_canFire = false;
		//Create a vertical projectile
		Vector3 newPosition1 = this.transform.position;
		//Apply offset
		GameObject projectile = Instantiate(enemyProjectile, newPosition1, Quaternion.identity) as GameObject;
		projectile.rigidbody2D.velocity = new Vector2(0.0f, -projectileSpeed);
		Destroy(projectile, 2.0f);

		AudioSource.PlayClipAtPoint(fireSound, this.transform.position,0.25f);
		//Update burst and check end condition
		if(m_burstShots > 0)
		{
			m_burstShots--;
			Invoke("FireVertical", burstSpeed);
		}
		else
		{
			m_canFire = true;
			m_shootDiagonal = true;
		}
	}

}
