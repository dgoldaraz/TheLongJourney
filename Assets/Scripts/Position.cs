using UnityEngine;
using System.Collections;

public class Position : MonoBehaviour {

	//This method draws a sphere for define a position
	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, 0.5f);
	}
	//-----------------------------------------------------------------------------------------------------
}
