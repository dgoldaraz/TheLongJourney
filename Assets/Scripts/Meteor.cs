using UnityEngine;
using System.Collections;

public class Meteor : MonoBehaviour {


	public Sprite[] meteorSprites;
	public float maxScale = 3.0f;
	public float maxTorque = 100.0f;
	public int maxDamage = 100;
	public int minDamage = 25;

	private int m_damage;
	// Use this for initialization
	//Set a random sprite when initialize
	void Start ()
	{
		int spriteCount = meteorSprites.Length;
		int spriteChoose = Random.Range(0,spriteCount-1);
		SpriteRenderer sp = GetComponent<SpriteRenderer>();
		sp.sprite = meteorSprites[spriteChoose];

		//Create a random scale
		float newScale = Random.Range(1.0f, maxScale);
		Vector3 newScaleVector = new Vector3(newScale, newScale, 1.0f);
		this.transform.localScale = newScaleVector;

		//Add a random torque
		this.rigidbody2D.AddTorque(Random.Range (1.0f, maxTorque));

		//Set a random Damage
		m_damage = Random.Range (minDamage, maxDamage);
	}
	//----------------------------------------------------------------
	// Return the current Damage 
	public float getDamage()
	{
		return m_damage;
	}
	//----------------------------------------------------------------
}
