using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

	public Color mainColour;
	public Color backgroundColour;
	private float m_health = 150;
	private Texture2D m_mainTexture;
	private Texture2D m_backGroundTexture;


	//------------------------------------------------------
	// Use this for initialization
	//Create Textures and initialize SpriteRender
	void Start () {

		Rect unit = new Rect(0, 0, 1, 1);

		//Create main Texture


		//Create Background
		m_backGroundTexture = new Texture2D(1, 1);
		m_backGroundTexture.SetPixel(0, 0, backgroundColour);
		m_backGroundTexture.wrapMode = TextureWrapMode.Repeat;
		m_backGroundTexture.Apply();

		SpriteRenderer lSr = this.gameObject.GetComponent<SpriteRenderer>();
		lSr.sprite = Sprite.Create(m_backGroundTexture, unit, Vector3.zero);
	
		//Set life in child
		m_mainTexture = new Texture2D(1, 1);
		m_mainTexture.SetPixel(0, 0, mainColour);
		m_mainTexture.wrapMode = TextureWrapMode.Repeat;
		m_mainTexture.Apply();

		Rect unit2 = new Rect(0, 0, 1, 1);
		foreach(Transform tr in this.transform)
		{
			tr.GetComponent<SpriteRenderer>().sprite = Sprite.Create(m_mainTexture, unit2, Vector3.zero);;
		}
	}
	//------------------------------------------------------
	//Sets the initial health
	public void setInitialHealth(float totalHealth)
	{
		m_health = totalHealth;
	}
	//------------------------------------------------------
	//This method update the health bar depending on the health passed and the total health
	public void updateHealth(float health)
	{
		//Calculate the current dimension based on the total life
		float xScale = health/m_health;

		foreach(Transform t in this.transform)
		{
			t.localScale = new Vector3( xScale, t.localScale.y,t.localScale.z);;
		}
	}
	//------------------------------------------------------
}
