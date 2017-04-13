using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BrickDrop : MonoBehaviour {
	
	PaletteItem m_Brick; 

	GravityActivate gravityActivate; 

	void Awake(){
		m_Brick = GetComponent<PaletteItem> (); 
		gravityActivate = GetComponent<GravityActivate> (); 
	}
	public void TurnToKinematic(){
		if (m_Brick.Rigidbody){
			if(GravityActivate.Instance.kinematicEnabled == true){
			//if(gravityActivate.kinematicEnabled == true){ // in the Kinematic Mode
				m_Brick.Rigidbody.isKinematic = true; 
			}
			m_Brick.tag = "Brick"; 
		}
		//m_Brick.transform.position;
	}



	public void FitTheGrid(){
		Vector3 position = m_Brick.transform.position;

		position.x = UnityEngine.Mathf.Round (m_Brick.transform.position.x * 5) / 5; 
		position.y = UnityEngine.Mathf.Round (m_Brick.transform.position.y * 5) / 5;
		position.z = UnityEngine.Mathf.Round (m_Brick.transform.position.z * 5) / 5;

		m_Brick.transform.position = position;

	}

	public void IdentityDirection(){
		m_Brick.transform.rotation = UnityEngine.Quaternion.identity;
	}
}
