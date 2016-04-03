using UnityEngine;
using System.Collections;
//-----------------------------------------------------------------------------------------------------
//This class create a double Shot for the enemy
public class EnemyDouble : EnemyShooter {

	public float offsetX;
	//-----------------------------------------------------------------------------------------------------
	protected override void Fire()
	{
		//Double Shoot based on the offset
		Quaternion quat = Quaternion.Euler(0,0,180);
		Vector3 offset = new Vector3(offsetX, 0.0f, 0.0f);
		Vector3 newPosition1 = this.transform.position + offset;
		Vector3 newPosition2 = this.transform.position - offset;
		Vector2 newVelocity = new Vector2(0.0f, -projectileSpeed);

		//Shoot Projectiles
		shotProjectile(newPosition1, quat, newVelocity);
		shotProjectile(newPosition2, quat, newVelocity);
	}
}
