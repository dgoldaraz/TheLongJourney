using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour {

	public float shotsPerSecond = 0.5f;
	public bool m_canFire = true;
	public float projectileSpeed = 7.0f;
	public GameObject enemyProjectile;
	public AudioClip fireSound;
	public bool incrementWhenLowHealth = true;
	//-----------------------------------------------------------------------------------------------------
	// Update is called once per frame
	void Update () 
	{
		//Shoot
		float probability = Time.deltaTime * shotsPerSecond;
		//Increase this depending of the life in the spaceship
		Enemy enemyComponent = this.gameObject.GetComponent<Enemy>();
		if(enemyComponent && incrementWhenLowHealth)
		{
			float increment = (enemyComponent.getHealth() / enemyComponent.getMaxHealth());
			if(increment > 0.1f)
			{
				probability /= increment;
			}

		}
		if(Random.value < probability && m_canFire)
		{
			Fire();
		}
	}
	//-----------------------------------------------------------------------------------------------------
	//FireMethod
	protected virtual void Fire()
	{
		Quaternion quat = Quaternion.Euler(0,0,180);
		Vector2 vel = new Vector2(0.0f, -projectileSpeed);
		shotProjectile(this.transform.position, quat, vel);
	}
	//-----------------------------------------------------------------------------------------------------
	//This method allow to create a projectile with a position and a rotation, and a velocity applied
	public GameObject shotProjectile(Vector3 position, Quaternion rotation, Vector2 velocity,float timeToDead = 2.0f, GameObject enemyProj = null)
	{
		if(enemyProj == null)
		{
			enemyProj = enemyProjectile;
		}
		
		GameObject projectile = Instantiate(enemyProj, position, rotation) as GameObject;
		projectile.rigidbody2D.velocity = velocity;//new Vector2(0.0f, -projectileSpeed);
		Destroy(projectile, timeToDead);
		AudioSource.PlayClipAtPoint(fireSound, this.transform.position,0.25f);
		return projectile;
	}

}
