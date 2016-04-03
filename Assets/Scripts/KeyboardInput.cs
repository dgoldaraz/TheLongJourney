using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KeyboardInput : MonoBehaviour {

	public enum InputEntry{Keyboard, Mouse};
	private InputEntry m_currentInput;

	//-----------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		m_currentInput = InputEntry.Mouse;
	}

	//-----------------------------------------------------------------------------------------------------
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.K))
		{
			SwitchInput();
		}
	
	}
	//-----------------------------------------------------------------------------------------------------
	//Switch between using keyboard and mouse input
	void SwitchInput()
	{
		if(m_currentInput == InputEntry.Mouse)
		{
			gameObject.GetComponent<Text>().text = "Keyboard";
			Manager ply = GameObject.FindObjectOfType<Manager>();
			if(ply)
			{
				ply.setInputEntry(1);
			}
			m_currentInput = InputEntry.Keyboard;
		}
		else
		{
			gameObject.GetComponent<Text>().text = "Mouse";
			Manager ply = GameObject.FindObjectOfType<Manager>();
			if(ply)
			{
				ply.setInputEntry(0);
			}
			m_currentInput = InputEntry.Mouse;
		}
	}
	//-----------------------------------------------------------------------------------------------------
}
