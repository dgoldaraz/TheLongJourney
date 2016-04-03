using UnityEngine;
using System.Collections;

public class StarFieldManager : MonoBehaviour {

	//This class Stores and Manage the StarField System
	//It's a Singleton

	public delegate void NotifyIncreasing();
	public static event NotifyIncreasing onIncreasingSpeed;

	public delegate void NotifyDecreasing();
	public static event NotifyDecreasing onDecreaseSpeed;

	static StarFieldManager instance = null;

	public float highVelocity = 10.0f;
	public float lowestVelcity = 5.0f;
	public float secondsToChange = 1.0f;
	public AudioClip highSpeedSound;
	public AudioClip lowSpeedSound;
	
	private bool m_increasing = false;
	private bool m_decreasing = false;
	private float m_desiredVelocity = 0.0f;
	private float m_step = 0.0f;
	private float m_PSRatio;


	public ParticleSystem[] particlesSystem; //List of particle Systems. It's supposed that the first one it's the main one
	//----------------------------------------------------------------------
	void Awake () {
		if(instance != null )
		{
			//Destroy duplicate Instances
			Destroy (this.gameObject);
		}
		else
		{
			//Assign the instance and don't destroy
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}
	//----------------------------------------------------------------------
	void Start()
	{
		//Get the ratio between speeds in particles systems
		if(particlesSystem.Length != 2)
		{
			Debug.Log("Too many Particles Systems, only the first 2 will be used");
		}
		else
		{
			m_PSRatio = particlesSystem[1].startSpeed / particlesSystem[0].startSpeed;
		}
	}
	//----------------------------------------------------------------------
	//Call every frame, if there is a velocity change, increase/decrease the velocity
	void Update()
	{
		if(m_increasing)
		{
			if(particlesSystem[0].startSpeed < m_desiredVelocity)
			{
				particlesSystem[0].startSpeed += m_step;
				particlesSystem[1].startSpeed += (m_step * m_PSRatio);
			}
			else
			{
				particlesSystem[0].startSpeed = m_desiredVelocity;
				particlesSystem[1].startSpeed = (m_desiredVelocity * m_PSRatio);
				m_increasing = false;
				AudioSource.PlayClipAtPoint(highSpeedSound, this.transform.position, 0.5f);
			}
		}
		if(m_decreasing)
		{
			if(particlesSystem[0].startSpeed > m_desiredVelocity)
			{
				particlesSystem[0].startSpeed -= m_step;
				particlesSystem[1].startSpeed -= (m_step * m_PSRatio);
			}
			else
			{
				particlesSystem[0].startSpeed = m_desiredVelocity;
				particlesSystem[1].startSpeed =(m_desiredVelocity * m_PSRatio);
				m_decreasing = false;
				AudioSource.PlayClipAtPoint(lowSpeedSound, this.transform.position, 0.5f);
			}
		}
	}
	//----------------------------------------------------------------------
	//Start the increase the velocity until it reach to hight velocity
	public void HighVelocity()
	{
		//Get current velocity and check that is not the desired
		float currentVelocity = particlesSystem[0].startSpeed;
		if(currentVelocity < highVelocity && !m_increasing)
		{
			m_increasing = true;
			m_desiredVelocity = highVelocity;
			float difVelocity = highVelocity - currentVelocity;
			m_step = (Time.deltaTime * difVelocity)/secondsToChange;

			if(onIncreasingSpeed != null)
			{
				onIncreasingSpeed();
			}
		}

	}
	//----------------------------------------------------------------------
	//Start to decrease the velocity until it reach the lowest velocity
	public void LowVelocity()
	{
		//Get current velocity and check that is not the desired
		float currentVelocity = particlesSystem[0].startSpeed;;
		if(currentVelocity > lowestVelcity && !m_decreasing)
		{
			m_decreasing = true;
			m_desiredVelocity = lowestVelcity;
			float difVelocity = currentVelocity - lowestVelcity;
			m_step = (Time.deltaTime * difVelocity)/secondsToChange;

			if(onDecreaseSpeed != null)
			{
				onDecreaseSpeed();
			}
		}

	}
	//----------------------------------------------------------------------
	public bool isChanging()
	{
		return m_increasing || m_decreasing;
	}
}
