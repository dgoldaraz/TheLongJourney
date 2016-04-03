using UnityEngine;
using System.Collections;
//----------------------------------------------------------------
//This method deals whith how the bomb behaves 
public class BombProjectile : MonoBehaviour {

	public GameObject subProjectiles;
	public int numberOfBombProjectiles;
	public AudioClip bombExplosion;
	public float projectileSpeed = 7.0f;
	public float lifeOfProjectiles = 2.0f;

	private float m_timeOfLife = 4.0f;
	private float m_currentTime = 0.0f;
	
	//----------------------------------------------------------------
	// This is call when the bomb dies, create a number of projectiles in the 360 grades
	void DestroyBomb()
	{
		AudioSource.PlayClipAtPoint(bombExplosion, this.transform.position,0.25f);

		float angleRange = 360.0f / (numberOfBombProjectiles);// we substract one for the first shoot
		float currentAngle = 0;
		for(int i = 0; i < numberOfBombProjectiles; ++i)
		{
			float yVelocity = Mathf.Cos(currentAngle * Mathf.Deg2Rad);
			float xVelocity = Mathf.Sin(currentAngle * Mathf.Deg2Rad);
			Vector2 velocity = new Vector2( xVelocity * projectileSpeed, yVelocity* projectileSpeed);
			GameObject projectile = Instantiate(subProjectiles, this.transform.position, Quaternion.Euler(0,0,360f - currentAngle)) as GameObject;
			projectile.rigidbody2D.velocity = velocity;
			Destroy(projectile, lifeOfProjectiles);
			currentAngle += angleRange;
		}
	}
	//----------------------------------------------------------------
	// This method sets the timeOfLife of this bomb
	public void setTimeOfLife(float time)
	{
		m_timeOfLife = time;
	}
	//----------------------------------------------------------------
	// Update Method
	void Update ()
	{
		m_currentTime += Time.deltaTime;
		if(m_currentTime > m_timeOfLife)
		{
			DestroyBomb();
			Destroy (this.gameObject);
		}
	}
}
