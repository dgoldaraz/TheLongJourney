using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {


	public float damage = 10.0f;

	//--------------------------------------------------------------------------
	public float getDamage()
	{
		return damage;
	}
	//--------------------------------------------------------------------------
	//This method handles what happend when the projectile hits somethig
	public void Hit()
	{
		Destroy (this.gameObject);
	}
	//--------------------------------------------------------------------------
	//This method handles what happend when the projectile hits somethig
	public void setDamage(float newDamage)
	{
		damage = newDamage;
	}
	//--------------------------------------------------------------------------
}
