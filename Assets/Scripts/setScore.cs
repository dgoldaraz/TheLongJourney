using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class setScore : MonoBehaviour {

	//-----------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start () {
		//Assumes that there is a manager in the scene and this script it's attached to a text, that will show the score 
		Manager m = GameObject.FindObjectOfType<Manager>();
		Text txtObject = gameObject.GetComponent<Text>();
		if(m && txtObject)
		{
			int points = m.getScore();
			txtObject.text = "Score: " + points.ToString();
		}
		else
		{
			txtObject.text = "";
		}
	}
}
