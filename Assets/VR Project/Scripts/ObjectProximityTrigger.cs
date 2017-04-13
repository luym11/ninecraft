using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProximityTrigger : MonoBehaviour {
	NewtonVR.NVRHand m_Hand;
	Vector3 LastPosition; 
	float MaxVelocity;
	public GameObject destroyParticle;


	void Awake()
	{
		m_Hand = GetComponent < NewtonVR.NVRHand >();
	}


	void Update()
	{
		if (Input.GetKeyDown (KeyCode.R)) {
			MaxVelocity = 0;
		}

		Vector3 DeltaPosition = m_Hand.transform.position - LastPosition; 
		float Velocity = DeltaPosition.magnitude / Time.deltaTime; 
		MaxVelocity = Mathf.Max (MaxVelocity, Velocity);
		//Debug.Log (Velocity + " - " + MaxVelocity);
		LastPosition = m_Hand.transform.position; 
	}

	void OnTriggerEnter(Collider collider){
		PaletteItem paletteItem = collider.GetComponent<PaletteItem> ();
		if (paletteItem != null) {
			if (m_Hand.UseButtonPressed) {
				GameObject particalPrefab = Instantiate<GameObject> (destroyParticle);
				particalPrefab.transform.localScale = paletteItem.transform.localScale; 
				particalPrefab.transform.position = paletteItem.transform.position;
				int childNum; 
				childNum = particalPrefab.transform.childCount; 
				Debug.Log ("childNum = " + childNum); 
				for (int i = 0; i < childNum; i++) {
					particalPrefab.transform.GetChild(i).localScale= paletteItem.transform.localScale; 
				}
				paletteItem.DestroyBrick (); 
			}
		}
	}


}
